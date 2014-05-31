using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;

namespace MerchantWarrior
{
    /// <summary>
    /// Represents the base response object.
    /// </summary>
    [Serializable]
    public abstract class BaseResponse
    {
        /// <summary>
        /// Code of the response.
        /// </summary>
        public int ResponseCode { get; set; }

        /// <summary>
        /// Message of the response.
        /// </summary>
        public string ResponseMessage { get; set; }

        /// <summary>
        /// Raw fields from the response.
        /// </summary>
        public NameValueCollection Fields { get; set; }
    }
}
