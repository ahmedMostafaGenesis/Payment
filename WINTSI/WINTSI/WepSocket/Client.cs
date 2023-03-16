using System;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GenesisCreations.PharmaTech;
using Ingenico;
using SuperWebSocket;
using Newtonsoft.Json;
using SuperSocket.ClientEngine;
using WebSocket4Net;
using ClientWebSocket = WebSocket4Net.WebSocket;

namespace WINTSI.WebSocket
{
    public static class Client
    {
        const string USER_NAME = "ptadmin";
        const string USER_HASH = "ZrK0AOQz+L1wjr2jNXZwMcgosUOeicIifZ+8fOWDur2TkV7m";
        const string USERNAME_KEY = "username";
        const string PASSWORD_KEY = "password";
        const float TOLERANCE = 0.01f;
        const bool DEBUG = true;
        public static ClientWebSocket SocketClient;

        private static bool IsProduction = false;
        private const string DevelopmentUri = "ws://127.0.0.1:9080";
        private const string ProductionUri = "ws://192.168.30.100:9080";
        static string Uri => IsProduction ? ProductionUri : DevelopmentUri;

        private enum ValidationResponse
        {
            None = 0,
            InvalidUsernameOrPassword = 1,
            ValidUsername = 2,
            ValidPassword = 3
        }
        
        public static void Initialize()
        {
            try
            {
                SocketClient = new ClientWebSocket(Uri);
                
                SocketClient.Opened += OnSocketOpened;
                SocketClient.Closed += OnSocketClosed;
                SocketClient.Error += OnSocketError;
                SocketClient.MessageReceived += OnSocketMessageReceived;

                SocketClient.Open();
            }catch (Exception ex)
            {
                Console.WriteLine($"An error occured while attempting to connect to server on {Uri}.");
                if (DEBUG) Console.WriteLine(ex.Message);
            }
        }
        
        private static void OnSocketMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Console.WriteLine(e.Message);
            try
            {
                var paymentRequest = JsonConvert.DeserializeObject<PaymentRequest>(e.Message);
                //currentSession = session;
                Console.WriteLine("[202] Payment request received.");
                if (!ValidatePaymentRequest(paymentRequest))
                {
                    Console.WriteLine("[400] Bad payment request.");
                    SocketClient.Send("Bad payment request.");
                    return;
                }

                //TBD: Process payment here
                Program.CreateRequest(paymentRequest.totalPrice);
                //TBD: Build actual <see cref="PaymentResponse"/> data here:
                // var response = new PaymentResponse(PaymentResponse.PaymentStatus.SUCCESS, paymentRequest.totalPrice,
                //     "**** **** 4942", "SOME_REFERENCE");
            }
            catch (Exception ex)
            {
                Console.WriteLine("[500] Internal server error.");
                SocketClient.Send("Internal server error.");
                if (DEBUG)
                {
                    Console.WriteLine(ex.Message);
                    SocketClient.Send(ex.Message);
                }
            }
        }

        private static void OnSocketError(object sender, ErrorEventArgs e)
        {
            Console.WriteLine($"An error occured while communicating with server on {Uri}.");
            if (DEBUG) Console.WriteLine(e.Exception.Message);
        }
        private static void OnSocketClosed(object sender, EventArgs e) => Console.WriteLine($"Connection with server on {Uri} was shutdown.");
        private static void OnSocketOpened(object sender, EventArgs e) => Console.WriteLine($"Connection with server on {Uri} was opened successfully."); // TODO: Validate session here.
        public static void SendResponse(string result, PaymentStatus status = PaymentStatus.UNKNOWN)
        {
            Console.WriteLine(status);
            var response = status == PaymentStatus.UNKNOWN
                ? new PaymentResponse(result)
                : new PaymentResponse(result, status);
            var jsonResponse = JsonConvert.SerializeObject(response, Formatting.Indented);
            PrintingManager.PrintReceiptForTransaction(response);
            //Console.WriteLine("[201] Payment successful.");
            SocketClient.Send(jsonResponse);
            /*currentSession.Close();
            currentSession = null;*/
        }
        
        static bool ValidatePaymentRequest(PaymentRequest paymentRequest)
        {
            if (paymentRequest.products.Length != paymentRequest.productCount)
            {
                if (DEBUG) Console.WriteLine("[409] Product count mismatch.");
                return false;
            }

            var totalPrice = paymentRequest.products.Sum(product => product.GetTotalPrice());
            var validPrice = Math.Abs(paymentRequest.totalPrice - totalPrice) < TOLERANCE;
            if (DEBUG && !validPrice) Console.WriteLine("[409] Price mismatch.");
            return validPrice;
        }

        static bool ValidateSession(WebSocketSession session)
        {
            if (ValidatePrameters(session.Path)) return true;
            Console.WriteLine("[403] Client access attempt denied.");
            session.Send("12152 - Access denied.");
            session.Close();
            return false;
        }

        static bool ValidatePrameters(string sessionPath)
        {
            var parameters = HttpUtility.ParseQueryString(sessionPath);
            var validUsername = false;
            var validPassword = false;
            foreach (string parameter in parameters.AllKeys)
            {
                var key = Regex.Replace(parameter, @"[^a-zA-Z0-9]", "");
                var value = parameters[parameter];
                validUsername |= ProcessKeyValue(key, value) == ValidationResponse.ValidUsername;
                validPassword |= ProcessKeyValue(key, value) == ValidationResponse.ValidPassword;
            }

            return validUsername && validPassword;
        }

        static ValidationResponse ProcessKeyValue(string key, string value) =>
            key.Equals(USERNAME_KEY, StringComparison.OrdinalIgnoreCase)
                ? ValidateUsername(value)
                : (key.Equals(PASSWORD_KEY, StringComparison.OrdinalIgnoreCase)
                    ? ValidatePassword(value)
                    : ValidationResponse.None);

        static ValidationResponse ValidateUsername(string username) =>
            username.Equals(USER_NAME, StringComparison.OrdinalIgnoreCase)
                ? ValidationResponse.ValidUsername
                : ValidationResponse.InvalidUsernameOrPassword;

        static ValidationResponse ValidatePassword(string password)
        {
            var hashBytes = Convert.FromBase64String(USER_HASH);
            var salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000);
            var hash = pbkdf2.GetBytes(20);
            for (var i = 0; i < 20; i++)
                if (hashBytes[i + 16] != hash[i])
                    return ValidationResponse.InvalidUsernameOrPassword;
            return ValidationResponse.ValidPassword;
        }
    }
}