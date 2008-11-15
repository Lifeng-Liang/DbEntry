namespace Lephone.Web.Rails
{
    public class LTArgs
    {
        public string Title;
        public string Controller;
        public string Action;
        public string Addon;

        public UTArgs ToUTArgs()
        {
            var ua = new UTArgs
            {
                Controller = Controller,
                Action = Action
            };
            return ua;
        }
    }
}
