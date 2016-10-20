using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ChatWeb.Models;

namespace ChatWeb.Controllers
{
    [Authorize]
    public class ChatUsersController : Controller
    {
        private ChatDBContext db = new ChatDBContext();

        // GET: ChatUsers
        public ActionResult Index()
        {
            return View(db.ChatUsers.ToList());
        }

        // GET: ChatUsers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChatUser chatUser = db.ChatUsers.Find(id);
            if (chatUser == null)
            {
                return HttpNotFound();
            }
            return View(chatUser);
        }

        // GET: ChatUsers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ChatUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,user_id,user_name,pwd,user_email,user_type,phone_number,online_state,contact_list,room_list")] ChatUser chatUser)
        {
            if (ModelState.IsValid)
            {
                
                db.ChatUsers.Add(chatUser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(chatUser);
        }

        // GET: ChatUsers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChatUser chatUser = db.ChatUsers.Find(id);
            if (chatUser == null)
            {
                return HttpNotFound();
            }
            return View(chatUser);
        }

        // POST: ChatUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,user_id,user_name,pwd,user_email,user_type,phone_number,online_state,room_list, auth_token, device_id")] ChatUser chatUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(chatUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(chatUser);
        }

        // GET: ChatUsers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChatUser chatUser = db.ChatUsers.Find(id);
            if (chatUser == null)
            {
                return HttpNotFound();
            }
            return View(chatUser);
        }

        // POST: ChatUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ChatUser chatUser = db.ChatUsers.Find(id);
            db.ChatUsers.Remove(chatUser);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
