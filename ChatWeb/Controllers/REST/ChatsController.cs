using ChatWeb.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.Results;


namespace ChatWeb.Controllers
{
    [RoutePrefix("api/chats")]
    public class ChatsController : ApiController
    {

        /* REST service
        * 
        * POST  /api/chats/user/login
        * 
        * request
        *  {
        *      'user_id': user_id, 
        *      'user_email':email,
        *      'pwd': password ,        
        *  }
        *  
        * response        
        *  {       
        *       'res_type':'login'
        *      'state': 0,1,2,3,5,  //0:success, 1: no id , 2:no email , 3: no password, 4:db error 5: items error
        *      'auth_token': auth_token
        *  }
        */
        private ChatDBContext db = new ChatDBContext();
        [Route("user/login")]
        public Object login(JObject person)
        {
            
            string user_id;
            string email;
            string pwd;            
            string auth_token;            
            long utc_time;
            string device_id="";

            Response_Person rp = new Response_Person();
            try
            {
                user_id = person["user_id"].ToString();
                device_id = person["device_id"].ToString();
                if (!String.IsNullOrEmpty(device_id))
                {
                    if (  String.IsNullOrEmpty(user_id))
                    {
                        var result_chatuser = db.ChatUsers.SingleOrDefault(d => d.device_id == device_id);
                        if (result_chatuser != null)
                        {
                            rp.res_type = "login";
                            rp.state = 9;
                            rp.user_id = result_chatuser.user_id;
                            rp.user_name = result_chatuser.user_name;
                            rp.auth_token = result_chatuser.auth_token;
                            return rp;
                        }
                    }
                   
                }

                                                 
                 email = person["user_email"].ToString();
                if (user_id.Contains("@"))
                {
                    email = user_id;
                    user_id = "";
                }
                    
                 pwd = person["pwd"].ToString();
                
                //string offset_time_str = person["offset_time"].ToString();
                //offset_time = Convert.ToInt64(offset_time_str);
                utc_time = DateTime.UtcNow.Ticks;
            }
            catch(Exception e)
            {
                string estring = e.ToString();                
                //rp.res_type = "login";
                rp.res_type = estring;
                rp.state = 5;
                rp.user_id = "";
                rp.user_name = "";
                rp.auth_token = "";
                return rp;
            }
            
          
           
            var chatusers = from m in db.ChatUsers select m;
            
            if (!String.IsNullOrEmpty(user_id))
            {
                chatusers = chatusers.Where(s => s.user_id == user_id);
                if (!chatusers.Any())
                {
                    rp.res_type = "login";
                    rp.state = 2;
                    rp.user_id = "";
                    rp.user_name = "";
                    rp.auth_token = "";
                    return rp;

                }
            }
            if (!String.IsNullOrEmpty(email))
            {
                chatusers = chatusers.Where(s => s.user_email == email);
                if (!chatusers.Any())
                {
                    rp.res_type = "login";
                    rp.state = 2;
                    rp.user_id = "";
                    rp.user_name = "";
                    rp.auth_token = "";
                    return rp;

                }
            }
            if (!String.IsNullOrEmpty(pwd))
            {
                chatusers = chatusers.Where(s => s.pwd == pwd);
                if (!chatusers.Any())
                {

                    rp.res_type = "login";
                    rp.state = 3;
                    rp.user_id = "";
                    rp.user_name = "";
                    rp.auth_token = "";
                    return rp;

                }
            }
            
            auth_token = Guid.NewGuid().ToString();
            
            var result = db.ChatUsers.SingleOrDefault(b=>b.user_id ==user_id);
            if(string.IsNullOrEmpty(user_id))
                result =  db.ChatUsers.SingleOrDefault(b => b.user_email == email);
            
            try
            {
                result.auth_token = auth_token;                    
                result.online_state = true;
                result.last_logdate = utc_time;
                result.device_id = device_id;
                db.SaveChanges();
                if (!string.IsNullOrEmpty(device_id))
                {
                    var dev_result = db.ChatUsers.Where(k => k.device_id == device_id && k.user_id != user_id);
                    foreach(ChatUser cu in dev_result )
                    {
                        cu.device_id = "";
                    }
                    db.SaveChanges();
                }
                
            }catch(Exception e1)
            {

                rp.res_type = "login";
                rp.state = 4;
                rp.user_id = "";
                rp.user_name = "";
                rp.auth_token = "";
                return rp;
            }
                
            

            rp.res_type = "login";
            rp.state = 0;
            rp.auth_token = auth_token;
            rp.user_id = result.user_id;
            rp.user_name = result.user_name;
            return rp;
            
        }
        /* REST service
        * 
        * POST  /api/chats/user/register
        * 
        * request
        *  {   
        *      'user_id':userid,    //require, unique     
        *      'user_email':user_email,  //require, unique
        *      'user_name' :user_name,  //require
        *      'pwd': password,     //require
        *      'phone_number' :phone_number //no require
        *      
        *  }
        *  
        * response        
        *  {         
        *      'res_type':'register'
        *      'state': 0,1,2,3,5,  //0:success, 1: no unique id , 2:no unique email , 3: no unique device_id  4: db error 
        *   5: items error 6: id input error
        *      'auth_token': auth_token
        *  }
        *  tasks
        *  (        
        *    chatuser.auth_token = guid
        *             .online_state =true
        *   )
        */
        [Route("user/register")]
        [HttpPost]
        public Object register(JObject person)
        {
            string user_id;
            string user_email;
            string user_name;
            string pwd;
            long phone_number;                        
            long utc_time;
            string auth_token;
            string device_id="";

            Response_Person res_person = new Response_Person();
            try
            {
                user_id = person["user_id"].ToString();
                if (user_id.Contains('&') || user_id.Contains('@'))
                {

                    res_person.res_type = "register";
                    res_person.state = 6;
                    res_person.user_id = "";
                    res_person.user_name = "";
                    res_person.auth_token = "";
                    return res_person;
                }
                user_email = person["user_email"].ToString();
                user_name = person["user_name"].ToString();
                pwd = person["pwd"].ToString();
                device_id = person["device_id"].ToString();
                string phone_str = person["phone_number"].ToString();                
                if(String.IsNullOrEmpty(phone_str))
                    phone_number = 0;
                else
                    phone_number = Convert.ToInt64(phone_str);
                //offset_time = Convert.ToInt64(offset_time_str);
                utc_time = DateTime.UtcNow.Ticks;
            }
            catch (Exception e)
            {
                string estring = e.ToString();

                res_person.res_type = "register";
                res_person.state = 5;
                res_person.user_id = "";
                res_person.user_name = "";
                res_person.auth_token = "";
                return res_person;
            }
            auth_token = Guid.NewGuid().ToString();
            var result = db.ChatUsers.Where(p => p.user_id == user_id);
            if (result.Any())
            {
                res_person.res_type = "register";
                res_person.state = 1;
                res_person.user_id = "";
                res_person.user_name = "";
                res_person.auth_token = "";
                return res_person;
            }
            result = db.ChatUsers.Where(p => p.user_email == user_email);
            if (result.Any())
            {
                res_person.res_type = "register";
                res_person.state = 2;
                res_person.user_id = "";
                res_person.user_name = "";
                res_person.auth_token = "";
                return res_person;

            }
            if (!string.IsNullOrEmpty(device_id))
            {
                var result_chatuser = db.ChatUsers.SingleOrDefault(p=>p.device_id == device_id);
                if (result_chatuser != null)
                {
                    result_chatuser.device_id = "";                    
                    db.SaveChanges();

                }
            }
            
            ChatUser chatuser = new ChatUser();
            chatuser.user_id = user_id;
            chatuser.user_email = user_email;
            chatuser.user_name = user_name;
            chatuser.pwd = pwd;
            chatuser.phone_number = phone_number;
            chatuser.reg_date = utc_time;
            chatuser.last_logdate = utc_time;
            chatuser.device_id = device_id;
            chatuser.auth_token = auth_token;
            chatuser.online_state = false;
            try
            {              
              
                db.ChatUsers.Add(chatuser);
                db.SaveChanges();
                if (!string.IsNullOrEmpty(device_id))
                {
                    var dev_result = db.ChatUsers.Where(k => k.device_id == device_id && k.user_id != user_id);
                    foreach (ChatUser cu in dev_result)
                    {
                        cu.device_id = "";
                    }
                    db.SaveChanges();
                }
            }
            catch(Exception e2)
            {
                res_person.res_type = "register";
                res_person.state = 4;
                res_person.user_id = "";
                res_person.user_name = "";
                res_person.auth_token = "";
                return res_person;
            }


            res_person.res_type = "register";
            res_person.state = 0;
            res_person.user_id = user_id;
            res_person.user_name = user_name;
            res_person.auth_token = auth_token;
            return res_person;
            
            
        }
        public class Response_Person
        {
            public string res_type { get; set; }
            public int state { get; set; }
            public string auth_token{ get; set; }
            public string user_id { get; set; }
            public string user_name { get; set; }
        }
        /*
         * api/chats/contacts?user_id=user_id&auth_token=
         * [HttpGet]
         * Response
         *   {
         *       res_type:'contacts',
         *       state:'0',1,2,3,4  //0:success, 1: no user, 2: no operator
         *       operators:[{
         *       receiver_id:receiver_id,
         *       receiver_name:receiver_name},
         *       {
         *       receiver_id:receiver_id,
         *       receiver_name:receiver_name
         *       }]
         *       
         *   }
         *   task
         *   (
         *   
         *   )
         */
        [Route("contacts")]
        [HttpGet]
        public Object Contacts(string user_id, string auth_token)
        {          

            var checkuser = db.ChatUsers.SingleOrDefault(s => s.user_id == user_id && s.auth_token == auth_token);
            if(checkuser == null)
            {
                return new
                {
                    res_type ="contacts",
                    state = "1",
                    operators =""
                };
            }
            var result = db.ChatUsers.Where(p=> p.user_id!=user_id);
            if(!result.Any())
            {
                return new
                {
                    res_type = "contacts",
                    state = "2",
                    operators=""
                };
            }

            List<Operator> myOperators = new List<Operator>();
            foreach (ChatUser cuser in result)
            {
                Operator op = new Operator();
                op.receiver_id = cuser.user_id;
                op.receiver_name = cuser.user_name;
                op.online_state = cuser.online_state;
                myOperators.Add(op);
                
            }

            return new
            {
                res_type = "contacts",
                state = "0",
                operators = myOperators
            };
        }
        class Operator
        {
            public string receiver_id { get; set; }
            public bool online_state { get; set; }
            public string receiver_name { get; set; }

        }

