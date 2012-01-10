using Leafing.Core;

namespace Leafing.CodeGen
{
    public class ArgsErrorException : LeafingException
    {
        public readonly int ReturnCode;

        public ArgsErrorException(int returnCode, string message) : base(message)
        {
            this.ReturnCode = returnCode;
        }
    }
}
