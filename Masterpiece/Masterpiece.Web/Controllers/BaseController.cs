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
    public class BaseController : Controller
    {
        protected MasterpieceDbContextFactory db = MasterpieceDbContextFactory.CreateDbContext();

        protected override void Dispose(bool disposing)
        {
            if (db != null)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            LogHelper.ErrorLog("", filterContext.Exception);

            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.StatusCode = 200;

            base.OnException(filterContext);

            JsonSimpleResponse response = new JsonSimpleResponse();
            if (filterContext.Exception is MasterException && (filterContext.Exception as MasterException).ErrorCode < 10000)
            {
                response.ErrorCode = (filterContext.Exception as MasterException).ErrorCode;
                response.ErrorMsg = (filterContext.Exception as MasterException).Message;
            }
            else
            {
                response.ErrorCode = 10000;
                response.ErrorMsg = "出错了，请联系管理员";
            }

            if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.Result = new JsonResult() { Data = response };
            }
            else
            {
                filterContext.Result = new RedirectResult(Url.Action("index", "error", new { code = response.ErrorCode, msg = response.ErrorMsg }));
            }
        }
    }
}