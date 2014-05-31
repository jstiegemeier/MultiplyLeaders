using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Security.Cryptography;
using System.Net;
using System.IO;
using System.Xml;
using System.Globalization;
using System.Collections.Specialized;

namespace MerchantWarrior
{
    /// <summary>
    /// MerchantWarrior API client proxy.
    /// </summary>
    public class Client
    {
        private string m_MerchantUuid;
        private string m_ApiKey;
        private string m_ApiPassphrase;
        private bool m_TestMode;

        /// <summary>
        /// Create a new instance of the API client.
        /// </summary>
        /// <param name="merchantUuid">UUID of your merchant account.</param>
        /// <param name="apiKey">Key for the API integration.</param>
        /// <param name="apiPassphrase">API passphrase for signing messages.</param>
        public Client(string merchantUuid, string apiKey, string apiPassphrase)
            : this(merchantUuid, apiKey, apiPassphrase, false)
        {
        }

        /// <summary>
        /// Create a new instance of the API client.
        /// </summary>
        /// <param name="merchantUuid">UUID of your merchant account.</param>
        /// <param name="apiKey">Key for the API integration.</param>
        /// <param name="testMode">Specifies whether or not to perform transactions in test mode.</param>
        public Client(string merchantUuid, string apiKey, string apiPassphrase, bool testMode)
        {
            m_MerchantUuid = merchantUuid;
            m_ApiKey = apiKey;
            m_ApiPassphrase = apiPassphrase;
            m_TestMode = testMode;
        }

        /// <summary>
        /// The URL to the API.
        /// </summary>
        protected virtual string ApiUrl
        {
            get
            {
                if (m_TestMode)
                    return "https://base.merchantwarrior.com/post/";
                else
                    return "https://api.merchantwarrior.com/post/";
            }
        }

        /// <summary>
        /// The URL to the token API.
        /// </summary>
        protected virtual string TokenUrl
        {
            get
            {
                if (m_TestMode)
                    return "https://base.merchantwarrior.com/token/";
                else
                    return "https://api.merchantwarrior.com/token/";
            }
        }

        /// <summary>
        /// Processes a card payment.
        /// </summary>
        /// <param name="transactionAmount">Amount of the transaction.</param>
        /// <param name="transactionCurrency">3-letter ISO currency code for the transaction.</param>
        /// <param name="transactionProduct">Product being transacted.</param>
        /// <param name="customerDetails">Details of the customer.</param>
        /// <param name="paymentDetails">Details of the payment.</param>
        /// <returns>Response to the payment request.</returns>
        public PaymentResponse ProcessCard(decimal transactionAmount, string transactionCurrency, string transactionProduct,
            CustomerDetails customerDetails, PaymentDetails paymentDetails)
        {
            if (string.IsNullOrEmpty(transactionCurrency))
                throw new ArgumentNullException("transactionCurrency");

            if (string.IsNullOrEmpty(transactionProduct))
                throw new ArgumentNullException("transactionProduct");

            if (customerDetails == null)
                throw new ArgumentNullException("customerDetails");

            if (paymentDetails == null)
                throw new ArgumentNullException("paymentDetails");

            StringBuilder postFields = new StringBuilder();

            postFields.Append("method=processCard&");
            postFields.Append(string.Format("merchantUUID={0}&", HttpUtility.UrlEncode(m_MerchantUuid)));
            postFields.Append(string.Format("apiKey={0}&", HttpUtility.UrlEncode(m_ApiKey)));
            postFields.Append(string.Format("transactionAmount={0:0.00}&", transactionAmount));
            postFields.Append(string.Format("transactionCurrency={0}&", HttpUtility.UrlEncode(transactionCurrency)));
            postFields.Append(string.Format("transactionProduct={0}&", HttpUtility.UrlEncode(transactionProduct)));
            postFields.Append(string.Format("customerName={0}&", HttpUtility.UrlEncode(customerDetails.Name)));
            postFields.Append(string.Format("customerCountry={0}&", HttpUtility.UrlEncode(customerDetails.Country)));
            postFields.Append(string.Format("customerState={0}&", HttpUtility.UrlEncode(customerDetails.State)));
            postFields.Append(string.Format("customerCity={0}&", HttpUtility.UrlEncode(customerDetails.City)));
            postFields.Append(string.Format("customerAddress={0}&", HttpUtility.UrlEncode(customerDetails.Address)));
            postFields.Append(string.Format("customerPostCode={0}&", HttpUtility.UrlEncode(customerDetails.PostCode)));
            if (!string.IsNullOrEmpty(customerDetails.Phone))
                postFields.Append(string.Format("customerPhone={0}&", HttpUtility.UrlEncode(customerDetails.Phone)));
            if (!string.IsNullOrEmpty(customerDetails.Email))
                postFields.Append(string.Format("customerEmail={0}&", HttpUtility.UrlEncode(customerDetails.Email)));
            if (!string.IsNullOrEmpty(customerDetails.IpAddress))
                postFields.Append(string.Format("customerIP={0}&", HttpUtility.UrlEncode(customerDetails.IpAddress)));

            postFields.Append(string.Format("paymentCardNumber={0}&", HttpUtility.UrlEncode(paymentDetails.CardNumber)));
            postFields.Append(string.Format("paymentCardExpiry={0:00}{1:00}&", paymentDetails.CardExpiryMonth, paymentDetails.CardExpiryYear % 100));
            postFields.Append(string.Format("paymentCardName={0}&", HttpUtility.UrlEncode(paymentDetails.CardName)));
            postFields.Append(string.Format("paymentCardCSC={0}&", HttpUtility.UrlEncode(paymentDetails.CardCsc)));

            postFields.Append(string.Format("hash={0}", this.GenerateHash(transactionAmount, transactionCurrency)));

            return this.PostRequest(postFields.ToString());
        }

