
using System;
using System.IO;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using org.hanzify.llf.Data;
using org.hanzify.llf.util;

public partial class _Default : System.Web.UI.Page 
{
    protected DataSourceControl myDataSource = new DbEntryDataSource<User>();

    protected void Page_Load(object sender, EventArgs e)
    {
        InitDatabase();
        if (!IsPostBack)
        {
            GridView1.DataSource = myDataSource;
            GridView1.DataBind();
        }
    }

    protected void InitDatabase()
    {
        if (File.Exists(SystemHelper.BaseDirectory + @"App_Data\test.db")) { return; }

        DbEntry.UsingTransaction(delegate()
        {
            global::User.New("tom", 19, DateTime.Now, Gender.Male).Save();
            global::User.New("jerry", 26, DateTime.Now, Gender.Male).Save();
            global::User.New("mike", 21, DateTime.Now, Gender.Male).Save();
            global::User.New("rose", 17, DateTime.Now, Gender.Female).Save();
            global::User.New("alice", 16, DateTime.Now, Gender.Female).Save();

            global::User.New("peter", 41, DateTime.Now, Gender.Male).Save();
            global::User.New("vito", 28, DateTime.Now, Gender.Male).Save();
            global::User.New("jeff", 23, DateTime.Now, Gender.Male).Save();
            global::User.New("kate", 22, DateTime.Now, Gender.Female).Save();
            global::User.New("july", 25, DateTime.Now, Gender.Female).Save();

            global::User.New("lephone", 31, DateTime.Now, Gender.Male).Save();
            global::User.New("juan", 25, DateTime.Now, Gender.Female).Save();
        });
    }
}
