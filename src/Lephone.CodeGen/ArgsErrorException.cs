using Lephone.Util;

namespace Lephone.CodeGen
{
    public class ArgsErrorException : LephoneException
    {
        public readonly int ReturnCode;

        public ArgsErrorException(int ReturnCode, string message) : base(message)
        {
            this.ReturnCode = ReturnCode;
        }
    }
}