        /// <summary>
        /// Refunds a previously processed card.
        /// </summary>
        /// <param name="transactionAmount">Amount of the transaction.</param>
        /// <param name="transactionCurrency">Currency of the transaction.</param>
        /// <param name="transactionId">ID of the transaction.</param>
        /// <param name="refundAmount">Amount to refund the card.</param>
        /// <returns>Response to the refund request.</returns>
        public PaymentResponse RefundCard(decimal transactionAmount, string transactionCurrency, string transactionId, decimal refundAmount)
        {
            if (string.IsNullOrEmpty(transactionCurrency))
                throw new ArgumentNullException("transactionCurrency");

            if (string.IsNullOrEmpty(transactionId))
                throw new ArgumentNullException("transactionId");

            StringBuilder postFields = new StringBuilder();

            postFields.Append("method=refundCard&");
            postFields.Append(string.Format("merchantUUID={0}&", HttpUtility.UrlEncode(m_MerchantUuid)));
            postFields.Append(string.Format("apiKey={0}&", HttpUtility.UrlEncode(m_ApiKey)));
            postFields.Append(string.Format("transactionAmount={0:0.00}&", transactionAmount));
            postFields.Append(string.Format("transactionCurrency={0}&", HttpUtility.UrlEncode(transactionCurrency)));
            postFields.Append(string.Format("transactionID={0}&", HttpUtility.UrlEncode(transactionId)));
            postFields.Append(string.Format("refundAmount={0:0.00}&", refundAmount));

            postFields.Append(string.Format("hash={0}", this.GenerateHash(transactionAmount, transactionCurrency)));

            return this.PostRequest(postFields.ToString());
        }

        /// <summary>
        /// Queries a previous transaction.
        /// </summary>
        /// <param name="transactionId">ID of the transaction.</param>
        /// <param name="method">Original method of the transaction.</param>
        /// <param name="extended">Supply extended information.</param>
        /// <returns>Original response of the processed transaction.</returns>
        public PaymentResponse QueryCard(string transactionId, bool extended)
        {
            if (string.IsNullOrEmpty(transactionId))
                throw new ArgumentNullException("transactionId");

            StringBuilder postFields = new StringBuilder();

            postFields.Append("method=queryCard&");
            postFields.Append(string.Format("merchantUUID={0}&", HttpUtility.UrlEncode(m_MerchantUuid)));
            postFields.Append(string.Format("apiKey={0}&", HttpUtility.UrlEncode(m_ApiKey)));
            postFields.Append(string.Format("transactionID={0}&", HttpUtility.UrlEncode(transactionId)));
            if (extended)
                postFields.Append("extended=1&");

            postFields.Append(string.Format("hash={0}", this.GenerateQueryHash(transactionId)));

            return this.PostRequest(postFields.ToString());
        }

