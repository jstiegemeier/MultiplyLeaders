using System;
using System.Collections.Generic;
using System.Text;

namespace MerchantWarrior
{
    /// <summary>
    /// Details of a customer for use in API operations.
    /// </summary>
    [Serializable]
    public class CustomerDetails
    {
        /// <summary>
        /// Name of the customer.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Country of the customer's address.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// State of the customer's address.
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// City of the customer's address.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Customer's address.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Post code of the customer's address.
        /// </summary>
        public string PostCode { get; set; }

        /// <summary>
        /// Customer's phone number.
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Customer's email address.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Customer's IP address.
        /// </summary>
        public string IpAddress { get; set; }
    }
}
