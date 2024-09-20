using Interview.Model;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IdentityUI.Controllers
{
    public class testController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}