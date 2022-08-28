using System;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Ingenico;
using SuperWebSocket;
using Newtonsoft.Json;

namespace WINTSI.WepSocket
{
    public static class Server
    {
        const string USER_NAME = "ptadmin";
        const string USER_HASH = "ZrK0AOQz+L1wjr2jNXZwMcgosUOeicIifZ+8fOWDur2TkV7m";
        const string USERNAME_KEY = "username";
        const string PASSWORD_KEY = "password";
        private static int _port = 8088;
        const float TOLERANCE = 0.01f;
        const bool DEBUG = true;
        private static WebSocketServer wsServer;

        private enum ValidationResponse
        {
            None = 0,
            InvalidUsernameOrPassword = 1,
            ValidUsername = 2,
            ValidPassword = 3
        }

        public static void StartServer(Communication communication)
        {
            _port = communication.IpPort;
            wsServer = new WebSocketServer();
            wsServer.Setup(_port);
            wsServer.NewSessionConnected += WsServer_NewSessionConnected;
            wsServer.NewMessageReceived += WsServer_NewMessageReceived;
            wsServer.NewDataReceived += WsServer_NewDataReceived;
            wsServer.SessionClosed += WsServer_SessionClosed;
            wsServer.Start();
            Console.WriteLine($"Server is running on port {_port}.");
        }

        private static void WsServer_NewSessionConnected(WebSocketSession session)
        {
            if (!ValidateSession(session)) return;
            Console.WriteLine("[200] Client connected successfully.");

        }

        private static void WsServer_NewMessageReceived(WebSocketSession session, string value)
        {
            if (!ValidateSession(session)) return;
            try
            {
                var paymentRequest = JsonConvert.DeserializeObject<PaymentRequest>(value);
                Console.WriteLine("[202] Payment request received.");
                if (!ValidatePaymentRequest(paymentRequest))
                {
                    Console.WriteLine("[400] Bad payment request.");
                    session.Send("Bad payment request.");
                    return;
                }

                //TBD: Process payment here
                Program.CreateRequest(paymentRequest.totalPrice);
                //TBD: Build actual <see cref="PaymentResponse"/> data here:
                var response = new PaymentResponse(PaymentResponse.PaymentStatus.SUCCESS, paymentRequest.totalPrice,
                    "**** **** 4942", "SOME_REFEENCE");
                var jsonResponse = JsonConvert.SerializeObject(response, Formatting.Indented);
                Console.WriteLine("[201] Payment successful.");
                session.Send(jsonResponse);
                session.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[500] Internal server error.");
                session.Send("Internal server error.");
                if (DEBUG)
                {
                    Console.WriteLine(ex.Message);
                    session.Send(ex.Message);
                }
            }
        }

        private static void WsServer_NewDataReceived(WebSocketSession session, byte[] value)
        {
            if (!ValidateSession(session)) return;
            //For use if needed.
        }

        private static void
            WsServer_SessionClosed(WebSocketSession session, SuperSocket.SocketBase.CloseReason value) =>
            Console.WriteLine("Client disconnected.");

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