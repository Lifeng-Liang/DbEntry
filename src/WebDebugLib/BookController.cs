
using System;
using System.Collections.Generic;
using System.Text;
using Lephone.Web;

namespace DebugLib
{
    public class BookController : ControllerBase
    {
        public void List()
        {
            context.Response.Write("That is list.");
        }
    }
}
