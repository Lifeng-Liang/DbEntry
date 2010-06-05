using Lephone.Util;

namespace Lephone.CodeGen
{
    public class ArgsErrorException : LephoneException
    {
        public readonly int ReturnCode;

        public ArgsErrorException(int returnCode, string message) : base(message)
        {
            this.ReturnCode = returnCode;
        }
    }
}
