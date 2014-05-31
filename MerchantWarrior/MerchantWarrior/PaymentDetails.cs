using System;
using System.Collections.Generic;
using System.Text;

namespace MerchantWarrior
{
    /// <summary>
    /// Details for a payment.
    /// </summary>
    [Serializable]
    public class PaymentDetails
    {
        /// <summary>
        /// Card number for the payment.
        /// </summary>
        public string CardNumber { get; set; }

        /// <summary>
        /// Expiry month for the card (1 to 12).
        /// </summary>
        public int CardExpiryMonth { get; set; }

        /// <summary>
        /// 4-digit or 2-digit expiry year for the card.
        /// </summary>
        public int CardExpiryYear { get; set; }

        /// <summary>
        /// Name on the card.
        /// </summary>
        public string CardName { get; set; }

        /// <summary>
        /// CSC details for the card.
        /// </summary>
        public string CardCsc { get; set; }
    }
}
