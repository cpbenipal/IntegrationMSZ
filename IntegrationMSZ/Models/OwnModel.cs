using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace IntegrationMSZ.Models
{
    public class Signature
    {
        public string SignatureText { get; set; }
    }

    public class ApplicationModel
    {
        [DisplayName("Topic")]
        public string topic { get; set; }
        [DisplayName("Topic")]
        public int type { get; set; }
        [DisplayName("Start Datetime")]
        public DateTime start_time { get; set; }
        [DisplayName("Duration")]
        public int duration { get; set; }
        [DisplayName("Duration")]
        public Settings settings { get; set; }
    }

    public class ScheduleMeetingModel
    {
        public string id { get; set; }
        public string password { get; set; }
        public string topic { get; set; }
        public string start_time { get; set; }
        public string duration { get; set; }
        public string join_url { get; set; } 
    }
    public class Settings
    {
        public bool host_video { get; set; }
        public bool participant_video { get; set; }
        public bool cn_meeting { get; set; }
        public bool in_meeting { get; set; }
        public bool join_before_host { get; set; }
        public bool mute_upon_entry { get; set; } = false;
        public string audio { get; set; }
        public bool registrants_email_notification { get; set; }

    }
}