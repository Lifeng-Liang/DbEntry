
using System;
using System.Collections.Generic;
using System.Text;
using Lephone.Web;
using Lephone.Data;
using DebugLib.Models;

namespace DebugLib.Controllers
{
    public class UserController : ControllerBase<User>
    {
        public override void List()
        {
            bag["list"] = User.FindAll();
        }
    }
}
