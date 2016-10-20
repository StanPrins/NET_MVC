using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ChatWeb.Models
{
    public class Room
    {
        public int ID { get; set; }
        public string room_id { get; set; }
        public string room_name { get; set; }
        public string owner_id { get; set; }
        public string member_list { get; set; }
        public long make_date { get; set; }

    }
   

}