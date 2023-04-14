using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication12.Models;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using Microsoft.Win32;

namespace WebApplication12.Controllers
{
    public class HomeController : Controller
    {
        SqlConnection _connection = new SqlConnection("Data Source=DESKTOP-MBN74RR;Initial Catalog=Crud;Integrated Security=True");
        public ActionResult Index()
        {

            HttpCookie ck = new HttpCookie("user");
            ck["user_id"] = "xyz";
            //ck.Expires = DateTime.Now.AddDays(10);
            ck.Expires = DateTime.Now.AddYears(10000000);
            Response.Cookies.Add(ck);


            Request.Cookies["name"].ToString();


            return View();
        }

        public ActionResult Login()
        {
            if (Session["user_id"] != null)
            {
                return RedirectToAction("UserProfile");
            }

            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginStudent login)
        {
            if (Session["user_id"] != null)
            {
                return RedirectToAction("UserProfile");
            }
            if (ModelState.IsValid)
            {
                string checkEmail = $"SELECT id,email,password,count from Student where email = '{login.email}'";
                _connection.Open();
                SqlCommand cmd = new SqlCommand(checkEmail, _connection);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    if(login.password == dr["password"].ToString())
                    {
                       string user_id = dr["id"].ToString();
                        Session["user_id"] = user_id;
                        if (dr["count"].ToString() == "0")
                        {
                            string updateCount = $"update Student set count = 1 where id={user_id}";
                            _connection.Close();
                            _connection.Open();
                            new SqlCommand(updateCount,_connection).ExecuteNonQuery();
                            return RedirectToAction("UpdateProfile");
                        }
                        else
                        {
                            return RedirectToAction("userprofile");
                        }
                    }
                    else
                    {
                        ViewBag.msg = "Password is wrong";
                    }
                }
                else
                {
                    ViewBag.msg = "Email is wrong";
                }
            }
            return View();
        }
        public ActionResult Register()
        {
            if (Session["user_id"] != null)
            {
                return RedirectToAction("UserProfile");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Register(RegStudent register)
        {
            if (Session["user_id"] != null)
            {
                return RedirectToAction("UserProfile");
            }
            if (ModelState.IsValid)
            {
                string checkEmail = $"SELECT email from Student where email = '{register.email}'";
                _connection.Open();
                SqlCommand cmd = new SqlCommand(checkEmail, _connection);

                if (!cmd.ExecuteReader().Read())
                {
                    _connection.Close();
                    string query = $"INSERT INTO Student(name,email,password,count) VALUES('{register.name}','{register.email}','{register.password}','0')";
                    _connection.Open();
                    SqlCommand cmd2 = new SqlCommand(query, _connection);
                    cmd2.ExecuteNonQuery();
                    ViewBag.msg = "registeration successfully";
                    _connection.Close();
                }
                else
                {
                    ModelState.AddModelError("email", "Email Already exist");
                }               
            }
            return View();
        }

        public ActionResult UserProfile()
        {
            if (Session["user_id"] == null)
            {
                return RedirectToAction("login");
            }

            Response.Write(Session["user_id"].ToString());

            return View();
        }

        public ActionResult UpdateProfile()
        {
            if (Session["user_id"] == null)
            {
                return RedirectToAction("login");
            }

            string user_id = Session["user_id"].ToString();
            _connection.Open();
            SqlCommand cmd = new SqlCommand($"select * from Student where id={user_id}", _connection);
            SqlDataReader dr = cmd.ExecuteReader();
            dr.Read();
            Student student = new Student()
            {
                id = dr["id"].ToString(),
                name = dr["name"].ToString(),
                email = dr["email"].ToString(),
                
                phone = dr["phone"].ToString(),
                dob = dr["dob"].ToString(),
                image = dr["image"].ToString(),
                address = dr["address"].ToString(),
            };

            return View(student);
        }

        [HttpPost]
        public ActionResult UpdateProfile(UpdateStudent updateStudent)
        {
            if (Session["user_id"] == null)
            {
                return RedirectToAction("login");
            }

            string user_id = Session["user_id"].ToString();

            string name = updateStudent.name;
            string email = updateStudent.email;
            string password = "";
            string dob = updateStudent.dob;
            string address = updateStudent.address;
            string phone = updateStudent.phone;
            string storepath = "";

            string query = $"select * from Student where id= '{user_id}'";
            _connection.Open();


            SqlDataReader dr = new SqlCommand(query, _connection).ExecuteReader();
            dr.Read();
            password = dr["password"].ToString();
            storepath = dr["image"].ToString();

            if (updateStudent.password != null)
            {
                if (updateStudent.password.Trim() != "")
                {
                    password = updateStudent.password;
                }

            }

            if (updateStudent.fileimage != null)
            {
                string folder = "~/images";
                string filename = updateStudent.fileimage.FileName;
                string savepath = Path.Combine(Server.MapPath(folder), filename);
                updateStudent.fileimage.SaveAs(savepath);
                storepath = folder + "/" + filename;
            }
            _connection.Close();



            string updatequrey = $"UPDATE Student SET name='{name}',email='{email}',password='{password}',dob='{dob}',address='{address}',phone='{phone}',image='{storepath}' where id= '{user_id}'";
            _connection.Open();
            new SqlCommand(updatequrey,_connection).ExecuteNonQuery();


            return RedirectToAction("userprofile");
        }



        }
}