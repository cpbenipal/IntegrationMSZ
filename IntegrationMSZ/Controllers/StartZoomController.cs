using IntegrationMSZ.Models;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IntegrationMSZ.Controllers
{
    public class StartZoomController : Controller
    {
        protected string ApiKey = System.Configuration.ConfigurationManager.AppSettings["apiKey"];
        protected string ApiSecret = System.Configuration.ConfigurationManager.AppSettings["apiSecret"];
        protected string JwtKey = System.Configuration.ConfigurationManager.AppSettings["jwtKey"];
        protected string JwtSecret = System.Configuration.ConfigurationManager.AppSettings["jwtSecret"];
        protected string Redirect_uri = System.Configuration.ConfigurationManager.AppSettings["redirect_uri"];
        protected string SiteUrl = System.Configuration.ConfigurationManager.AppSettings["site_url"];
        protected string UserName = System.Configuration.ConfigurationManager.AppSettings["user_name"];

        public ActionResult Index()
        {
            var ApplicationModel = new ApplicationModel();
            ApplicationModel.type = 2;
            return View(ApplicationModel);
        }
         
        /// <summary>
        /// Create and Join meeting: 
        /// </summary>
        /// <param name="application"></param>        
        /// <returns>Meeting detail</returns>
        [HttpPost]
        public ActionResult CreateaMeeting(ApplicationModel application)
        {
            application.settings = new Settings()
            {
                host_video = true,
                participant_video = true,
                join_before_host = false,
                mute_upon_entry = false,
                audio = "both",
                registrants_email_notification = false
            };

            string uri = string.Format("https://api.zoom.us");
            var client = new RestClient(uri);
            var request = new RestRequest("v2/users/me/meetings", Method.Post);

            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { topic = application.topic, duration = application.duration, start_time = application.start_time, type = application.type });
            //request.AddJsonBody(new { topic = "Meeting with Ussain", duration = "10", start_time = DateTime.Now.AddHours(1), type = "2" });
            request.AddHeader("authorization", String.Format("Bearer {0}", GenerateAccessToken()));
            var restResponse = Task.Run(() => client.ExecuteAsync(request)).Result;

            var response = Newtonsoft.Json.JsonConvert.DeserializeObject<ScheduleMeetingModel>(restResponse.Content);
            
            return RedirectToAction("ScheduleMeeting",response);
        }
        public ActionResult ScheduleMeeting(ScheduleMeetingModel meeting) 
        {
            return View(meeting);
        }
        /// <summary>
        /// Generate Signature for base64Salth before start meeting.
        /// Role : 1 -> (Teacher) Create meeting , 0 -> (Student) Join meeting
        /// </summary>
        /// <param name="meetingNumber"></param>
        /// <param name="role"></param>
        /// <returns>Signature</returns>
        [HttpGet]
        public ActionResult GenerateSignature(string meetingNumber, string role)
        {

            char[] padding = { '=' };
            string apiKey = JwtKey;
            string apiSecret = JwtSecret;
            String ts = (ToTimestamp(DateTime.UtcNow.ToUniversalTime()) - 30000).ToString();

            string message = String.Format("{0}{1}{2}{3}", apiKey, meetingNumber, ts, role);
            apiSecret = apiSecret ?? "";
            var encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(apiSecret);
            byte[] messageBytesTest = encoding.GetBytes(message);
            string msgHashPreHmac = System.Convert.ToBase64String(messageBytesTest);
            byte[] messageBytes = encoding.GetBytes(msgHashPreHmac);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                string msgHash = System.Convert.ToBase64String(hashmessage);
                string token = String.Format("{0}.{1}.{2}.{3}.{4}", apiKey, meetingNumber, ts, role, msgHash);
                var tokenBytes = System.Text.Encoding.UTF8.GetBytes(token);
                var sig = new Signature()
                {
                    SignatureText = System.Convert.ToBase64String(tokenBytes).TrimEnd(padding)
                };
                return this.Json(sig.SignatureText, JsonRequestBehavior.AllowGet);
            }           
        }
        long ToTimestamp(DateTime value)
        {
            long epoch = (value.Ticks - 621355968000000000) / 10000;
            return epoch;
        }
        private object GenerateAccessToken()
        {
            var signinkey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSecret));

            var claims = new List<Claim> { new Claim("username", UserName) };
            claims.Add(new Claim("userid", UserName));
            //claims.Add(new Claim("tenantid", _configuration["Tenant:TenantId"]));

            var token = new JwtSecurityToken(
                issuer: JwtKey,
                audience: SiteUrl,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(90),
                signingCredentials: new SigningCredentials(signinkey, SecurityAlgorithms.HmacSha256)
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

      
    }
}