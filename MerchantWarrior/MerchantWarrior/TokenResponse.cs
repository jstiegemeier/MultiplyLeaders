using System;
using System.Collections.Generic;
using System.Text;

namespace MerchantWarrior
{
    /// <summary>
    /// Response to a token operation.
    /// </summary>
    public class TokenResponse : BaseResponse
    {
        /// <summary>
        /// ID of the card.
        /// </summary>
        public int CardId { get; set; }

        /// <summary>
        /// Unique card key for the card.
        /// </summary>
        public string CardKey { get; set; }
    }
}
