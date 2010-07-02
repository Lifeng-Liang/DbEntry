using Soaring.Biz.Models;
using Lephone.Web.Mvc;

namespace Soaring.Biz.Controllers
{
    public class WorkitemController : ControllerBase<Workitem>
    {
        public void Grid()
        {
            this["ListProposed"] = Workitem.Find(p => p.Status == WorkitemStage.Proposed, "Id DESC");
            this["ListWorking"] = Workitem.Find(p => p.Status == WorkitemStage.Working, "Id DESC");
            this["ListReadyForTest"] = Workitem.Find(p => p.Status == WorkitemStage.ReadyForTest, "Id DESC");
            this["ListComplated"] = Workitem.Find(p => p.Status == WorkitemStage.Complated, "Id DESC");
        }
    }
}
