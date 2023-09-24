using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CleanArchitecture.Models.Attributes
{
    /// <summary>
    /// Validates a List of strings as email addresses
    /// </summary>
    public sealed class EmailAddressListAttribute : ValidationAttribute
    {
        private const string defaultError = "'{0}' contains an invalid email address.";
        /// <summary>
        /// Constructor
        /// </summary>
        public EmailAddressListAttribute() : base(defaultError)
        {
        }
        /// <summary>
        /// IsValid
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool IsValid(object value)
        {
            EmailAddressAttribute emailAttribute = new EmailAddressAttribute();
            return (value is IList<string> list && list.All(email => emailAttribute.IsValid(email)));
        }
        /// <summary>
        /// Format Error Message
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override string FormatErrorMessage(string name)
        {
            return String.Format(ErrorMessageString, name);
        }
    }
}
