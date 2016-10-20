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
    public class MessagesController : Controller
    {
        private ChatDBContext db = new ChatDBContext();
        private int pageSize = 20;
        private int excluedRows = 0;
        private static int pageNumber = 0;
        // GET: Messages
        public ActionResult Index(int?id)
        {
            if (id == null || id<0) id = 0;
            id = (Int32)id  ;
            if (id == 0 && pageNumber !=0)
                pageNumber = pageNumber - 1;
            if(id ==1)
                pageNumber = pageNumber + 1;
                                      
            excluedRows = pageSize * pageNumber;
            List<Result_Message> result_messageList = new List<Result_Message>();
            var count = (from o in db.Messages select o).Count();
            if (count < pageNumber * pageSize)
                return View(result_messageList);
            var mlist = db.Messages.OrderByDescending(s => s.sender_date).Skip(excluedRows).Take(pageSize);
                        
            foreach(Message m in mlist)
            {
                Result_Message rm = new Result_Message();
                rm.ID = m.ID;
                rm.message_id = m.message_id;
                rm.room_id = m.room_id;
                rm.sender_id = m.sender_id;
                rm.receiver_id = m.receiver_id;
                rm.sender_date = new DateTime(m.sender_date).ToString("MMM d yyyy hh:mm tt");
                rm.receiver_date = m.receiver_date;
                rm.context = m.context;
                rm.read = m.read;
                result_messageList.Add(rm);
            }
            ViewBag.npage = pageNumber+1;
            return View(result_messageList);
        }
       
        // GET: Messages/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Message message = db.Messages.Find(id);
            if (message == null)
            {
                return HttpNotFound();
            }
            return View(message);
        }

        // GET: Messages/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Messages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,message_id,room_id,sender_id,receiver_id,sender_date,receiver_date,context")] Message message)
        {
            if (ModelState.IsValid)
            {
                db.Messages.Add(message);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(message);
        }

        // GET: Messages/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Message message = db.Messages.Find(id);
            if (message == null)
            {
                return HttpNotFound();
            }
            return View(message);
        }

        // POST: Messages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,message_id,room_id,sender_id,receiver_id,sender_date,receiver_date,context")] Message message)
        {
            if (ModelState.IsValid)
            {
                db.Entry(message).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(message);
        }

        // GET: Messages/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Message message = db.Messages.Find(id);
            if (message == null)
            {
                return HttpNotFound();
            }
            db.Messages.Remove(message);
            db.SaveChanges();
            return RedirectToAction("Index");
            //return View(message);
        }

        // POST: Messages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Message message = db.Messages.Find(id);
            db.Messages.Remove(message);
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
