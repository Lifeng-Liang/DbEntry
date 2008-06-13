namespace Lephone.MockSql.Recorder
{
    public interface IRecorder
    {
        void Write(string Msg, params object[] os);
    }
}
