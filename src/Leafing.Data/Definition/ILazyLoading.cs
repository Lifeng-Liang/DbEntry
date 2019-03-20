namespace Leafing.Data.Definition {
    public interface ILazyLoading {
        object Read();
        void Write(object item, bool isLoad);
        void Load();
        bool IsLoaded { get; set; }
    }
}
