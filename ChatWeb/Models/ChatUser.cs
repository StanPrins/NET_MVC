using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ChatWeb.Models
{
    public class ChatUser
    {
        public int ID { get; set; }
        public string user_id { get; set; }
        public string user_name { get; set; }
        public string pwd { get; set; }
        public string user_email { get; set; }
        public bool user_type { get; set; }
        public long phone_number { get; set; }
        public bool online_state { get; set; }
        
        public long reg_date { get; set; }
        public string contact_list { get; set; }
        public string room_list { get; set; }

        public string auth_token { get; set; }
        public long last_logdate { get; set; }

        public string device_id { get; set; }
        

    }

  
}