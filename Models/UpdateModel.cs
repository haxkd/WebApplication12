using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication12.Models
{
    public class UpdateModel
    {
        public string name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public DateTime dob { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
    }
}