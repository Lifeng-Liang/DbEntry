namespace Lephone.MockSql.Recorder
{
    public interface IRecorder
    {
        void Write(string msg, params object[] os);
    }
}
