using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplicationHR.Models.DM;
using System.IO;

namespace WebApplicationHR.Controllers
{
    public class EmployeeController : Controller
    {
        private EmployeeEntities db = new EmployeeEntities();

        // GET: /Employee/
        public async Task<ActionResult> Index(string sortOrder, string searchString, string branch)
        {
            ViewBag.SortingName = String.IsNullOrEmpty(sortOrder) ? "Last Name" : "";
            var hrs = db.HRs.AsQueryable();
            if (!String.IsNullOrEmpty(searchString))
            {
                hrs = hrs.Where(s => s.LName.Contains(searchString)
                                       || s.FName.Contains(searchString));
            }
            if (!String.IsNullOrEmpty(branch))
            {
                hrs = hrs.Where(s => s.branch.Contains(branch));
            }
            switch(sortOrder)
            {
                case "Last Name":
                    hrs=hrs.OrderByDescending(s=> s.LName);
                    break;
                default:
                    hrs.OrderBy(s=>s.FName);
                    break;
            }

            return View(await hrs.ToListAsync());
        }

        // GET: /Employee/Details/5
       
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR hr = await db.HRs.FindAsync(id);
            if (hr == null)
            {
                return HttpNotFound();
            }
            return View(hr);
        }

        // GET: /Employee/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Employee/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(/*[Bind(Include = "Id,FName,MName,LName,Gender,c_residence,address,m_number,email,d_work,r_status,m_status,n_kids,s_date,branch,title,salary,off_day,u_name,u_address,u_years,u_grad,degree,v_name,v_years,v_training,sa_name,sa_address,sa_years,so_name,so_address,so_years,p_name,p_address,nid_number,nssf,dr_li,dr_li_exp,pre_job,position,time,emp_name,emp_address,supervisor,last_day,emer_name,emer_number,image")]*/ HR hr, HttpPostedFileBase img)
        {
            if (ModelState.IsValid)
            {
                if (img != null)
                {
                    string ImageName = Path.GetFileName(img.FileName);
                    string path = Server.MapPath(("~/Images/profile/")+ImageName);
                    img.SaveAs(path);
                    hr.image = img.FileName;
                    db.HRs.Add(hr);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                else
                {
                    
                    db.HRs.Add(hr);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }

            return View(hr);
        }

        // GET: /Employee/Edit/5
       
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR hr = await db.HRs.FindAsync(id);
            if (hr == null)
            {
                return HttpNotFound();
            }
            return View(hr);
        }

        // POST: /Employee/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(HR hr, HttpPostedFileBase img_e)
        {
            if (ModelState.IsValid)
            {
                if (img_e != null)
                {
                    string ImageName = Path.GetFileName(img_e.FileName);
                    string path = Server.MapPath(("~/Images/profile/") + ImageName);
                   
                    hr.image = img_e.FileName;
                    db.Entry(hr).State = EntityState.Modified;
                   db.SaveChanges();
                    img_e.SaveAs(path);
                    return RedirectToAction("Index");
                }
                else
                {

                    
                    var q = from item in db.HRs.Where(u => u.Id == hr.Id) select item.image;
                    hr.image = q.First();
                    
                    db.Entry(hr).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
               
            }
            return View(hr);
        }
        

        // GET: /Employee/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR hr = await db.HRs.FindAsync(id);
            if (hr == null)
            {
                return HttpNotFound();
            }
            return View(hr);
        }

        // POST: /Employee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            HR hr = await db.HRs.FindAsync(id);
            db.HRs.Remove(hr);
            await db.SaveChangesAsync();
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
