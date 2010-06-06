using System;
using Lephone.Data.Definition;

namespace DebugLib.Models
{
    public enum BookCatagory
    {
        Manager,
        Computer,
        Story,
    }

    public class Book : DbObjectModel<Book>
    {
        [Length(30)] public string Name { get; set; }
        public Date BuyDate { get; set; }
        public float Price { get; set; }
        public BookCatagory Catagory { get; set; }
        public bool Read { get; set; }
        [SpecialName] public DateTime CreatedOn { get; set; }
        [SpecialName] public DateTime? UpdatedOn { get; set; }

        [BelongsTo]
        public SysUser User { get; set; }
    }
}