        /// <summary>
        /// Adds a card for a token payment.
        /// </summary>
        /// <param name="cardName">Name on the card.</param>
        /// <param name="cardNumber">Number of the card.</param>
        /// <param name="expiryMonth">Expiry month of the card.</param>
        /// <param name="expiryYear">Expiry year of the card.</param>
        /// <returns>Response to the add card request.</returns>
        public TokenResponse AddCard(string cardName, string cardNumber, int expiryMonth, int expiryYear)
        {
            if (string.IsNullOrEmpty(cardName))
                throw new ArgumentNullException("cardName");

            if (string.IsNullOrEmpty(cardNumber))
                throw new ArgumentNullException("cardNumber");

            StringBuilder postFields = new StringBuilder();

            postFields.Append(string.Format("merchantUUID={0}&", HttpUtility.UrlEncode(m_MerchantUuid)));
            postFields.Append(string.Format("apiKey={0}&", HttpUtility.UrlEncode(m_ApiKey)));
            postFields.Append(string.Format("cardName={0}&", HttpUtility.UrlEncode(cardName)));
            postFields.Append(string.Format("cardNumber={0}&", HttpUtility.UrlEncode(cardNumber)));
            postFields.Append(string.Format("cardExpiryMonth={0:00}&", expiryMonth));
            postFields.Append(string.Format("cardExpiryYear={0:00}", expiryYear));

            return this.PostTokenRequest(postFields.ToString(), string.Format("{0}{1}", this.TokenUrl, "addCard"));
        }

        /// <summary>
        /// Removes a card for a token payment.
        /// </summary>
        /// <param name="cardId">ID of the card.</param>
        /// <param name="cardKey">Secret key for the card.</param>
        /// <returns>Response to the remove card request.</returns>
        public TokenResponse RemoveCard(int cardId, string cardKey)
        {
            if (string.IsNullOrEmpty(cardKey))
                throw new ArgumentNullException("cardKey");

            StringBuilder postFields = new StringBuilder();

            postFields.Append(string.Format("merchantUUID={0}&", HttpUtility.UrlEncode(m_MerchantUuid)));
            postFields.Append(string.Format("apiKey={0}&", HttpUtility.UrlEncode(m_ApiKey)));
            postFields.Append(string.Format("cardID={0}&", cardId.ToString()));
            postFields.Append(string.Format("cardKey={0}&", HttpUtility.UrlEncode(cardKey)));

            return this.PostTokenRequest(postFields.ToString(), string.Format("{0}{1}", this.TokenUrl, "removeCard"));
        }

        /// <summary>
        /// Gets the information for a stored token card.
        /// </summary>
        /// <param name="cardId">ID of the card.</param>
        /// <param name="cardKey">Secret key for the card.</param>
        /// <returns>Response to the remove card request.</returns>
        public TokenCardInfoResponse GetCardInfo(int cardId, string cardKey)
        {
            if (string.IsNullOrEmpty(cardKey))
                throw new ArgumentNullException("cardKey");

            StringBuilder postFields = new StringBuilder();

            postFields.Append(string.Format("merchantUUID={0}&", HttpUtility.UrlEncode(m_MerchantUuid)));
            postFields.Append(string.Format("apiKey={0}&", HttpUtility.UrlEncode(m_ApiKey)));
            postFields.Append(string.Format("cardID={0}&", cardId.ToString()));
            postFields.Append(string.Format("cardKey={0}&", HttpUtility.UrlEncode(cardKey)));

            return this.PostRequest<TokenCardInfoResponse>(postFields.ToString(), string.Format("{0}{1}", this.TokenUrl, "cardInfo"));
        }

