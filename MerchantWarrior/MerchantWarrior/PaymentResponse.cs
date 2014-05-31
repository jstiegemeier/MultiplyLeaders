using System;
using System.Collections.Generic;
using System.Text;

namespace MerchantWarrior
{
    /// <summary>
    /// Response to a payment.
    /// </summary>
    [Serializable]
    public class PaymentResponse : BaseResponse
    {
        /// <summary>
        /// ID of the resulting transaction.
        /// </summary>
        public string TransactionId { get; set; }

        /// <summary>
        /// Authorization code of the payment if applicable.
        /// </summary>
        public string AuthCode { get; set; }

        /// <summary>
        /// Message for the authorization of the payment if applicable.
        /// </summary>
        public string AuthMessage { get; set; }

        /// <summary>
        /// Code for the authorization response if applicable.
        /// </summary>
        public int? AuthResponseCode { get; set; }
    }
}
