
#region usings

using System;
using org.hanzify.llf.util.Text;

#endregion

namespace org.hanzify.llf.Data.Definition
{
	[Serializable]
	public enum CompareOpration
	{
		[ShowString(">")]		GreatThan,
		[ShowString("<")]		LessThan,
		[ShowString("=")]		Equal,
		[ShowString(">=")]		GreatOrEqual,
		[ShowString("<=")]		LessOrEqual,
		[ShowString("<>")]		NotEqual,
		[ShowString("Like")]	Like,
		[ShowString("Is")]		Is,
        [ShowString("Is Not")]  IsNot
	}
}