        /// <summary>
        /// Processes a token card payment.
        /// </summary>
        /// <param name="transactionAmount">Amount of the transaction.</param>
        /// <param name="transactionCurrency">3-letter ISO currency code for the transaction.</param>
        /// <param name="transactionProduct">Product being transacted.</param>
        /// <param name="customerDetails">Details of the customer.</param>
        /// <param name="cardId">ID of the card to process.</param>
        /// <param name="cardKey">Secret key of the card to process.</param>
        /// <returns>Response to the payment request.</returns>
        public PaymentResponse ProcessCard(decimal transactionAmount, string transactionCurrency, string transactionProduct,
            CustomerDetails customerDetails, int cardId, string cardKey)
        {
            if (string.IsNullOrEmpty(transactionCurrency))
                throw new ArgumentNullException("transactionCurrency");

            if (string.IsNullOrEmpty(transactionProduct))
                throw new ArgumentNullException("transactionProduct");

            if (customerDetails == null)
                throw new ArgumentNullException("customerDetails");

            if (string.IsNullOrEmpty(cardKey))
                throw new ArgumentNullException("cardKey");

            StringBuilder postFields = new StringBuilder();

            postFields.Append(string.Format("merchantUUID={0}&", HttpUtility.UrlEncode(m_MerchantUuid)));
            postFields.Append(string.Format("apiKey={0}&", HttpUtility.UrlEncode(m_ApiKey)));
            postFields.Append(string.Format("transactionAmount={0:0.00}&", transactionAmount));
            postFields.Append(string.Format("transactionCurrency={0}&", HttpUtility.UrlEncode(transactionCurrency)));
            postFields.Append(string.Format("transactionProduct={0}&", HttpUtility.UrlEncode(transactionProduct)));
            postFields.Append(string.Format("customerName={0}&", HttpUtility.UrlEncode(customerDetails.Name)));
            postFields.Append(string.Format("customerCountry={0}&", HttpUtility.UrlEncode(customerDetails.Country)));
            postFields.Append(string.Format("customerState={0}&", HttpUtility.UrlEncode(customerDetails.State)));
            postFields.Append(string.Format("customerCity={0}&", HttpUtility.UrlEncode(customerDetails.City)));
            postFields.Append(string.Format("customerAddress={0}&", HttpUtility.UrlEncode(customerDetails.Address)));
            postFields.Append(string.Format("customerPostCode={0}&", HttpUtility.UrlEncode(customerDetails.PostCode)));
            if (!string.IsNullOrEmpty(customerDetails.Phone))
                postFields.Append(string.Format("customerPhone={0}&", HttpUtility.UrlEncode(customerDetails.Phone)));
            if (!string.IsNullOrEmpty(customerDetails.Email))
                postFields.Append(string.Format("customerEmail={0}&", HttpUtility.UrlEncode(customerDetails.Email)));
            if (!string.IsNullOrEmpty(customerDetails.IpAddress))
                postFields.Append(string.Format("customerIP={0}&", HttpUtility.UrlEncode(customerDetails.IpAddress)));

            postFields.Append(string.Format("paymentCardID={0}&", cardId.ToString()));
            postFields.Append(string.Format("paymentCardKey={0}&", HttpUtility.UrlEncode(cardKey)));

            postFields.Append(string.Format("hash={0}", this.GenerateHash(transactionAmount, transactionCurrency)));

            return this.PostRequest(postFields.ToString(), string.Format("{0}{1}", this.TokenUrl, "processCard"));
        }

        /// <summary>
        /// Posts a request to the API.
        /// </summary>
        /// <param name="postFields">Fields to post.</param>
        /// <param name="url">URL to which to post the request.</param>
        /// <returns>Response to the payment request.</returns>
        protected virtual PaymentResponse PostRequest(string postFields, string url)
        {
            return this.PostRequest<PaymentResponse>(postFields, url);
        }

        /// <summary>
        /// Posts a request to the API.
        /// </summary>
        /// <param name="postFields">Fields to post.</param>
        /// <returns>Response to the payment request.</returns>
        protected virtual PaymentResponse PostRequest(string postFields)
        {
            return this.PostRequest(postFields, this.ApiUrl);
        }

        /// <summary>
        /// Posts a request to the API.
        /// </summary>
        /// <param name="postFields">Fields to post.</param>
        /// <returns>Response to the payment request.</returns>
        protected virtual TokenResponse PostTokenRequest(string postFields, string url)
        {
            return this.PostRequest<TokenResponse>(postFields, url);
        }

        /// <summary>
        /// Posts a request to the API.
        /// </summary>
        /// <param name="postFields">Fields to post.</param>
        /// <param name="url">URL to which to post the request.</param>
        /// <typeparam name="T">Type of the resposne to return.</typeparam>
        /// <returns>Response to the payment request.</returns>
        protected virtual T PostRequest<T>(string postFields, string url)
            where T : BaseResponse, new()
        {
            byte[] postBytes = Encoding.ASCII.GetBytes(postFields);

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.ContentLength = postBytes.Length;

            using (Stream stream = webRequest.GetRequestStream())
            {
                stream.Write(postBytes, 0, postBytes.Length);
            }

            WebResponse webResponse = webRequest.GetResponse();
            string responseString;
            using (Stream responseStream = webResponse.GetResponseStream())
            {
                using (StreamReader streamReader = new StreamReader(responseStream))
                {
                    responseString = streamReader.ReadToEnd();
                }
            }

            XmlDocument response = new XmlDocument();
            response.LoadXml(responseString);

            return this.SerializeResponse<T>(response);
        }

