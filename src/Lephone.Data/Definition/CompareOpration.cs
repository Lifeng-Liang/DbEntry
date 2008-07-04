using System;
using Lephone.Util.Text;

namespace Lephone.Data.Definition
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
