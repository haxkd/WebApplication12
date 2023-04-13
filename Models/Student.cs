
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Web;
using System.Web.Helpers;

namespace WebApplication12.Models
{
    public class RegStudent
    {
        [Required(ErrorMessage = "Name is requiured")]
        public string name { get; set; }
        [Required(ErrorMessage = "Email is requiured")]
        [EmailAddress]
        public string email { get; set; }
        [Required(ErrorMessage = "password is requiured")]
        public string password { get; set; }
        [Required(ErrorMessage = "Confirm password is requiured")]
        [Compare("password", ErrorMessage = "password and confirm password didnt matched")]
        public string cpassword { get; set; }
    }
    public class LoginStudent
    {
        [EmailAddress]
        public string email { get; set; }
        public string password { get; set; }
    }
    public class Student
    {
        public string id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string dob { get; set; }
        public string image { get; set; }
        public string address { get; set; }
        public string phone { get; set; }

    }

    public class UpdateStudent
    {
        public string id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string dob { get; set; }
        public HttpPostedFileBase fileimage { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
    }
}