        /// <summary>
        /// Serializes a payment response.
        /// </summary>
        /// <param name="response">Response XML.</param>
        /// <typeparam name="T">Type of the resposne to serialize.</typeparam>
        /// <returns>A payment response object.</returns>
        protected virtual T SerializeResponse<T>(XmlDocument response)
            where T : BaseResponse, new()
        {
            T serializedResponse = new T();
            PaymentResponse paymentResponse = serializedResponse as PaymentResponse;
            TokenResponse tokenResponse = serializedResponse as TokenResponse;
            TokenCardInfoResponse infoResponse = serializedResponse as TokenCardInfoResponse;
            serializedResponse.Fields = new NameValueCollection();

            foreach (XmlNode node in response.DocumentElement.ChildNodes)
            {
                if (!(node is XmlElement))
                    continue;

                XmlElement element = (XmlElement)node;

                serializedResponse.Fields[element.Name] = element.InnerText;

                switch (element.Name)
                {
                    case "responseCode":
                        ((BaseResponse)serializedResponse).ResponseCode = int.Parse(element.InnerText);
                        break;

                    case "responseMessage":
                        ((BaseResponse)serializedResponse).ResponseMessage = element.InnerText;
                        break;

                    case "transactionID":
                        if (paymentResponse != null)
                            paymentResponse.TransactionId = element.InnerText;
                        break;

                    case "authCode":
                        if (paymentResponse != null)
                            paymentResponse.AuthCode = element.InnerText;
                        break;

                    case "authMessage":
                        if (paymentResponse != null)
                            paymentResponse.AuthMessage = element.InnerText;
                        break;

                    case "authResponseCode":
                        if (paymentResponse != null)
                            paymentResponse.AuthResponseCode = int.Parse(element.InnerText);
                        break;

                    case "cardID":
                        if (tokenResponse != null)
                            tokenResponse.CardId = int.Parse(element.InnerText);
                        else if (infoResponse != null)
                            infoResponse.CardId = int.Parse(element.InnerText);
                        break;

                    case "cardKey":
                        if (tokenResponse != null)
                            tokenResponse.CardKey = element.InnerText;
                        break;

                    case "cardName":
                        if (infoResponse != null)
                            infoResponse.CardName = element.InnerText;
                        break;

                    case "cardExpiryMonth":
                        if (infoResponse != null)
                            infoResponse.ExpiryMonth = int.Parse(element.InnerText);
                        break;

                    case "cardExpiryYear":
                        if (infoResponse != null)
                            infoResponse.ExpiryYear = int.Parse(element.InnerText);
                        break;

                    case "cardNumberFirst":
                        if (infoResponse != null)
                            infoResponse.CardNumberFirst = element.InnerText;
                        break;

                    case "cardNumberLast":
                        if (infoResponse != null)
                            infoResponse.CardNumberLast = element.InnerText;
                        break;

                    case "cardAdded":
                        if (infoResponse != null)
                            infoResponse.DateAdded = DateTime.ParseExact(element.InnerText, "yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture);
                        break;
                }
            }

            return serializedResponse;
        }

        /// <summary>
        /// Generates a hash for signing messages.
        /// </summary>
        /// <param name="transactionAmount">Amount of the transaction.</param>
        /// <param name="transactionCurrency">Currency for the transaction.</param>
        /// <returns>A hex-encoded MD5 hash to include as signing for the message.</returns>
        protected virtual string GenerateHash(decimal transactionAmount, string transactionCurrency)
        {
            string stringToHash = string.Format("{0}{1}{2:0.00}{3}", m_ApiPassphrase, m_MerchantUuid, transactionAmount, transactionCurrency).ToLower();
            byte[] bytesToHash = Encoding.ASCII.GetBytes(stringToHash);

            MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
            byte[] hash = provider.ComputeHash(bytesToHash);

            StringBuilder hashString = new StringBuilder();
            foreach (byte hashByte in hash)
                hashString.Append(hashByte.ToString("x2"));
            return hashString.ToString();
        }

        /// <summary>
        /// Generates a hash for signing query messages.
        /// </summary>
        /// <param name="transactionId">ID of the transaction.</param>
        /// <returns>A hex-encoded MD5 hash to include as signing for the query message.</returns>
        protected virtual string GenerateQueryHash(string transactionId)
        {
            string stringToHash = string.Format("{0}{1}{2}", m_ApiPassphrase, m_MerchantUuid, transactionId).ToLower();
            byte[] bytesToHash = Encoding.ASCII.GetBytes(stringToHash);

            MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
            byte[] hash = provider.ComputeHash(bytesToHash);

            StringBuilder hashString = new StringBuilder();
            foreach (byte hashByte in hash)
                hashString.Append(hashByte.ToString("x2"));
            return hashString.ToString();
        }
    }
}
