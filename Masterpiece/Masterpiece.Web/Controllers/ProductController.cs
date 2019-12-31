using Masterpiece.Bll;
using Masterpiece.Code.Common;
using Masterpiece.Domain.MasterException;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Masterpiece.Web.Controllers
{
    public class ProductController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}