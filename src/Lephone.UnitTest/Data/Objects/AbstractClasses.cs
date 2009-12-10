using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Lephone.Data.Common;
using Lephone.Data.Definition;

namespace Lephone.UnitTest.Data.Objects
{
    public abstract class AbstractClass : IDbObject
    {
        public abstract string Name { get; set; }

        public AbstractClass() { }

        public AbstractClass(string Name)
        {
            this.Name = Name;
        }
    }

    [Serializable]
    public abstract class SerializedBase : IDbObject
    {
        public abstract string Name { get; set; }

        public SerializedBase() { }

        public SerializedBase(string Name)
        {
            this.Name = Name;
        }
    }

    [Serializable]
    public class SerializedClass : SerializedBase, ISerializable
    {
        private string _Name;
        public override string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            DynamicObjectReference.SerializeObject(this, info, context);
        }
    }

    public abstract class ConstructorBase : IDbObject
    {
        public int p4;
        public bool p5;
        public DateTime p6;

        public string p1;
        public int p2;
        public string p3;

        public ConstructorBase() { }

        public ConstructorBase(string p1, int p2, string p3, int p4, bool p5, DateTime p6)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
            this.p4 = p4;
            this.p5 = p5;
            this.p6 = p6;
        }
    }

    public class TestConstructor : ConstructorBase
    {
        public TestConstructor(string p1, int p2, string p3, int p4, bool p5, DateTime p6)
            : base(p1, p2, p3, p4, p5, p6)
        {
        }
    }

    public abstract class AbstractClassOfAge : AbstractClass
    {
        public abstract int Age { get; set; }
    }

    [DbTable("abc")]
    public abstract class NamedClass : AbstractClass
    {
        public abstract string Title { get; set; }
    }

    [JoinOn(1, typeof(NamedClass), "a1", typeof(AbstractClassOfAge), "a2", CompareOpration.LessOrEqual, JoinMode.Left)]
    [JoinOn(2, typeof(NamedClass), "b1", typeof(SerializableClass), "b2", CompareOpration.Like, JoinMode.Right)]
    public abstract class JoinClass : AbstractClass
    {
    }

    [Serializable]
    public abstract class SerializableClass : AbstractClass
    {
    }

    public abstract class AbstractClassWithOneImplProperty : AbstractClassOfAge
    {
        public bool Gender
        {
            set
            {
            }
            get
            {
                return false;
            }
        }
    }

    [DbTable("People")]
    public class ImpPeople : DbObjectModel<ImpPeople>
    {
        private string _Name;
        public string Name { get { return _Name; } set { _Name = value; m_ColumnUpdated("Name"); } }

        protected internal HasOne<ImpPCs> _pc;

        [HasOne(OrderBy = "Id")]
        public ImpPCs pc { get { return _pc.Value; } set { _pc.Value = value; } }

        public ImpPeople()
        {
            _pc = new HasOne<ImpPCs>(this, "Id");
            m_InitUpdateColumns();
        }
    }

    [DbTable("PCs")]
    public class ImpPCs : DbObjectModel<ImpPCs>
    {
        private string _Name;
        public string Name { get { return _Name; } set { _Name = value; m_ColumnUpdated("Name"); } }

        [DbColumn("Person_Id")]
        protected internal BelongsTo<ImpPeople> _owner;

        [BelongsTo, DbColumn("Person_Id")]
        public ImpPeople owner { get { return _owner.Value; } set { _owner.Value = value; } }

        public ImpPCs()
        {
            _owner = new BelongsTo<ImpPeople>(this);
            m_InitUpdateColumns();
        }
    }

    [DbTable("People")]
    public class ImpPeople1 : DbObjectModel<ImpPeople1>
    {
        private string _Name;
        public string Name { get { return _Name; } set { _Name = value; m_ColumnUpdated("Name"); } }

        protected internal HasMany<ImpPCs1> _pcs;

        [HasMany(OrderBy = "Id DESC")]
        public IList<ImpPCs1> pcs { get { return _pcs; } set { } }

        public ImpPeople1()
        {
            _pcs = new HasMany<ImpPCs1>(this, "Id DESC");
            m_InitUpdateColumns();
        }
    }

    [DbTable("PCs")]
    public abstract class ImpPCs1 : DbObjectModel<ImpPCs1>
    {
        private string _Name;
        public string Name { get { return _Name; } set { _Name = value; m_ColumnUpdated("Name"); } }

        [DbColumn("Person_Id")]
        protected internal BelongsTo<ImpPeople1> _owner;

        [BelongsTo, DbColumn("Person_Id")]
        public ImpPeople1 owner { get { return _owner.Value; } set { _owner.Value = value; } }

        public ImpPCs1()
        {
            _owner = new BelongsTo<ImpPeople1>(this);
            m_InitUpdateColumns();
        }
    }

    public abstract class People : DbObjectModel<People>
    {
        public abstract string Name { get; set; }

        [HasOne(OrderBy = "Id")]
        public abstract PCs pc { get; set; }
    }

    public abstract class PCs : DbObjectModel<PCs>
    {
        public abstract string Name { get; set; }

        [BelongsTo, DbColumn("Person_Id")]
        public abstract People owner { get; set; }
    }

    [DbTable("People")]
    public abstract class People1 : DbObjectModel<People1>
    {
        public abstract string Name { get; set; }

        [HasMany(OrderBy = "Id DESC")]
        public abstract IList<PCs1> pcs { get; set; }
    }

    [DbTable("PCs")]
    public abstract class PCs1 : DbObjectModel<PCs1>
    {
        public abstract string Name { get; set; }

        [BelongsTo, DbColumn("Person_Id")]
        public abstract People1 owner { get; set; }
    }

    [DbTable("Article")]
    public abstract class DArticle : DbObjectModel<DArticle>
    {
        public abstract string Name { get; set; }

        [HasAndBelongsToMany(OrderBy = "Id")]
        public abstract IList<DReader> readers { get; set; }
    }

    [DbTable("Reader")]
    public abstract class DReader : DbObjectModel<DReader>
    {
        public abstract string Name { get; set; }

        [HasAndBelongsToMany(OrderBy = "Id")]
        public abstract IList<DArticle> arts { get; set; }
    }

    [DbTable("People")]
    public class PeopleImp1 : DbObjectModel<PeopleImp1>
    {
        public string Name;

        public HasOne<PCsImp1> pc;

        public PeopleImp1()
        {
            pc = new HasOne<PCsImp1>(this, "Id");
        }
    }

    [DbTable("PCs")]
    public class PCsImp1 : DbObjectModel<PCsImp1>
    {
        public string Name;

        [DbColumn("Person_Id")]
        public BelongsTo<PeopleImp1> owner;

        public PCsImp1()
        {
            owner = new BelongsTo<PeopleImp1>(this);
        }
    }

    [DbTable("People")]
    public class PeopleImp2 : DbObjectModel<PeopleImp2>
    {
        public string Name;

        public HasOne<PCsImp2> pc;

        public PeopleImp2()
        {
            pc = new HasOne<PCsImp2>(this, "Id DESC");
        }
    }

    [DbTable("PCs")]
    public class PCsImp2 : DbObjectModel<PCsImp2>
    {
        public string Name;

        [DbColumn("Person_Id")]
        public BelongsTo<PeopleImp2> owner;

        public PCsImp2()
        {
            owner = new BelongsTo<PeopleImp2>(this);
        }
    }

    [DbTable("People")]
    public abstract class PeopleWith : DbObjectModel<PeopleWith>
    {
        public abstract string Name { get; set; }

        public Dictionary<string, object> GetUpdateColumns()
        {
            return m_UpdateColumns;
        }
    }
}
