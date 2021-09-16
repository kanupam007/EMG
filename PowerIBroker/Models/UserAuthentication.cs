
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Newtonsoft.Json;
using PowerIBrokerDataLayer;
using PowerIBrokerBusinessLayer.Employee;

namespace PowerIBroker.Models
{
    public class UserAuthentication
    {
        public string CultureName { get; set; }
        public string UserType { get; set; }
        [Required(ErrorMessage = "Please provide your First Name")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please provide your First Name")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
        public bool Success { get; set; }
        public string DeviceTokenId { get; set; }
        public string DeviceType { get; set; }
        public string MessageType { get; set; }
        public List<string> ErrorMessage { get; set; }

    }
    public class AndroidFCMPushNotificationStatus
    {
        public bool Successful
        {
            get;
            set;
        }

        public string Response
        {
            get;
            set;
        }
        public Exception Error
        {
            get;
            set;
        }
    }
    public class EMGPlanBeneficiary
    {
        public long ID { get; set; }
        public Nullable<long> DependentId { get; set; }
        public Nullable<decimal> Allocation { get; set; }
        public Nullable<long> EmployeeId { get; set; }
        public Nullable<long> OpenEnrollId { get; set; }
        public Nullable<long> PlanId { get; set; }
        public Nullable<long> CatId { get; set; }
        public string Type { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string Dependent { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DOB { get; set; }
        public string SSN { get; set; }
        public string Gender { get; set; }
        public string MiddleName { get; set; }
        public string NameTitle { get; set; }
        public string IsBeneficiary { get; set; }
    }
    public class Status
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? SuccessMessage { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? MessageStatus { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? ISEOISkip { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? IsWaived { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Paypercost { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? PlanCount { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public dynamic DataList { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string StartDate { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message1 { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? MessageStatus1 { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public dynamic DataList1 { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message2 { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? MessageStatus2 { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public dynamic DataList2 { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message3 { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? MessageStatus3 { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public dynamic DataList3 { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message4 { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? MessageStatus4 { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public dynamic DataList4 { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message5 { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? MessageStatus5 { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public dynamic DataList5 { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? TotalCount { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CompanyName { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ACAUrl { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string BenefitUrl { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string EOiFileName { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string EOISpouseFileName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string EOIDesc { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string EOIDownloadFileName { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string EOIUrlDesc { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string EOIUrlLink { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsEOIUpload { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsEOIUrl { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public long Id { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal TotalSum { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? MonthlyBasisID { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int PlanStatus { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool IsSpouseAvail { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<Proc_GetPlanDetails_Result> PlanDetails { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<CostDetails> PlanCostDetails { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<psp_GetInsPlanResourse_Result> PlanResourse { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<psp_GetPlanDetailsByPlanDetails_Result> PlanInfo { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<Proc_GetPlanDetails_Result> PropertyDetails { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<Proc_GetPlanDetails_Result> NetworkDetails { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<psp_GetPlanNameByPlanID_Result> Plans { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<tblEmployee_Benefits> GetDependents { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<DependentsViewBenefits> BenefitDependents { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<EmploymentDetails> EmploymentDetails { get; set; }


    }
    public class EmploymentDetails
    {
        public long ID { get; set; }
        public Nullable<long> EmployeeID { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public string Type { get; set; }
        public Nullable<int> EmployeeType { get; set; }
        public Nullable<long> Salary { get; set; }
        public string Status { get; set; }
        public string SSN { get; set; }
        public long Bonus { get; set; }
        public long Commisiion { get; set; }
        public Nullable<long> MonthlyBasisID { get; set; }
        public Nullable<long> DivisionID { get; set; }
    }
    public class DependentsViewBenefits
    {
        public string Category { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<psp_GetVieBenefitsDependentsByEmployeeID_Result> Dependents { get; set; }
    }
    public class NotificationPayload
    {
        public string Alert { get; set; }
        public string DeviceToken { get; set; }
        public int? Badge { get; set; }
        public string Sound { get; set; }
        //public string CustomeData { get; set; }
        public string TypeID { get; set; }
        internal int PayloadId { get; set; }
        //public  string UserInfo{get; set;}
        //public string DateLocation { get; set;}

        //public Dictionary<string, object[]> CustomItems
        //{
        //    get;
        //    private set;
        //}

        //public Dictionary<string, dynamic> CustomItemsAlert
        //{
        //    get;
        //    private set;
        //}

        //public NotificationPayload(string deviceToken)
        //{
        //    DeviceToken = deviceToken;
        //    Alert = new NotificationAlert();
        //    CustomItems = new Dictionary<string, object[]>();
        //}

        //public NotificationPayload(string deviceToken, string alert)
        //{
        //    DeviceToken = deviceToken;
        //    Alert = new NotificationAlert() { Body = alert };
        //    CustomItems = new Dictionary<string, object[]>();
        //}

        //public NotificationPayload(string deviceToken, string alert, int badge)
        //{
        //    DeviceToken = deviceToken;
        //    Alert = new NotificationAlert() { Body = alert };
        //    Badge = badge;
        //    CustomItems = new Dictionary<string, object[]>();
        //}

        //public NotificationPayload(string deviceToken, string alert, int badge, string sound)
        //{
        //    DeviceToken = deviceToken;
        //    Alert = new NotificationAlert() { Body = alert };
        //    Badge = badge;
        //    Sound = sound;
        //    CustomItems = new Dictionary<string, object[]>();
        //}

        public NotificationPayload(string deviceToken, string alert, int badge, string sound, string Type)
        {
            DeviceToken = deviceToken;
            Alert = alert;
            Badge = badge;
            Sound = sound;
            TypeID = Type;
        }


        //public NotificationPayload(string deviceToken, string alert, int badge, string sound, int contentAvailable)
        //{
        //    DeviceToken = deviceToken;
        //    Alert = new NotificationAlert() { Body = alert };
        //    Badge = badge;
        //    Sound = sound;
        //    content_available = contentAvailable;
        //    CustomItemsAlert = new Dictionary<string, dynamic>();
        //}

        //public void AddCustom(string key, dynamic values)
        //{
        //    if (values != null)
        //        this.CustomItemsAlert.Add(key, values);
        //}

        //public string ToJson()
        //{
        //    JObject json = new JObject();
        //    JObject aps = new JObject();
        //    if (!this.Alert.IsEmpty)
        //    {
        //        if (!string.IsNullOrEmpty(this.Alert.Body)
        //            && string.IsNullOrEmpty(this.Alert.LocalizedKey)
        //            && string.IsNullOrEmpty(this.Alert.ActionLocalizedKey)
        //            && (this.Alert.LocalizedArgs == null || this.Alert.LocalizedArgs.Count <= 0))
        //        {
        //            aps["alert"] = new JValue(this.Alert.Body);
        //        }
        //        else
        //        {
        //            JObject jsonAlert = new JObject();
        //            if (!string.IsNullOrEmpty(this.Alert.LocalizedKey))
        //                jsonAlert["loc-key"] = new JValue(this.Alert.LocalizedKey);

        //            if (this.Alert.LocalizedArgs != null && this.Alert.LocalizedArgs.Count > 0)
        //                jsonAlert["loc-args"] = new JArray(this.Alert.LocalizedArgs.ToArray());

        //            if (!string.IsNullOrEmpty(this.Alert.Body))
        //                jsonAlert["body"] = new JValue(this.Alert.Body);

        //            if (!string.IsNullOrEmpty(this.Alert.ActionLocalizedKey))
        //                jsonAlert["action-loc-key"] = new JValue(this.Alert.ActionLocalizedKey);

        //            aps["alert"] = jsonAlert;
        //        }
        //    }

        //    if (this.Badge.HasValue)
        //        aps["badge"] = new JValue(this.Badge.Value);

        //    if (!string.IsNullOrEmpty(this.Sound))
        //        aps["sound"] = new JValue(this.Sound);
        //    if (!string.IsNullOrEmpty(this.content_available.ToString()))
        //        aps["content-available"] = new JValue(this.content_available.ToString());
        //    aps["message"] = new JValue("");
        //    json["aps"] = aps;
        //    foreach (string key in this.CustomItemsAlert.Keys)
        //    {
        //        json[key] = new JArray(this.CustomItemsAlert[key]);
        //    }
        //    string rawString = json.ToString(Newtonsoft.Json.Formatting.None, null);
        //    StringBuilder encodedString = new StringBuilder();
        //    foreach (char c in rawString)
        //    {
        //        if ((int)c < 32 || (int)c > 127)
        //            encodedString.Append("\\u" + String.Format("{0:x4}", Convert.ToUInt32(c)));
        //        else
        //            encodedString.Append(c);
        //    }
        //    return rawString;// encodedString.ToString();
        //}

        //public override string ToString()
        //{
        //    return ToJson();
        //}
    }
    //public class NotificationAlert
    //{
    //    /// <summary>
    //    /// Constructor
    //    /// </summary>
    //    public NotificationAlert()
    //    {
    //        Body = null;
    //        ActionLocalizedKey = null;
    //        LocalizedKey = null;
    //        LocalizedArgs = new List<object>();
    //    }

    //    /// <summary>
    //    /// Body Text of the Notification's Alert
    //    /// </summary>
    //    public string Body
    //    {
    //        get;
    //        set;
    //    }

    //    /// <summary>
    //    /// Action Button's Localized Key
    //    /// </summary>
    //    public string ActionLocalizedKey
    //    {
    //        get;
    //        set;
    //    }

    //    /// <summary>
    //    /// Localized Key
    //    /// </summary>
    //    public string LocalizedKey
    //    {
    //        get;
    //        set;
    //    }

    //    /// <summary>
    //    /// Localized Argument List
    //    /// </summary>
    //    public List<object> LocalizedArgs
    //    {
    //        get;
    //        set;
    //    }

    //    public void AddLocalizedArgs(params object[] values)
    //    {
    //        this.LocalizedArgs.AddRange(values);
    //    }

    //    /// <summary>
    //    /// Determines if the Alert is empty and should be excluded from the Notification Payload
    //    /// </summary>
    //    public bool IsEmpty
    //    {
    //        get
    //        {
    //            if (!string.IsNullOrEmpty(Body)
    //                || !string.IsNullOrEmpty(ActionLocalizedKey)
    //                || !string.IsNullOrEmpty(LocalizedKey)
    //                || (LocalizedArgs != null && LocalizedArgs.Count > 0))
    //                return false;
    //            else
    //                return true;
    //        }
    //    }
    //}
    public class ApplePushNotification
    {

        private TcpClient _apnsClient;
        private SslStream _apnsStream;
        private X509Certificate _certificate;
        private X509CertificateCollection _certificates;

        public string P12File { get; set; }
        public string P12FilePassword { get; set; }


        // Default configurations for APNS
        private const string ProductionHost = "gateway.push.apple.com";
        private const string SandboxHost = "gateway.push.apple.com";
        private const int NotificationPort = 2195;

        // Default configurations for Feedback Service
        private const string ProductionFeedbackHost = "gateway.push.apple.com";
        private const string SandboxFeedbackHost = "gateway.push.apple.com";
        private const int FeedbackPort = 2195;


        private bool _conected = false;

        private readonly string _host;
        private readonly string _feedbackHost;

        private List<NotificationPayload> _notifications = new List<NotificationPayload>();
        private List<string> _rejected = new List<string>();

        private Dictionary<int, string> _errorList = new Dictionary<int, string>();

        public ApplePushNotification(bool useSandbox, string p12File, string p12FilePassword)
        {
            if (useSandbox)
            {
                _host = SandboxHost;
                _feedbackHost = SandboxFeedbackHost;
            }
            else
            {
                _host = ProductionHost;
                _feedbackHost = ProductionFeedbackHost;
            }

            //Load Certificates in to collection.
            //_certificate = string.IsNullOrEmpty(p12FilePassword)? new X509Certificate2(File.ReadAllBytes(p12File)): new X509Certificate2(File.ReadAllBytes(p12File), p12FilePassword);

            _certificate = string.IsNullOrEmpty(p12FilePassword) ? new X509Certificate2(File.ReadAllBytes(p12File)) : new X509Certificate2(File.ReadAllBytes(p12File), p12FilePassword, X509KeyStorageFlags.MachineKeySet);
            _certificates = new X509CertificateCollection { _certificate };

            // Loading Apple error response list.
            _errorList.Add(0, "No errors encountered");
            _errorList.Add(1, "Processing error");
            _errorList.Add(2, "Missing device token");
            _errorList.Add(3, "Missing topic");
            _errorList.Add(4, "Missing payload");
            _errorList.Add(5, "Invalid token size");
            _errorList.Add(6, "Invalid topic size");
            _errorList.Add(7, "Invalid payload size");
            _errorList.Add(8, "Invalid token");
            _errorList.Add(255, "None (unknown)");
        }

        public List<string> SendToApple(List<NotificationPayload> queue)
        {
            _notifications = queue;
            if (queue.Count < 8999)
            {
                SendQueueToapple(_notifications);
            }
            else
            {
                const int pageSize = 8999;
                int numberOfPages = (queue.Count / pageSize) + (queue.Count % pageSize == 0 ? 0 : 1);
                int currentPage = 0;

                while (currentPage < numberOfPages)
                {
                    _notifications = (queue.Skip(currentPage * pageSize).Take(pageSize)).ToList();
                    SendQueueToapple(_notifications);
                    currentPage++;
                }
            }
            //Close the connection
            Disconnect();
            return _rejected;
        }

        private void SendQueueToapple(IEnumerable<NotificationPayload> queue)
        {
            int i = 1000;
            foreach (var item in queue)
            {
                if (!_conected)
                {
                    Connect(_host, NotificationPort, _certificates);
                    var response = new byte[6];
                    _apnsStream.BeginRead(response, 0, 6, ReadResponse, new MyAsyncInfo(response, _apnsStream));
                }
                try
                {
                    if (item.DeviceToken.Length == 64) //check lenght of device token, if its shorter or longer stop generating Payload.
                    {
                        item.PayloadId = i;
                        byte[] payload = GeneratePayload(item);
                        _apnsStream.Write(payload);

                        Thread.Sleep(1000); //Wait to get the response from apple.
                    }
                    else { }
                }
                catch (Exception ex)
                {
                    _conected = false;
                }
                i++;
            }
        }

        private void ReadResponse(IAsyncResult ar)
        {
            if (!_conected)
                return;
            string payLoadId = "";
            int payLoadIndex = 0;
            try
            {
                var info = ar.AsyncState as MyAsyncInfo;
                info.MyStream.ReadTimeout = 100;
                if (_apnsStream.CanRead)
                {
                    var command = Convert.ToInt16(info.ByteArray[0]);
                    var status = Convert.ToInt16(info.ByteArray[1]);
                    var ID = new byte[4];
                    Array.Copy(info.ByteArray, 2, ID, 0, 4);

                    payLoadId = Encoding.Default.GetString(ID);
                    payLoadIndex = ((int.Parse(payLoadId)) - 1000);

                    _rejected.Add(_notifications[payLoadIndex].DeviceToken);
                    _conected = false;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void Connect(string host, int port, X509CertificateCollection certificates)
        {
            try
            {
                _apnsClient = new TcpClient();
                _apnsClient.Connect(host, port);
            }
            catch (SocketException ex)
            {

            }

            var sslOpened = OpenSslStream(host, certificates);

            if (sslOpened)
            {
                _conected = true;

            }

        }

        private void Disconnect()
        {
            try
            {
                Thread.Sleep(500);
                _apnsClient.Close();
                _apnsStream.Close();
                _apnsStream.Dispose();
                _apnsStream = null;
                _conected = false;

            }
            catch (Exception ex)
            {

            }
        }

        private bool OpenSslStream(string host, X509CertificateCollection certificates)
        {

            _apnsStream = new SslStream(_apnsClient.GetStream(), false, validateServerCertificate, SelectLocalCertificate);

            try
            {
                _apnsStream.AuthenticateAsClient(host, certificates, System.Security.Authentication.SslProtocols.Tls, false);
            }
            catch (System.Security.Authentication.AuthenticationException ex)
            {

                return false;
            }

            if (!_apnsStream.IsMutuallyAuthenticated)
            {

                return false;
            }

            if (!_apnsStream.CanWrite)
            {

                return false;
            }
            return true;
        }

        private bool validateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true; // Dont care about server's cert
        }

        private X509Certificate SelectLocalCertificate(object sender, string targetHost, X509CertificateCollection localCertificates, X509Certificate remoteCertificate, string[] acceptableIssuers)
        {
            return _certificate;
        }

        private static byte[] GeneratePayload(NotificationPayload payload)
        {
            try
            {
                //convert Devide token to HEX value.
                byte[] deviceToken = new byte[payload.DeviceToken.Length / 2];
                for (int i = 0; i < deviceToken.Length; i++)
                    deviceToken[i] = byte.Parse(payload.DeviceToken.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
                var memoryStream = new MemoryStream();
                // Command
                memoryStream.WriteByte(1); // Changed command Type 
                //Adding ID to Payload
                memoryStream.Write(Encoding.ASCII.GetBytes(payload.PayloadId.ToString()), 0, payload.PayloadId.ToString().Length);
                //Adding ExpiryDate to Payload
                int epoch = (int)(DateTime.UtcNow.AddMinutes(300) - new DateTime(1970, 1, 1)).TotalSeconds;
                byte[] timeStamp = BitConverter.GetBytes(epoch);
                memoryStream.Write(timeStamp, 0, timeStamp.Length);
                byte[] tokenLength = BitConverter.GetBytes((Int16)32);
                Array.Reverse(tokenLength);
                // device token length
                memoryStream.Write(tokenLength, 0, 2);
                // Token
                memoryStream.Write(deviceToken, 0, 32);
                // String length
                //  string apnMessage = string.Empty;
                //if (payload.TypeID == "1")
                //{
                //    apnMessage = "{\"aps\":{\"badge\":1,\"sound\":\"Beep.mp3\",\"content-available\":\"1\"},\"DateID\": \"" + payload.CustomeData + "\",\"Type\": \"" + payload.TypeID + "\",\"UserInformation\": \"" + payload.UserInfo + "\",\"DateLocation\": \"" + payload.DateLocation + "\"}"; // payload.ToJson();
                //}
                //else
                //{
                //   apnMessage = "{\"aps\":{\"alert\":\"" + payload.Alert.Body + "\",\"badge\":1,\"sound\":\"Beep.mp3\"},\"DateID\": \"" + payload.CustomeData + "\",\"Type\": \"" + payload.TypeID + "\"}"; // payload.ToJson();
                // }
                string apnMessage = "{\"aps\":{\"alert\":\"" + payload.Alert + "\",\"badge\":1,\"sound\":\"Beep.mp3\"},\"Type\": \"" + payload.TypeID + "\"}"; // payload.ToJson(); 

                byte[] apnMessageLength = BitConverter.GetBytes((Int16)apnMessage.Length);
                Array.Reverse(apnMessageLength);
                // message length
                memoryStream.Write(apnMessageLength, 0, 2);
                // Write the message
                memoryStream.Write(Encoding.ASCII.GetBytes(apnMessage), 0, apnMessage.Length);
                return memoryStream.ToArray();
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        public List<Feedback> GetFeedBack()
        {
            try
            {
                var feedbacks = new List<Feedback>();


                if (!_conected)
                    Connect(_feedbackHost, FeedbackPort, _certificates);

                if (_conected)
                {
                    //Set up
                    byte[] buffer = new byte[38];
                    int recd = 0;
                    DateTime minTimestamp = DateTime.UtcNow.AddYears(-1);

                    //Get the first feedback
                    recd = _apnsStream.Read(buffer, 0, buffer.Length);


                    if (recd == 0)


                        //Continue while we have results and are not disposing
                        while (recd > 0)
                        {

                            var fb = new Feedback();

                            //Get our seconds since 1970 ?
                            byte[] bSeconds = new byte[4];
                            byte[] bDeviceToken = new byte[32];

                            Array.Copy(buffer, 0, bSeconds, 0, 4);

                            //Check endianness
                            if (BitConverter.IsLittleEndian)
                                Array.Reverse(bSeconds);

                            int tSeconds = BitConverter.ToInt32(bSeconds, 0);

                            //Add seconds since 1970 to that date, in UTC and then get it locally
                            fb.Timestamp = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(tSeconds).ToLocalTime();


                            //Now copy out the device token
                            Array.Copy(buffer, 6, bDeviceToken, 0, 32);

                            fb.DeviceToken = BitConverter.ToString(bDeviceToken).Replace("-", "").ToLower().Trim();

                            //Make sure we have a good feedback tuple
                            if (fb.DeviceToken.Length == 64 && fb.Timestamp > minTimestamp)
                            {
                                //Raise event
                                //this.Feedback(this, fb);
                                feedbacks.Add(fb);
                            }

                            //Clear our array to reuse it
                            Array.Clear(buffer, 0, buffer.Length);

                            //Read the next feedback
                            recd = _apnsStream.Read(buffer, 0, buffer.Length);
                        }
                    //clode the connection here !
                    Disconnect();
                    if (feedbacks.Count > 0)

                        return feedbacks;
                }
            }
            catch (Exception ex)
            {

                return null;
            }
            return null;
        }
    }
    public class Feedback
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Feedback()
        {
            this.DeviceToken = string.Empty;
            this.Timestamp = DateTime.MinValue;
        }

        /// <summary>
        /// Device Token string in hex form without any spaces or dashes
        /// </summary>
        public string DeviceToken
        {
            get;
            set;
        }

        /// <summary>
        /// Timestamp of the Feedback for when Apple received the notice to stop sending notifications to the device
        /// </summary>
        public DateTime Timestamp
        {
            get;
            set;
        }
    }
    public class MyAsyncInfo
    {
        public Byte[] ByteArray { get; set; }
        public SslStream MyStream { get; set; }

        public MyAsyncInfo(Byte[] array, SslStream stream)
        {
            ByteArray = array;
            MyStream = stream;
        }
    }
}