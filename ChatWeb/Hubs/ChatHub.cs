using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ChatWeb.Models;

namespace ChatWeb.Hubs
{
    [HubName("ChatHub")]
    public class ChatHub:Hub
    {
        //private readonly static ConnectionMapping<string> _connections = new ConnectionMapping<string>();

        private ChatDBContext db = new ChatDBContext();
        private static List<User> Users = new List<User>();
        public class User
        {
            public string User_ID { get; set; }
            public string ConnectionID { get; set; }
            public List<String> InRooms { get; set; }
        }
        
        public class InRoom
        {
            public string Room_ID { get; set; }
            
        }

        public override Task OnConnected()
        {
            var user_id = Context.QueryString["user_id"];
            
            var result = Users.SingleOrDefault(u => u.User_ID == user_id && u.ConnectionID == Context.ConnectionId);
            if(result ==null)
            {
                User user = new User() { User_ID = user_id, ConnectionID = Context.ConnectionId };
                Users.Add(user);
                var r = db.ChatUsers.SingleOrDefault(c => c.user_id ==user_id);
                r.online_state = true;
                Clients.Group("PUBLIC_ROOM").GetMessage(user_id, "LOG_IN", "PUBLIC_ROOM", "");
                db.SaveChanges();
            }
            
            //Clients.Others.userConnected(user.User_ID);
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {

            var r = Users.Where(u => u.ConnectionID == Context.ConnectionId);
            if (r != null)
            {
                var uid = "";
                
                foreach (User user in r)
                {
                    uid = user.User_ID;
                    Clients.Group("PUBLIC_ROOM").GetMessage(uid, "LOG_OUT", "PUBLIC_ROOM", "");
                }
                Users.RemoveAll(k => k.User_ID == uid);
                var result = db.ChatUsers.SingleOrDefault(o => o.user_id == uid);
                result.online_state = false;
                db.SaveChanges();
                
            }


            return base.OnDisconnected(stopCalled);
        }
        public bool GetOnlineState(string receiverid)
        {
            var result = Users.SingleOrDefault(u => u.User_ID == receiverid && u.ConnectionID == Context.ConnectionId);
            if (result == null)
                return false;
            return true;
        }
        public void SendMessage(string sendername, string message, string roomName, string strdate)
        {
          
            
            Message msg = new Message();
            msg.message_id = Guid.NewGuid().ToString();
            msg.room_id = roomName;
            
            string[] str_member_list = roomName.Split('&');
            
            foreach (string str in str_member_list)
            {
                if (str != sendername)
                    msg.receiver_id = str;
            }

            msg.context = message;
            msg.sender_id = sendername;
            var user = Users.Where(u=>u.User_ID == msg.receiver_id);
            if(user.Any())
            {
                foreach(User ur in user)
                {
                    if(ur.InRooms != null && ur.InRooms.Contains(roomName))
                        msg.read = true;                   

                }
            }
                
           
            msg.sender_date = DateTime.UtcNow.Ticks;
                db.Messages.Add(msg);
                db.SaveChanges();
                strdate = msg.sender_date.ToString();
                Clients.Group(roomName).GetMessage( sendername, message, roomName, strdate);
          
        }

        //public void RelayMessage(string sendername, string message, string roomName, string strdate)
        //{
        //    Clients.Group(roomName).GetMessage(sendername, message, roomName, strdate);
        //}
        
        public Task JoinRoom(string roomId)
        {
            //ChatManager.onChatEvent(this, "JOIN ROOM: " + roomName);
            var user = Users.SingleOrDefault(u => u.ConnectionID == Context.ConnectionId);
            //if (user == null) return null;
            if(user.InRooms != null)
            {
                var inroom = user.InRooms.Where(i=>i.Contains(roomId));
                if (inroom == null)
                    user.InRooms.Add(roomId);
            }             
            else
            {
                user.InRooms = new List<string>();
                user.InRooms.Add(roomId);
            }            
            return Groups.Add(Context.ConnectionId, roomId);
        }

        public Task LeaveRoom(string roomId)
        {

            var user = Users.SingleOrDefault(u => u.ConnectionID == Context.ConnectionId);
            //if (user == null) return null;
            if (user.InRooms != null)
            {
                var inroom = user.InRooms.Where(i => i.Contains(roomId));
                if (inroom != null) 
                    user.InRooms.Remove(roomId);
            }
           
            return Groups.Remove(Context.ConnectionId, roomId);
        }

        /////////////----------
      
        //public override Task OnReconnected()
        //{
        //    string name = Context.User.Identity.Name;

        //    if (!_connections.GetConnections(name).Contains(Context.ConnectionId))
        //    {
        //        _connections.Add(name, Context.ConnectionId);
        //    }

        //    return base.OnReconnected();
        //}

    }
}