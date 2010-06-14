using System.Dynamic;
using Lephone.Data.Definition;
using Lephone.Core.Text;

namespace Lephone.Data.Common
{
    public class DynamicQuery<T> : DynamicObject where T : class, IDbObject
    {
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            var ss = StringHelper.SplitByCase(binder.Name);
            if(ss.Count % 2 == 0)
            {
                throw new DataException("FindBy need Cola(AndColb) format.");
            }
            if (((ss.Count + 1) >> 1) != args.Length)
            {
                throw new DataException("The args count doesn't match method call " + binder.Name + "");
            }
            Condition c = null;
            int n = 0;
            for (int i = 0; i < ss.Count; i += 2)
            {
                c &= CK<T>.Field[ss[i]] == args[n++];
                if (i + 1 < ss.Count)
                {
                    if (ss[i + 1] != "And")
                    {
                        throw new DataException("FindBy need Cola(AndColb) format.");
                    }
                }
            }
            result = DbEntry.GetObject<T>(c);
            return true;
        }
    }
}
