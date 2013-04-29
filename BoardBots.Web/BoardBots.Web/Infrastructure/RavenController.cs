using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Raven.Client;

namespace BoardBots.Web
{
    public abstract class RavenController : Controller
    {
        protected IDocumentSession RavenSession { get; private set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            RavenSession = HttpContext.Items["RavenSession"] as IDocumentSession;
        }
    }
}
