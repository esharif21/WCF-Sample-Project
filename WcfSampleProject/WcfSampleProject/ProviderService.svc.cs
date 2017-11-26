using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using WcfSampleProject.Models;

namespace WcfSampleProject
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ProviderService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select ProviderService.svc or ProviderService.svc.cs at the Solution Explorer and start debugging.
    public class ProviderService : IProviderService
    {
        public string DoSomeTasks(SampleModel data)
        {
            try
            {
                var result = Authenticate();
                // other staff in business logic
                return "{\"IsSuccess\" : true, \"Data\" : " + data.Field1 + " " + data.Field2 + "}";
            }
            catch (Exception ex)
            {
                return "{\"IsSuccess\" : false, \"Data\" : \"" + ex.Message + "\"}";
            }
        }


        private Tuple<long, string> Authenticate()
        {
            if (!HttpContext.Current.Request.Headers.AllKeys.Contains("Authorization"))
            {
                throw new Exception("Authentication Failed. Authorization must be provided");
            }

            if (!HttpContext.Current.Request.Headers.AllKeys.Contains("X-PW-TOKEN"))
            {
                throw new Exception("Authentication Failed. Token must be provided");
            }

            var auth = HttpContext.Current.Request.Headers["Authorization"];
            var token = HttpContext.Current.Request.Headers["X-PW-TOKEN"].Trim();
            var url = HttpContext.Current.Request.Url.ToString();
            var match = Regex.Match(auth ?? string.Empty, "Basic\\s*(.*)?;(.*)?"); // Basic AjkerDeal;2CH7

            if (string.IsNullOrWhiteSpace(token))
            {
                throw new Exception("Authentication Failed. Token must be provided");
            }

            if (!match.Success)
            {
                throw new Exception("Authentication Failed. Request format is invalid");
            }

            var result = AuthenticateHMAC(match.Groups[1].Value, url, match.Groups[2].Value, token);

            return result;
        }

        private Tuple<long, string> AuthenticateHMAC(string username, string url, string randomString, string token)
        {
            // find user from db by username
            var user = new { Id = 123456, UserName = "PayWell", Status = "Active", Password = "PayWell@123456", LastLoginToken = "" }; // let this is the matched user
            if (user == null)
            {
                throw new Exception("Authentication Failed. Invalid username or token.");
            }

            if (user.Status != "Active")
            {
                throw new Exception("AuthenticationFailed. User not allowed for login.");
            }

            var tokenString = $"{url.ToLower().Trim()};{user.Password};{username.Trim()};{randomString.Trim()}";

            SHA256 hasher = new SHA256CryptoServiceProvider();
            var hash = hasher.ComputeHash(Encoding.UTF8.GetBytes(tokenString));
            var base64 = Convert.ToBase64String(hash);

            if (!base64.Equals(token, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception("AuthenticationFailed. User's provided token is not valid.");
            }

            string lastLoginGuid = Guid.NewGuid().ToString();
            /* user.LastLoginToken = lastLoginGuid;
            //  update last login token in db.
            */

            return new Tuple<long, string>(user.Id, token);
        }
    }
}
