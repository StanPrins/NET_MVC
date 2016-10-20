using ChatWeb.Hubs;
using ChatWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ChatWeb.Controllers
{
    public class ChatController : Controller
    {
        private ChatDBContext db = new ChatDBContext();
        private ChatHub chatHub;
       
        public ActionResult Chat(string user_id, string receiver_id, string receiver_name, string auth_token)
        {
            
            try
            {
              var  checkuser= db.ChatUsers.SingleOrDefault(s => s.user_id == user_id && s.auth_token == auth_token);
                if (checkuser == null)
                    return RedirectToAction("ChatLogin");
            }
            catch(Exception e)
            {
                string strerr = e.ToString();
                return RedirectToAction("ChatLogin");
            }
            
            
            ViewBag.user_id = user_id;
            ViewBag.receiver_id = receiver_id;
            ViewBag.receiver_name = receiver_name;
            ViewBag.auth_token = auth_token;

            return View();
        }
       
        /**
         * Show the chat login page.
         */
        public ActionResult ChatLogin()
        {
          
            return View();
        }
       public ActionResult ChatLoginSuccess(string user_id, string auth_token)
        {
            var checkuser = db.ChatUsers.SingleOrDefault(s => s.user_id == user_id && s.auth_token == auth_token);
            if (checkuser == null)
                return RedirectToAction("ChatLogin");
            ViewBag.user_id = user_id;
            ViewBag.auth_token = auth_token;
            return View();
        }
        public ActionResult OperatorList(string user_id, string auth_token)
        {
            var checkuser = db.ChatUsers.SingleOrDefault(s => s.user_id == user_id && s.auth_token == auth_token);
            if (checkuser == null)
                return RedirectToAction("ChatLogin");
            ViewBag.user_id = user_id;
            ViewBag.auth_token = auth_token;
            
            return View();
        }
        public ActionResult RoomList(string user_id, string auth_token)
        {
            var checkuser = db.ChatUsers.SingleOrDefault(s => s.user_id == user_id && s.auth_token == auth_token);
            if (checkuser == null)
                return RedirectToAction("ChatLogin");
            ViewBag.user_id = user_id;
            ViewBag.auth_token = auth_token;
            return View();
        }
        public ActionResult ChatLogout(string user_id, string auth_token)
        {
            var checkuser = db.ChatUsers.SingleOrDefault(s => s.user_id == user_id && s.auth_token == auth_token);
            //if (checkuser == null)

            return RedirectToAction("ChatLogin");
            //ViewBag.user_id = user_id;
            //ViewBag.auth_token = auth_token;
                  
            //return View();
        }
    
        public ActionResult ChatRegister()
        {
            return View();
        }
      

        public ActionResult UpdateUser()
        {
          
            return View();
        }
       
        // GET: ChatUsers/Details/5
        public ActionResult Select(int? id)
        {
            
            return RedirectToAction("Index");
        }
       
      

    }
}