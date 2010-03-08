namespace Lephone.Data.Definition
{
    public interface ILazyLoading
    {
        object Read();
        void Write(object item, bool isLoad);
        void Init(string foreignKeyName);
        void Load();
        bool IsLoaded { get; set; }
    }
}
