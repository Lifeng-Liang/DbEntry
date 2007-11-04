
using System;
using System.Collections.Generic;
using System.Text;
using Lephone.Data.Definition;

namespace DebugLib.Models
{
    public enum BookCatagory
    {
        Manager,
        Computer,
        Story,
    }

    public abstract class Book : DbObjectModel<Book>
    {
        [Length(30)] public abstract string Name { get; set; }
        public abstract DateTime BuyDate { get; set; }
        public abstract float Price { get; set; }
        public abstract BookCatagory Catagory { get; set; }
        public abstract bool Read { get; set; }
        [SpecialName] public abstract DateTime CreatedOn { get; set; }
        [SpecialName] public abstract DateTime? UpdatedOn { get; set; }
    }
}
