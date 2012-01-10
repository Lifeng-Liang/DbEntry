namespace Leafing.Data.Model.Member
{
    public interface IMemberHandler
    {
        void SetValue(object obj, object value);
        object GetValue(object obj);
    }
}
