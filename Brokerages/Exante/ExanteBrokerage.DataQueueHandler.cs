/*
 * QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
 * Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/

using QuantConnect.Interfaces;
using System.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Exante.Net.Enums;
using Exante.Net.Objects;
using QuantConnect.Data;
using QuantConnect.Packets;
using NodaTime;
using QuantConnect.Data.Market;
using QuantConnect.Logging;
using QuantConnect.Securities;
using QuantConnect.Util;

namespace QuantConnect.Brokerages.Exante
{
    public partial class ExanteBrokerage : IDataQueueHandler
    {
        private readonly ConcurrentDictionary<string, Symbol> _subscribedTickers =
            new ConcurrentDictionary<string, Symbol>();

        private readonly ConcurrentDictionary<string, (ExanteStreamSubscription, ExanteStreamSubscription)>
            _subscribedTickersStreamSubscriptions =
                new ConcurrentDictionary<string, (ExanteStreamSubscription, ExanteStreamSubscription)>();

        /// <summary>
        /// Subscribe to the specified configuration
        /// </summary>
        /// <param name="dataConfig">defines the parameters to subscribe to a data feed</param>
        /// <param name="newDataAvailableHandler">handler to be fired on new data available</param>
        /// <returns>The new enumerator for this subscription request</returns>
        public IEnumerator<BaseData> Subscribe(SubscriptionDataConfig dataConfig, EventHandler newDataAvailableHandler)
        {
            if (!CanSubscribe(dataConfig.Symbol))
            {
                return Enumerable.Empty<BaseData>().GetEnumerator();
            }

            var enumerator = _aggregator.Add(dataConfig, newDataAvailableHandler);
            _subscriptionManager.Subscribe(dataConfig);

            return enumerator;
        }

        /// <summary>
        /// Returns true if this data provide can handle the specified symbol
        /// </summary>
        /// <param name="symbol">The symbol to be handled</param>
        /// <returns>True if this data provider can get data for the symbol, false otherwise</returns>
        private bool CanSubscribe(Symbol symbol)
        {
            var supportedSecurityTypes = new HashSet<SecurityType>
            {
                SecurityType.Forex,
                SecurityType.Equity,
                SecurityType.Future,
                SecurityType.Option,
                SecurityType.Cfd,
                SecurityType.Index,
                SecurityType.Crypto,
            };

            // ignore unsupported security types
            if (!supportedSecurityTypes.Contains(symbol.ID.SecurityType))
            {
                return false;
            }

            // ignore universe symbols
            return !symbol.Value.Contains("-UNIVERSE-");
        }

        /// <summary>
        /// Removes the specified configuration
        /// </summary>
        /// <param name="dataConfig">Subscription config to be removed</param>
        public void Unsubscribe(SubscriptionDataConfig dataConfig)
        {
            _subscriptionManager.Unsubscribe(dataConfig);
            _aggregator.Remove(dataConfig);
        }

        /// <summary>
        /// Sets the job we're subscribing for
        /// </summary>
        /// <param name="job">Job we're subscribing for</param>
        public void SetJob(LiveNodePacket job)
        {
        }

        /// <summary>
        /// Adds the specified symbols to the subscription
        /// </summary>
        /// <param name="symbols">The symbols to be added keyed by SecurityType</param>
        /// <param name="tickType">Type of tick data</param>
        private bool Subscribe(IEnumerable<Symbol> symbols, TickType tickType)
        {
            foreach (var symbol in symbols)
            {
                if (!symbol.IsCanonical())
                {
                    var ticker = _symbolMapper.GetBrokerageSymbol(symbol);
                    if (!_subscribedTickers.ContainsKey(ticker))
                    {
                        _subscribedTickers.TryAdd(ticker, symbol);
                        var feedQuoteStream = _client.StreamClient.GetFeedQuoteStreamAsync(
                            new[] { ticker },
                            tickShort =>
                            {
                                var tick = CreateTick(tickShort);
                                if (tick != null)
                                {
                                    _aggregator.Update(tick);
                                }
                            },
                            level: ExanteQuoteLevel.BestPrice).SynchronouslyAwaitTaskResult();
                        if (!feedQuoteStream.Success)
                        {
                            Log.Error(
                                $"Exante.StreamClient.GetFeedQuoteStreamAsync({ticker}): " +
                                $"Error: {feedQuoteStream.Error}"
                            );
                        }

                        var feedTradesStream = _client.StreamClient.GetFeedTradesStreamAsync(
                            new[] { ticker },
                            feedTrade =>
                            {
                                var tick = CreateTick(feedTrade);
                                if (tick != null)
                                {
                                    _aggregator.Update(tick);
                                }
                            }).SynchronouslyAwaitTaskResult();
                        if (!feedTradesStream.Success)
                        {
                            Log.Error(
                                $"Exante.StreamClient.GetFeedTradesStreamAsync({ticker}): " +
                                $"Error: {feedTradesStream.Error}"
                            );
                        }

                        _subscribedTickersStreamSubscriptions[ticker] = (feedQuoteStream.Data, feedTradesStream.Data);
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Create a tick from the Exante feed stream data
        /// </summary>
        /// <param name="exanteFeedTrade">Exante feed stream data object</param>
        /// <returns>LEAN Tick object</returns>
        private Tick CreateTick(ExanteFeedTrade exanteFeedTrade)
        {
            var symbolId = exanteFeedTrade.SymbolId;
            if (!_subscribedTickers.TryGetValue(symbolId, out var symbol))
            {
                // Not subscribed to this symbol.
                return null;
            }

            if (exanteFeedTrade.Size == decimal.Zero)
            {
                return null;
            }

            // Convert the timestamp to exchange timezone and pass into algorithm
            var time = GetRealTimeTickTime(exanteFeedTrade.Date, symbol);

            var instrument = _client.GetSymbol(symbolId);

            var size = exanteFeedTrade.Size ?? 0m;
            var price = exanteFeedTrade.Price ?? 0m;
            return new Tick(time, symbol, "", instrument.Data.Exchange, size, price);
        }

        /// <summary>
        /// Returns a timestamp for a tick converted to the exchange time zone
        /// </summary>
        private DateTime GetRealTimeTickTime(DateTime time, Symbol symbol)
        {
            DateTimeZone exchangeTimeZone;
            if (!_symbolExchangeTimeZones.TryGetValue(symbol, out exchangeTimeZone))
            {
                // read the exchange time zone from market-hours-database
                exchangeTimeZone = MarketHoursDatabase.FromDataFolder()
                    .GetExchangeHours(symbol.ID.Market, symbol, symbol.SecurityType).TimeZone;
                _symbolExchangeTimeZones.Add(symbol, exchangeTimeZone);
            }

            return time.ConvertFromUtc(exchangeTimeZone);
        }

        /// <summary>
        /// Create a tick from the Exante tick shorts stream data
        /// </summary>
        /// <param name="exanteTickShort">Exante tick short stream data object</param>
        /// <returns>LEAN Tick object</returns>
        private Tick CreateTick(ExanteTickShort exanteTickShort)
        {
            if (!_subscribedTickers.TryGetValue(exanteTickShort.SymbolId, out var symbol))
            {
                // Not subscribed to this symbol.
                return null;
            }

            var bids = exanteTickShort.Bid.ToList();
            var asks = exanteTickShort.Ask.ToList();
            return new Tick(exanteTickShort.Date, symbol, "", "",
                bids.IsNullOrEmpty() ? decimal.Zero : bids[0].Size,
                bids.IsNullOrEmpty() ? decimal.Zero : bids[0].Price,
                asks.IsNullOrEmpty() ? decimal.Zero : asks[0].Size,
                asks.IsNullOrEmpty() ? decimal.Zero : asks[0].Price);
        }

        /// <summary>
        /// Removes the specified symbols to the subscription
        /// </summary>
        /// <param name="symbols">The symbols to be added keyed by SecurityType</param>
        /// <param name="tickType">Type of tick data</param>
        private bool Unsubscribe(IEnumerable<Symbol> symbols, TickType tickType)
        {
            foreach (var symbol in symbols)
            {
                if (!symbol.IsCanonical())
                {
                    var ticker = _symbolMapper.GetBrokerageSymbol(symbol);
                    if (_subscribedTickers.ContainsKey(ticker))
                    {
                        _subscribedTickers.TryRemove(ticker, out _);
                    }

                    if (_subscribedTickersStreamSubscriptions.ContainsKey(ticker))
                    {
                        _subscribedTickersStreamSubscriptions.TryRemove(ticker,
                            out (ExanteStreamSubscription stream1, ExanteStreamSubscription stream2) streams);
                        _client.StreamClient.StopStream(streams.stream1);
                        _client.StreamClient.StopStream(streams.stream2);
                    }
                }
            }

            return true;
        }
    }
}
