using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Permissions;

namespace Lephone.Web.Common
{
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class LabelIDConverter : ControlIDConverter
    {
        protected override bool FilterControl(Control control)
        {
            return control is Label;
        }
    }
}
