using System;
using System.Collections.Generic;
using System.Text;

namespace MerchantWarrior
{
    /// <summary>
    /// Response for a card information request.
    /// </summary>
    public class TokenCardInfoResponse : BaseResponse
    {
        /// <summary>
        /// ID of the card.
        /// </summary>
        public int CardId { get; set; }

        /// <summary>
        /// Name on the card.
        /// </summary>
        public string CardName { get; set; }

        /// <summary>
        /// Month of the expiry for the card.
        /// </summary>
        public int ExpiryMonth { get; set; }

        /// <summary>
        /// 2-digit year of the expiry for the card.
        /// </summary>
        public int ExpiryYear { get; set; }

        /// <summary>
        /// First four digits of the card number.
        /// </summary>
        public string CardNumberFirst { get; set; }

        /// <summary>
        /// Last four digits of the card number.
        /// </summary>
        public string CardNumberLast { get; set; }

        /// <summary>
        /// Date the card was added.
        /// </summary>
        public DateTime DateAdded { get; set; }
    }
}
