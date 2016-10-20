using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatWeb.Models
{
    public class Message
    {
        public int ID { get; set; }
        public string message_id { get; set; }
        public string room_id { get; set; }
        public string sender_id { get; set; }
        public string receiver_id { get; set; }
        public long sender_date { get; set; }
        public long receiver_date { get; set; }
        public string context { get; set; }
        public bool read { get; set; }

    }
}