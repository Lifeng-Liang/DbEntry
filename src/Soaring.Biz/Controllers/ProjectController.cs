using Lephone.Web.Mvc;
using Soaring.Biz.Exceptions;
using Soaring.Biz.Models;

namespace Soaring.Biz.Controllers
{
    public class ProjectController : ControllerBase<Project>
    {
        public new string Show(long id)
        {
            var p = Project.FindById(id);
            if(p != null)
            {
                Session["project"] = id;
                return UrlTo.Controller("Requirement");
            }
            throw new InvalidOperationException();
        }
    }
}
