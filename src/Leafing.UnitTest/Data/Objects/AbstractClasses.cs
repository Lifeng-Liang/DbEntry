using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Leafing.Data.Common;
using Leafing.Data.Definition;

namespace Leafing.UnitTest.Data.Objects
{
    public class AbstractClass : IDbObject
    {
        public string Name { get; set; }
    }

    [Serializable]
    public class SerializedBase : IDbObject
    {
        public virtual string Name { get; set; }
    }

    [Serializable]
    public class SerializedClass : SerializedBase, ISerializable
    {
        private string _name;
        public override string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            DynamicObjectReference.SerializeObject(this, info, context);
        }
    }

    public class AbstractClassOfAge : AbstractClass
    {
        public int Age { get; set; }
    }

    [DbTable("abc")]
    public class NamedClass : AbstractClass
    {
        public string Title { get; set; }
    }

    [Serializable]
    public class SerializableClass : AbstractClass
    {
    }

    public class AbstractClassWithOneImplProperty : AbstractClassOfAge
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
        public string Name { get { return _Name; } set { _Name = value; } }

        protected internal HasOne<ImpPCs> _pc;

        //[HasOne(OrderBy = "Id")]
		[Exclude]
        public ImpPCs pc { get { return _pc.Value; } set { _pc.Value = value; } }

        public ImpPeople()
        {
            _pc = new HasOne<ImpPCs>(this, "Id", "Person_Id");
        }
    }

    [DbTable("PCs")]
    public class ImpPCs : DbObjectModel<ImpPCs>
    {
        private string _Name;
        public string Name { get { return _Name; } set { _Name = value; } }

        [DbColumn("Person_Id")]
        protected internal BelongsTo<ImpPeople, long> _owner;

        //[BelongsTo, DbColumn("Person_Id")]
		[Exclude]
        public ImpPeople owner { get { return _owner.Value; } set { _owner.Value = value; } }

        public ImpPCs()
        {
            _owner = new BelongsTo<ImpPeople, long>(this, "Person_Id");
        }
    }

    [DbTable("People")]
    public class ImpPeople1 : DbObjectModel<ImpPeople1>
    {
        private string _Name;
        public string Name { get { return _Name; } set { _Name = value; } }

        protected internal HasMany<ImpPCs1> _pcs;

        //[HasMany(OrderBy = "Id DESC")]
		[Exclude]
        public IList<ImpPCs1> pcs { get { return _pcs; } set { } }

        public ImpPeople1()
        {
            _pcs = new HasMany<ImpPCs1>(this, "Id DESC", "Person_Id");
        }
    }

    [DbTable("PCs")]
    public class ImpPCs1 : DbObjectModel<ImpPCs1>
    {
        private string _Name;
        public string Name { get { return _Name; } set { _Name = value; } }

        [DbColumn("Person_Id")]
        protected internal BelongsTo<ImpPeople1, long> _owner;

        //[BelongsTo, DbColumn("Person_Id")]
		[Exclude]
        public ImpPeople1 owner { get { return _owner.Value; } set { _owner.Value = value; } }

        public ImpPCs1()
        {
            _owner = new BelongsTo<ImpPeople1, long>(this, "Person_Id");
        }
    }

    public class People : DbObjectModel<People>
    {
        public string Name { get; set; }

        //[HasOne(OrderBy = "Id")]
		public HasOne<PCs> pc { get; private set; }

		public People ()
		{
			pc = new HasOne<PCs> (this, "Id", "Person_Id");
		}
    }

    public class PCs : DbObjectModel<PCs>
    {
        public string Name { get; set; }

        [DbColumn("Person_Id")]
		public BelongsTo<People, long> owner { get; private set; }

		public PCs ()
		{
			owner = new BelongsTo<People, long> (this, "Person_Id");
		}
    }

    [DbTable("People")]
    public class People1 : DbObjectModel<People1>
    {
        public string Name { get; set; }

		public HasMany<PCs1> pcs { get; private set; }

		public People1 ()
		{
			pcs = new HasMany<PCs1> (this, "Id DESC", "Person_Id");
		}
    }

    [DbTable("PCs")]
    public class PCs1 : DbObjectModel<PCs1>
    {
        public string Name { get; set; }

        [DbColumn("Person_Id")]
		public BelongsTo<People1, long> owner { get; private set; }

		public PCs1 ()
		{
			owner = new BelongsTo<People1, long> (this, "Person_Id");
		}
    }

    [DbTable("Article")]
    public class DArticle : DbObjectModel<DArticle>
    {
        public string Name { get; set; }

		public HasAndBelongsToMany<DReader> readers { get; private set; }

		public DArticle ()
		{
			readers = new HasAndBelongsToMany<DReader> (this, "Id", "Article_Id");
		}
    }

    [DbTable("Reader")]
    public class DReader : DbObjectModel<DReader>
    {
        public string Name { get; set; }

		public HasAndBelongsToMany<DArticle> arts { get; private set; }

		public DReader ()
		{
			arts = new HasAndBelongsToMany<DArticle> (this, "Id", "Reader_Id");
		}
    }

    [DbTable("Article"), DbContext("SQLite")]
    public class DArticleSqlite : DbObjectModel<DArticleSqlite>
    {
        public string Name { get; set; }

		public HasAndBelongsToMany<DReaderSqlite> readers { get; private set; }

		public DArticleSqlite ()
		{
			readers = new HasAndBelongsToMany<DReaderSqlite> (this, "Id", "Article_Id");
		}
    }

    [DbTable("Reader"), DbContext("SQLite")]
    public class DReaderSqlite : DbObjectModel<DReaderSqlite>
    {
        public string Name { get; set; }

		public HasAndBelongsToMany<DArticleSqlite> arts { get; private set; }

		public DReaderSqlite ()
		{
			arts = new HasAndBelongsToMany<DArticleSqlite> (this, "Id", "Reader_Id");
		}
    }

    [DbTable("People")]
    public class PeopleImp1 : DbObjectModel<PeopleImp1>
    {
        public string Name { get; set; }

        public HasOne<PCsImp1> pc;

        public PeopleImp1()
        {
            pc = new HasOne<PCsImp1>(this, "Id", "Person_Id");
        }
    }

    [DbTable("PCs")]
    public class PCsImp1 : DbObjectModel<PCsImp1>
    {
        public string Name { get; set; }

        [DbColumn("Person_Id")]
        public BelongsTo<PeopleImp1, long> owner;

        public PCsImp1()
        {
            owner = new BelongsTo<PeopleImp1, long>(this, "Person_Id");
        }
    }

    [DbTable("People")]
    public class PeopleImp2 : DbObjectModel<PeopleImp2>
    {
        public string Name { get; set; }

        public HasOne<PCsImp2> pc;

        public PeopleImp2()
        {
            pc = new HasOne<PCsImp2>(this, "Id DESC", "Person_Id");
        }
    }

    [DbTable("PCs")]
    public class PCsImp2 : DbObjectModel<PCsImp2>
    {
        public string Name { get; set; }

        [DbColumn("Person_Id")]
        public BelongsTo<PeopleImp2, long> owner;

        public PCsImp2()
        {
            owner = new BelongsTo<PeopleImp2, long>(this, "Person_Id");
        }
    }

    [DbTable("People")]
    public class PeopleWith : DbObjectModel<PeopleWith>
    {
        public string Name { get; set; }
    }
}
