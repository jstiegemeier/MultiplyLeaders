using System;
using System.Collections.Generic;
using System.Text;
using MerchantWarrior;

namespace MerchantWarrior.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("API Samples:");
            Console.WriteLine();
            RunApiSamples();

            Console.WriteLine();
            Console.WriteLine("Token Samples:");
            Console.WriteLine();
            RunTokenSamples();
        }

        static void RunApiSamples()
        {
            Client client = new Client("MERCHANTUUID", "APIKEY", "APIPASSPHRASE", true);

            PaymentResponse response = client.ProcessCard(10.00m, "AUD", "Test Product",
                new CustomerDetails()
                {
                    Address = "1 Ann Street",
                    City = "Brisbane",
                    State = "QLD",
                    Country = "AU",
                    Email = "test@test.com",
                    IpAddress = "127.0.0.1",
                    Name = "Test Customer",
                    Phone = "+61730003000",
                    PostCode = "4000"
                },
                new PaymentDetails()
                {
                    CardNumber = "4005550000000001",
                    CardExpiryMonth = 5,
                    CardExpiryYear = 2013,
                    CardCsc = "123",
                    CardName = "Test Customer"
                });

            Console.WriteLine("ProcessCard results:");
            Console.WriteLine("Response Code: " + response.ResponseCode.ToString());
            Console.WriteLine("Response Message: " + response.ResponseMessage);
            Console.WriteLine("Authorization Code: " + response.AuthCode);
            Console.WriteLine("Transaction ID: " + response.TransactionId);

            Console.WriteLine();
            Console.WriteLine("Press any key to perform the refund.");

            Console.ReadKey();

            PaymentResponse refundResponse = client.RefundCard(10.00m, "AUD", response.TransactionId, 10.00m);

            Console.WriteLine();
            Console.WriteLine("RefundCard results:");
            Console.WriteLine("Response Code: " + refundResponse.ResponseCode.ToString());
            Console.WriteLine("Response Message: " + refundResponse.ResponseMessage);
            Console.WriteLine("Authorization Code: " + refundResponse.AuthCode);
            Console.WriteLine("Transaction ID: " + refundResponse.TransactionId);

            Console.WriteLine();
            Console.WriteLine("Press any key to perform the query.");

            Console.ReadKey();

            PaymentResponse queryResponse = client.QueryCard(response.TransactionId, true);

            Console.WriteLine();
            Console.WriteLine("QueryCard results:");
            Console.WriteLine("Response Code: " + queryResponse.ResponseCode.ToString());
            Console.WriteLine("Response Message: " + queryResponse.ResponseMessage);
            Console.WriteLine("Authorization Code: " + queryResponse.AuthCode);
            Console.WriteLine("Transaction ID: " + queryResponse.TransactionId);

            Console.ReadKey();
        }

        static void RunTokenSamples()
        {
            Client client = new Client("MERCHANTUUID", "APIKEY", "APIPASSPHRASE", true);

            TokenResponse response = client.AddCard("Test User", "4005550000000001", 5, 13);

            Console.WriteLine("AddCard results:");
            Console.WriteLine("Response Code: " + response.ResponseCode.ToString());
            Console.WriteLine("Response Message: " + response.ResponseMessage);
            Console.WriteLine("Card ID: " + response.CardId.ToString());
            Console.WriteLine("Card Key: " + response.CardKey);

            Console.WriteLine();
            Console.WriteLine("Press any key to perform the card info.");

            Console.ReadKey();

            TokenCardInfoResponse infoResponse = client.GetCardInfo(response.CardId, response.CardKey);

            Console.WriteLine();
            Console.WriteLine("GetCardInfo results:");
            Console.WriteLine("Response Code: " + infoResponse.ResponseCode.ToString());
            Console.WriteLine("Response Message: " + infoResponse.ResponseMessage);
            Console.WriteLine("Card ID: " + infoResponse.CardId.ToString());
            Console.WriteLine("Card Name: " + infoResponse.CardName);
            Console.WriteLine("Expiry Month: " + infoResponse.ExpiryMonth.ToString());
            Console.WriteLine("Expiry Year: " + infoResponse.ExpiryYear.ToString());
            Console.WriteLine("Card Number First: " + infoResponse.CardNumberFirst);
            Console.WriteLine("Card Number Last: " + infoResponse.CardNumberLast);
            Console.WriteLine("Date Added: " + infoResponse.DateAdded.ToString());

            Console.WriteLine();
            Console.WriteLine("Press any key to perform the process.");

            Console.ReadKey();

            PaymentResponse paymentResponse = client.ProcessCard(10.00m, "AUD", "Test Product",
                new CustomerDetails()
                {
                    Address = "1 Ann Street",
                    City = "Brisbane",
                    State = "QLD",
                    Country = "AU",
                    Email = "test@test.com",
                    IpAddress = "127.0.0.1",
                    Name = "Test Customer",
                    Phone = "+61730003000",
                    PostCode = "4000"
                }, response.CardId, response.CardKey);

            Console.WriteLine();
            Console.WriteLine("ProcessCard results:");
            Console.WriteLine("Response Code: " + paymentResponse.ResponseCode.ToString());
            Console.WriteLine("Response Message: " + paymentResponse.ResponseMessage);
            Console.WriteLine("Authorization Code: " + paymentResponse.AuthCode);
            Console.WriteLine("Transaction ID: " + paymentResponse.TransactionId);

            Console.WriteLine();
            Console.WriteLine("Press any key to perform the remove.");

            Console.ReadKey();

            response = client.RemoveCard(response.CardId, response.CardKey);

            Console.WriteLine();
            Console.WriteLine("RemoveCard results:");
            Console.WriteLine("Response Code: " + response.ResponseCode.ToString());
            Console.WriteLine("Response Message: " + response.ResponseMessage);
            Console.WriteLine("Card ID: " + response.CardId.ToString());

            Console.ReadKey();
        }
    }
}
