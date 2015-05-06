Http Parameter
==========

Http Parameter is a custom attribute for web page to make the progress of parse http parameter easier. The following is the sample to show how to use it:

````c#
using Leafing.Web;

protected partial class Edit : SmartPageBase
{
    [HttpParameter] private string title;
    [HttpParameter(AllowEmpty = true)] private string path;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            var article = Article.FindByTitle(title);
            if(article != null)
            {
                Editor.Value = article.Content;
            }
        }
    }
}
````

Note the page must inherits from Leafing.Web.SmartPageBase. And there is a SmartMasterPageBase for master page too.

The default value of AllowEmpty is false. It will raise exception if can't find the parameter or it's empty.

If the AllowEmpty equals true, it will try to set the field to the default value if can't find the parameter or it's empty.

