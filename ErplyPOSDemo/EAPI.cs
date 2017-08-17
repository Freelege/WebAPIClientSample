using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography.X509Certificates;

namespace erplyAPI
{
    /*
     * Custom exception for errorCodes
     * */
    public class EAPIException : Exception
    {
        public int errorCode;
        public  EAPIException(string message,int errorCode): base(message) 
        {
            this.errorCode = errorCode;
        }
    }

    public class EAPI
    {

        public const int VERIFY_USER_FAILURE = 2001;
	    public const int MISSING_PARAMETERS = 2004;
        public const int WEBREQUEST_ERROR = 2002;
        public string url;
        public string clientCode;
        public string username;
        public string password;
        public string sslCACertPath;
        private string EAPISessionKey;
        private int EAPISessionKeyExpires;


        /*Initialize class and set values if specified
         * Args: url, clientCode, username, password, sslCACerthPath (All optional)
         * */
        public EAPI(string url = null, string clientCode = null, 
            string username = null, string password = null, string sslCACertPath = null)
        {
            this.url = url;
            this.clientCode = clientCode;
            this.username = username;
            this.password = password;
            this.sslCACertPath = sslCACertPath;

        }
        /*Send request and return output data
         * Args: request -> reqest type
         *       parameters -> POST parameters
         * Returns: output string
         * */
        public JObject sendRequest(string request, Dictionary<string, object> parameters = null)
        {
            if (parameters == null) parameters = new Dictionary<string, object>();
            if (this.clientCode == null || this.url == null ||
                this.username == null ||this.password == null)
            {
                throw new EAPIException("Missing parameters", 2004);
            }
            // Add extra parameters
            parameters.Add("request", request);
            parameters.Add("clientCode", this.clientCode);
            parameters.Add("version", "1.0");
            if (request != "verifyUser") parameters.Add("sessionKey",this.getSessionKey());

            // Create web request and and post data
            try
            {
                WebRequest wrequest = WebRequest.Create(this.url);
                wrequest.Method = "POST";
                string postData = createQueryString(parameters);
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                wrequest.ContentType = "application/x-www-form-urlencoded";
                wrequest.ContentLength = byteArray.Length;

                if (this.sslCACertPath != null)
                {
                    X509Certificate cert = X509Certificate.CreateFromCertFile(this.sslCACertPath);
                }

                Stream dataStream = wrequest.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                WebResponse response = wrequest.GetResponse();
                dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                JObject json = JObject.Parse(responseFromServer);
                reader.Close();
                dataStream.Close();
                response.Close();
                return json;
            }
            catch (Exception e)
            {
                throw new EAPIException("Webrequest error: " + e.ToString() , 2002);
            }
        }


        public String getSessionKey()
        {   // If key doesnt exist or expired
            if (EAPISessionKey == null || EAPISessionKeyExpires == 0 || EAPISessionKeyExpires < time())
            {
                JObject result = this.sendRequest("verifyUser", new Dictionary<string, object>()
                    {
                    {"username", this.username},
                    {"password", this.password}
                    });
                // Failure check
                if ((int) result["status"]["errorCode"] != 0)
                {
                     this.EAPISessionKey = null;
                     this.EAPISessionKeyExpires = 0;
                     throw new EAPIException("Verify user failure", 2001);
                }
                this.EAPISessionKey = (String) result["records"][0]["sessionKey"];
                this.EAPISessionKeyExpires = time() + (int)result["records"][0]["sessionLength"] - 30;
            }
            return this.EAPISessionKey;


        }
        /* The function that makes querystring out of dictionary
         * Args: parameters
         * */
        public string createQueryString(Dictionary<string, object> parameters)
        {
            var stringBuilder = new StringBuilder();
            foreach(KeyValuePair<string,object> entry in parameters) {
                stringBuilder.Append(entry.Key + "=" + entry.Value + "&");
            }
            stringBuilder.Length -= 1; // Remove the last 
            return stringBuilder.ToString();
        }
        /*
         *  Return time since Unix epoch
         * */
        public int time()
        {
            return (int)DateTime.Now.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
        }
    }
}