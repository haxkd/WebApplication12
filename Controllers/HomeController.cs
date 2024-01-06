using System.Linq;
using System.Web.Mvc;
using WebApplication12.Models;
namespace WebApplication12.Controllers
{

    public class HomeController : Controller
    {    
    
        CrudEntities _context = new CrudEntities();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ShowStudent()
        {
            var student = _context.Students.ToList();
            return View(student);
        }
        
        public ActionResult EditStudent(int id)
        {
            var student = _context.Students.FirstOrDefault(y => y.id == id);
            return View(student);
        }
        
        [HttpPost]
        public ActionResult EditStudent(int id, UpdateModel updateModel)
        {
            var student = _context.Students.FirstOrDefault(y => y.id == id);
            student.name = updateModel.name;
            student.email = updateModel.email;
            if (updateModel.password != null && updateModel.password != "")
            {
                student.password = updateModel.password;
            }
            student.dob = updateModel.dob;
            student.address = updateModel.address;
            student.phone = updateModel.phone;
            _context.SaveChanges();
            return View(student);
        }

        public ActionResult DeleteStudent(int id)
        {
            var student = _context.Students.FirstOrDefault(y=>y.id==id);
            _context.Students.Remove(student);
            _context.SaveChanges();
            TempData["msg"] = "Record Removed";
            return RedirectToAction("ShowStudent");
        }
    }
}
