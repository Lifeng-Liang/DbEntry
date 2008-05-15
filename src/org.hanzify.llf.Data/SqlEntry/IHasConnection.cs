namespace Lephone.Data.SqlEntry
{
    public interface IHasConnection
    {
        ConnectionContext ConnectionProvider { get; }
    }
}
