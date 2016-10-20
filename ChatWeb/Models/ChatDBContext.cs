using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ChatWeb.Models
{
    public class ChatDBContext : DbContext
    {
    
        public DbSet<ChatUser> ChatUsers { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Message> Messages { get; set; }
       
        public ChatDBContext() 
        {

        }
    }
}