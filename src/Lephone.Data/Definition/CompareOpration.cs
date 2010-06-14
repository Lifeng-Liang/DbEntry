using System;
using Lephone.Core.Text;

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
		[ShowString("LIKE")]	Like,
		[ShowString("IS")]		Is,
        [ShowString("IS NOT")]  IsNot
	}
}
