/* 
 *  # API versions   We supports several API versions simultaneously:   - Current **stable** API version is 2.0, version 1.0 is deprecated.   - API version 3.0 is **under development** and subject to change. We plan to freeze this API version at the late 2020.  # Limitations   Current API has the following limitations:   - URL should not be longer than 2000 symbols   - Maximal order depth for both archive and active orders is limited   by settings (1000 by default)  # Authentication  Bridge offers two form of authentication:   - HTTP Basic auth   - [JWT token](https://jwt.io) auth, which can be used as both authorization header and query parameter. Only HS256   (HMAC-SHA256) signature algo is supported. `iss` claim is used to pass clientId, `sub` is for application id. For example,   build JWT from following parts:    header:    ```   { \"alg\": \"HS256\", \"typ\": \"JWT\" }   ```    payload:    ```   {     \"sub\": \"77b378e8-3a30-4f85-9017-e839501f7589\",     \"iss\": \"469a8180-51fb-408f-a1f0-c3775eeb6ade\",     \"iat\": 1481850484,     \"aud\": [       \"feed\",       \"symbols\",       \"ohlc\",       \"crossrates\"     ]   }   ```    base64-encoded and concatenated with dot:    ```   eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI3N2IzNzhlOC0zYTMwLTRmODUtOTAxNy1lODM5NTAxZjc1ODkiLCJpc3MiOiI0NjlhODE4MC01MWZiLTQwOGYtYTFmMC1jMzc3NWVlYjZhZGUiLCJpYXQiOjE0ODE4NTA0ODQsImF1ZCI6WyJmZWVkIiwic3ltYm9scyIsIm9obGMiLCJjcm9zc3JhdGVzIl19   ```    and finally signed with shared secret:    ```   eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI3N2IzNzhlOC0zYTMwLTRmODUtOTAxNy1lODM5NTAxZjc1ODkiLCJpc3MiOiI0NjlhODE4MC01MWZiLTQwOGYtYTFmMC1jMzc3NWVlYjZhZGUiLCJpYXQiOjE0ODE4NTA0ODQsImF1ZCI6WyJmZWVkIiwic3ltYm9scyIsIm9obGMiLCJjcm9zc3JhdGVzIl19.Byn6aPDoMnaQUSGMnnddj2rI-noP9cQwa8JLJswgNGk   ```  <security-definitions />       
 *
 * OpenAPI spec version: 1.0.0
 * 
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */
using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;
using SwaggerDateConverter = IO.Swagger.Client.SwaggerDateConverter;

namespace IO.Swagger.Model
{
    /// <summary>
    /// Crossrate
    /// </summary>
    [DataContract]
        public partial class Crossrate :  IEquatable<Crossrate>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Crossrate" /> class.
        /// </summary>
        /// <param name="pair">crossrate pair (required).</param>
        /// <param name="symbolId">optional symbol id, which can be used to request history or subscribe to feed.</param>
        /// <param name="rate">current crossrate (required).</param>
        public Crossrate(string pair = default(string), string symbolId = default(string), string rate = default(string))
        {
            // to ensure "pair" is required (not null)
            if (pair == null)
            {
                throw new InvalidDataException("pair is a required property for Crossrate and cannot be null");
            }
            else
            {
                this.Pair = pair;
            }
            // to ensure "rate" is required (not null)
            if (rate == null)
            {
                throw new InvalidDataException("rate is a required property for Crossrate and cannot be null");
            }
            else
            {
                this.Rate = rate;
            }
            this.SymbolId = symbolId;
        }
        
        /// <summary>
        /// crossrate pair
        /// </summary>
        /// <value>crossrate pair</value>
        [DataMember(Name="pair", EmitDefaultValue=false)]
        public string Pair { get; set; }

        /// <summary>
        /// optional symbol id, which can be used to request history or subscribe to feed
        /// </summary>
        /// <value>optional symbol id, which can be used to request history or subscribe to feed</value>
        [DataMember(Name="symbolId", EmitDefaultValue=false)]
        public string SymbolId { get; set; }

        /// <summary>
        /// current crossrate
        /// </summary>
        /// <value>current crossrate</value>
        [DataMember(Name="rate", EmitDefaultValue=false)]
        public string Rate { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Crossrate {\n");
            sb.Append("  Pair: ").Append(Pair).Append("\n");
            sb.Append("  SymbolId: ").Append(SymbolId).Append("\n");
            sb.Append("  Rate: ").Append(Rate).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
  
        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public virtual string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="input">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object input)
        {
            return this.Equals(input as Crossrate);
        }

        /// <summary>
        /// Returns true if Crossrate instances are equal
        /// </summary>
        /// <param name="input">Instance of Crossrate to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Crossrate input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.Pair == input.Pair ||
                    (this.Pair != null &&
                    this.Pair.Equals(input.Pair))
                ) && 
                (
                    this.SymbolId == input.SymbolId ||
                    (this.SymbolId != null &&
                    this.SymbolId.Equals(input.SymbolId))
                ) && 
                (
                    this.Rate == input.Rate ||
                    (this.Rate != null &&
                    this.Rate.Equals(input.Rate))
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hashCode = 41;
                if (this.Pair != null)
                    hashCode = hashCode * 59 + this.Pair.GetHashCode();
                if (this.SymbolId != null)
                    hashCode = hashCode * 59 + this.SymbolId.GetHashCode();
                if (this.Rate != null)
                    hashCode = hashCode * 59 + this.Rate.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// To validate all properties of the instance
        /// </summary>
        /// <param name="validationContext">Validation context</param>
        /// <returns>Validation Result</returns>
        IEnumerable<System.ComponentModel.DataAnnotations.ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }
}
