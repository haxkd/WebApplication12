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
using System.Net.Mail;
using System.Text;
using System.Net;

namespace WebApplication12.Controllers
{
    public class HomeController : Controller
    {
        SqlConnection _connection = new SqlConnection("Data Source=DESKTOP-MBN74RR;Initial Catalog=Crud;Integrated Security=True");
        public ActionResult Index()
        {
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
                    if (login.password == dr["password"].ToString())
                    {
                        string user_id = dr["id"].ToString();
                        Session["user_id"] = user_id;
                        if (dr["count"].ToString() == "0")
                        {
                            string updateCount = $"update Student set count = 1 where id={user_id}";
                            _connection.Close();
                            _connection.Open();
                            new SqlCommand(updateCount, _connection).ExecuteNonQuery();
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
                    //Session["user_id"] = null;
                    //return RedirectToAction("UserProfile");
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

            string id = Session["user_id"].ToString();
            _connection.Open();
            SqlDataReader dr = new SqlCommand($"SELECT * FROM Student WHERE id={id}", _connection).ExecuteReader();
            dr.Read();
            Student student = new Student()
            {
                id = dr["id"].ToString(),
                name = dr["name"].ToString(),
                email = dr["email"].ToString(),
                dob = dr["dob"].ToString(),
                image = dr["image"].ToString(),
                address = dr["address"].ToString(),
                phone = dr["address"].ToString(),
            };
            return View(student);
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
            new SqlCommand(updatequrey, _connection).ExecuteNonQuery();


            return RedirectToAction("userprofile");
        }

        public ActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgetPassword(string email)
        {
            string checkQuery = $"SELECT * FROM Student where email='{email}'";
            _connection.Open();
            SqlCommand cmd = new SqlCommand(checkQuery, _connection);
            SqlDataReader rd = cmd.ExecuteReader();
            if (rd.Read())
            {
                string user_id = rd["id"].ToString();
                Random random = new Random();
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                string key = "";


                string checkKey = $"SELECT * FROM Forget where user_id='{user_id}'";
                SqlCommand cmd2 = new SqlCommand(checkKey, _connection);
                SqlDataReader dr2 = cmd2.ExecuteReader();
                if (dr2.Read())
                {
                    key = dr2["fkey"].ToString();
                }
                else
                {
                    key = new string(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray());
                    string query = $"INSERT INTO Forget(fkey,user_id) VALUES ('{key}','{user_id}')";
                    _connection.Close();
                    _connection.Open();
                    new SqlCommand(query, _connection).ExecuteNonQuery();
                }




                string link = $"https://localhost:44308/Home/Reset?key={key}";



                MailMessage message = new MailMessage("nitish0078@gmail.com", email);
                message.Subject = "Reset Your Password";
                message.Body = link;
                message.BodyEncoding = Encoding.UTF8;
                message.IsBodyHtml = true;
                SmtpClient client = new SmtpClient("smtp.gmail.com", 587); //Gmail smtp               
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("nitish0078@gmail.com", "aeujspxgcwxdwpxi");
                client.Send(message);

                ViewBag.msg = "mail sent please check it to reset";

            }
            else
            {
                ViewBag.msg = "email not exist";
            }
            return View();
        }


        public ActionResult Reset(string key)
        {
            _connection.Open();
            string checkKey = $"SELECT * FROM Forget where fkey='{key}'";
            SqlCommand cmd2 = new SqlCommand(checkKey, _connection);
            SqlDataReader dr2 = cmd2.ExecuteReader();
            if (!dr2.Read())
            {
                ViewBag.msg = "Link not valid";
                return RedirectToAction("ForgetPassword");
            }

            Session["user_id"] = dr2["user_id"].ToString();

            return View();
        }
        [HttpPost]
        public ActionResult Reset(string password, string cpassword)
        {
            if (password != cpassword)
            {
                ViewBag.msg = "Password and Confirm Password didnt matched";
                return View();
            }
            _connection.Open();
            string checkKey = $"UPDATE Student SET password ='{password}' where id='{Session["user_id"]}';DELETE FROM Forget where user_id='{Session["user_id"]}';";
            SqlCommand cmd2 = new SqlCommand(checkKey, _connection);
            cmd2.ExecuteNonQuery();
            Session["user_id"] = null;
            ViewBag.msg = "Password Has been updated";
            return RedirectToAction("login");
        }






    }
}