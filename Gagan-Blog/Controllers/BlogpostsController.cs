using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Gagan_Blog.Helpers;
using Gagan_Blog.Models;

namespace Gagan_Blog.Controllers
{
    public class BlogpostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Blogposts
        public ActionResult Index()
        {
            return View(db.Posts.ToList());
        }

        // GET: Blogposts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blogpost blogpost = db.Posts.Find(id);
            if (blogpost == null)
            {
                return HttpNotFound();
            }
            return View(blogpost);
        }

        // GET: Blogposts/Create
        
        public ActionResult Create()
        {
            return View();
        }

        // POST: Blogposts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Create([Bind(Include = "Id,Created,Updated,Title,Slug,Body,MediaUrl,Published")] Blogpost blogpost, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                var Slug = StringUtilites.URLFriendly(blogpost.Title);
                if (String.IsNullOrWhiteSpace(Slug))
                {
                    ModelState.AddModelError("Title", "Invalid title");
                    return View(blogpost);
                }

                if (ImageUploadValidator.IsWebFriendlyImage(image))
                {
                    var fileName = Path.GetFileName(image.FileName);
                    image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), fileName));
                    blogpost.MediaUrl = "/Uploads/" + fileName;
                }

                if (db.Posts.Any(p => p.Slug == Slug))
                {
                    ModelState.AddModelError("Title", "The title must be unique");
                    return View(blogpost);
                }

                blogpost.Slug = Slug;
                blogpost.Created = DateTimeOffset.Now;
                db.Posts.Add(blogpost);
                db.SaveChanges();
                return RedirectToAction("Index");

            }

            return View(blogpost);
        }

        // GET: Blogposts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blogpost blogpost = db.Posts.Find(id);
            if (blogpost == null)
            {
                return HttpNotFound();
            }
           
            return View(blogpost);
        }

        // POST: Blogposts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Created,Updated,Title,Slug,Body,MediaUrl,Published")] Blogpost blogpost, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {

                if (ImageUploadValidator.IsWebFriendlyImage(image))
                {
                    var fileName = Path.GetFileName(image.FileName);
                    image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), fileName));
                    blogpost.MediaUrl = "/Uploads/" + fileName;
                }

                db.Entry(blogpost).State = EntityState.Modified;

                blogpost.Updated = DateTime.Now;
                db.SaveChanges();
                return RedirectToAction("Index");
            };
           

            return View(blogpost);
        }


        // GET: Blogposts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blogpost blogpost = db.Posts.Find(id);
            if (blogpost == null)
            {
                return HttpNotFound();
            }
            return View(blogpost);
        }

        // POST: Blogposts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Blogpost blogpost = db.Posts.Find(id);
            db.Posts.Remove(blogpost);
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
