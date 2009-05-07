using System.Data;

namespace Lephone.Data.Definition
{
    public interface IDataReaderInitalize
    {
        IDataReaderInitalize Initalize(IDataReader dr, int startIndex);
        int FieldCount { get; }
    }
}