        /*
         * api/chats/rooms/users?user_id=user_id&auth_token=
         * [HttpGet]
         * Response
         *   {
         *       res_type:'rooms',
         *       state:'0',1,2,3,4  //0:success, 1: no user, 2: no operator
         *       rooms:[{
         *       room_id:room_id,
         *       receiver_id:receiver_id
         *       receiver_name:receiver_name
         *       },
         *       {
         *       room_id:room_id,
         *       receiver_id:receiver_id
         *       receiver_name:receiver_name
         *       }]
         *       
         *   }
         *   task
         *   (
         *   
         *   )
         */
        [Route("rooms")]
        [HttpGet]

        public Object ChatRooms(string user_id, string auth_token)
        {
            var result = db.ChatUsers.SingleOrDefault(u=>u.user_id == user_id && u.auth_token == auth_token);
            if (result == null)
            {
                return new
                {
                    res_type = "rooms",
                    state = "1",
                    rooms = ""
                };
            }
            var result_rooms = db.Rooms.Where(r=>r.member_list.Contains(user_id));
            if (!result_rooms.Any())
            {
                return new
                {
                    res_type = "rooms",
                    state = "2",
                    rooms = ""
                };
            }
            List<Roomclass> rooms_list = new List<Roomclass>();
            string lastm ="";
            long last_date=0;
            foreach (Room r in result_rooms)
            {
               
                Roomclass roomclass = new Roomclass();
                roomclass.room_id = r.room_id;
                var ms = db.Messages.Where(m => m.room_id == r.room_id).OrderByDescending(s => s.sender_date).Take(1);
                //k= ms["context"].ToString();
                lastm = "";
                if (ms != null)
                {
                    lastm = "";
                    foreach (Message m in ms)
                    {
                        lastm = m.context;
                        last_date = m.sender_date;

                    }
                }
                int unread = db.Messages.Count(k => k.read != true && k.room_id == r.room_id && k.sender_id !=user_id);
                string[] str_member_list = r.room_id.Split('&');
                foreach(string str in str_member_list)
                {
                    if (str != user_id)
                        roomclass.receiver_id = str;
                }
                result = db.ChatUsers.SingleOrDefault(a => a.user_id == roomclass.receiver_id);
                if(result !=null)
                {
                    roomclass.receiver_name = result.user_name;
                    roomclass.make_date = r.make_date;
                    roomclass.last_msg = lastm;
                    roomclass.last_date = last_date;
                    roomclass.isOnline_rec = result.online_state;
                    roomclass.unread_count = unread;
                    rooms_list.Add(roomclass);
                }
                
            }
            return new
            {
                res_type = "rooms",
                state = "0",
                rooms = rooms_list
            };

        }

        
        class Roomclass
        {
            public string room_id { get; set; }            
            public string receiver_id { get; set; }
            public string receiver_name { get; set; }
            public bool isOnline_rec { get; set; }
            public long make_date { get; set; }
            public long last_date { get; set; }
            public string last_msg { get; set; }
            public int unread_count { get; set; }
        }
        /* REST service
        * 
        * GET  /api/chats/user/logout
        * 
        
        *  
        * response        
        *  {         
        *      'res_type':'logout'
        *      'state': 0,1,2,3,5,  //0:success, 1: no unique id , 2:no unique email ,  4: db error 5: items error        
        *  }
        *  tasks
        *  (        
        *    chatuser.online_state = false
        *    chatuser.auth_token = ""
        *             
        *   )
        */
        [Route("user/logout")]
        [HttpGet]
        public Object ChatLogout(string user_id, string auth_token)
        {
            var result = db.ChatUsers.SingleOrDefault(u => u.user_id == user_id && u.auth_token == auth_token);
            if (result == null)
            {
                return new
                {
                    res_type = "logout",
                    state = "1"
                };
            }
            result.online_state = false;
            result.auth_token = "";
            return new
            {
                res_type = "logout",
                state = "0"
            };
        }
        /* REST service
       * 
       * POST  /api/chats/messages
       * 
       * request
       * {
       *    room_id:
       *    user_id:
       *    receiver_id:
       *    auth_token:
       *    time :
       *    count :
       * }
       *  
       * response        
       *  {         
       *      'res_type':'message'
       *      'state': 0,1,2,3,5,  //0:success, 1: no unique id , 2:no unique email ,  4: db error 5: items error        
       *      room_id:room_id,
       *      user_id:user_id,
       *      receiver_id,
       *      message
       *      [{
           *      room_id:room_id
           *      sender_id:sender_id
           *      rec_id:rec_id
           *      message_id:message_id
           *      context:context
           *      sender_date:sender_date
           *      receiver_date:receiver_date
           *      read:read
       *      },
       *      {
           *      room_id:room_id
           *      sender_id:sender_id
           *      rec_id:rec_id
           *      message_id:message_id
           *      context:context
           *      sender_date:sender_date
           *      receiver_date:receiver_date
           *      read:read
       *      }]
       *  }
       *  tasks
       *  (        
       *    
       *    create new room if room_id is empty
       *             
       *   )
       */
        class messageRes
        {            
            public string sender_id { get; set; }
            public string receiver_id { get; set; }
            public string message_id { get; set; }
            public string context { get; set; }
            public long sender_date { get; set; }
            public long receiver_date { get; set; }
            public bool read { get; set; }
            

        }
        [Route("messages")]
        [HttpPost]
        public Object PostMsg(JObject Msg)
        {

            string room_id;
            string user_id;
            string receiver_id;
            string receiver_name;
            string auth_token;
            string str_count;
            try
            {
                room_id = Msg["room_id"].ToString();
                user_id = Msg["user_id"].ToString();
                receiver_id = Msg["receiver_id"].ToString();
                auth_token = Msg["auth_token"].ToString();                
                str_count = Msg["count"].ToString();
                var result = db.ChatUsers.SingleOrDefault(u => u.user_id == user_id && u.auth_token == auth_token);
                if (result == null)
                {
                    return new
                    {
                        res_type = "message",
                        state = "1",
                        user_id = "",
                        receiver_id = "",
                        receiver_name = "",
                        room_id = "",
                        message =""
                    };
                }
                result = db.ChatUsers.SingleOrDefault(r => r.user_id == receiver_id);
                if (result == null)
                {
                    return new
                    {
                        res_type = "message",
                        state = "2",
                        user_id = "",
                        receiver_id = "",
                        receiver_name = "",
                        room_id = "",
                        message = ""
                    };
                }
                receiver_name = result.user_name;

            }
            catch(Exception e)
            {
                return new
                {
                    res_type = "message",
                    state = "5",
                    room_id="",
                    user_id = "",
                    receiver_id = "",
                    receiver_name = "",
                    message =""
                };
            }
            
            var result_room = db.Rooms.SingleOrDefault(r => r.room_id.Contains(user_id) && r.room_id.Contains(receiver_id));

            if (result_room ==null )
            {
                room_id = user_id + "&" + receiver_id;
                Room rnew = new Room();
                rnew.room_id = room_id;
                rnew.make_date = DateTime.UtcNow.Ticks;
                
                rnew.member_list = user_id + "," + receiver_id;
                db.Rooms.Add(rnew);
                db.SaveChanges();
                return new
                {
                    res_type = "message",
                    state = "0",
                    room_id = room_id,
                    user_id = user_id,
                    receiver_id = receiver_id,
                    receiver_name = receiver_name,
                    message = ""
                };
            }
            room_id = result_room.room_id;

            

            var ms = db.Messages.Where(m=>m.room_id == room_id).OrderByDescending(s => s.sender_date);           
            
            int ncount = Convert.ToInt32(str_count);
            int step = ncount >10 ? (ncount - 10) : ncount;
            var result_mlist = ms.Skip(step).Take(10).OrderBy(s => s.sender_date);
            var unread_ms = db.Messages.Where(p=>p.room_id == room_id && p.read != true && p.sender_id != user_id);            
            int nUnread = unread_ms.Count();            
            if (nUnread != 0)
            {
                foreach (Message mm in unread_ms)
                {
                    mm.read = true;
                    
                }
                db.SaveChanges();
            }
            return new
            {
                res_type = "message",
                state = "0",
                room_id = room_id,
                user_id = user_id,
                receiver_id = receiver_id,
                receiver_name = receiver_name,
                message = result_mlist
            };
        }
        
    }
}