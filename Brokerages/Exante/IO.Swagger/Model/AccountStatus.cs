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
    /// account status response
    /// </summary>
    [DataContract]
        public partial class AccountStatus :  IEquatable<AccountStatus>, IValidatableObject
    {
        /// <summary>
        /// account status
        /// </summary>
        /// <value>account status</value>
        [JsonConverter(typeof(StringEnumConverter))]
                public enum StatusEnum
        {
            /// <summary>
            /// Enum ReadOnly for value: ReadOnly
            /// </summary>
            [EnumMember(Value = "ReadOnly")]
            ReadOnly = 1,
            /// <summary>
            /// Enum CloseOnly for value: CloseOnly
            /// </summary>
            [EnumMember(Value = "CloseOnly")]
            CloseOnly = 2,
            /// <summary>
            /// Enum Full for value: Full
            /// </summary>
            [EnumMember(Value = "Full")]
            Full = 3        }
        /// <summary>
        /// account status
        /// </summary>
        /// <value>account status</value>
        [DataMember(Name="status", EmitDefaultValue=false)]
        public StatusEnum Status { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountStatus" /> class.
        /// </summary>
        /// <param name="status">account status (required).</param>
        /// <param name="accountId">account ID (required).</param>
        public AccountStatus(StatusEnum status = default(StatusEnum), string accountId = default(string))
        {
            // to ensure "status" is required (not null)
            if (status == null)
            {
                throw new InvalidDataException("status is a required property for AccountStatus and cannot be null");
            }
            else
            {
                this.Status = status;
            }
            // to ensure "accountId" is required (not null)
            if (accountId == null)
            {
                throw new InvalidDataException("accountId is a required property for AccountStatus and cannot be null");
            }
            else
            {
                this.AccountId = accountId;
            }
        }
        

        /// <summary>
        /// account ID
        /// </summary>
        /// <value>account ID</value>
        [DataMember(Name="accountId", EmitDefaultValue=false)]
        public string AccountId { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class AccountStatus {\n");
            sb.Append("  Status: ").Append(Status).Append("\n");
            sb.Append("  AccountId: ").Append(AccountId).Append("\n");
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
            return this.Equals(input as AccountStatus);
        }

        /// <summary>
        /// Returns true if AccountStatus instances are equal
        /// </summary>
        /// <param name="input">Instance of AccountStatus to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(AccountStatus input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.Status == input.Status ||
                    (this.Status != null &&
                    this.Status.Equals(input.Status))
                ) && 
                (
                    this.AccountId == input.AccountId ||
                    (this.AccountId != null &&
                    this.AccountId.Equals(input.AccountId))
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
                if (this.Status != null)
                    hashCode = hashCode * 59 + this.Status.GetHashCode();
                if (this.AccountId != null)
                    hashCode = hashCode * 59 + this.AccountId.GetHashCode();
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
