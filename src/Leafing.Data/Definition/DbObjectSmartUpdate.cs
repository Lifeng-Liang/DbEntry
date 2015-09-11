using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Leafing.Data.Builder;
using Leafing.Data.Builder.Clause;
using Leafing.Data.Common;

namespace Leafing.Data.Definition
{
    [Serializable]
    [XmlRoot("DbObject")]
    public abstract class DbObjectSmartUpdate : DbObjectBase
    {
		[Exclude]
        internal abstract ModelContext Context { get; }

		[NonSerialized]
		private UpdateHelper _updateHelper = UpdateHelper.CreateHelper();

		internal protected void InitLoadedColumns()
		{
			_updateHelper.InitLoadedColumns(this);
		}

		internal protected bool FindUpdateColumns(UpdateStatementBuilder builder)
		{
			return _updateHelper.FindUpdateColumns(this, builder);
		}

        public void Save()
        {
            Context.Operator.Save(this);
        }

        internal void RaiseInserting()
        {
            OnInserting();
        }

        protected virtual void OnInserting()
        {
        }

        internal void RaiseUpdating()
        {
            OnUpdating();
        }

        protected virtual void OnUpdating()
        {
        }

        public void Delete()
        {
            RaiseDeleting();
        }

        internal void RaiseDeleting()
        {
            OnDeleting();
            Context.Operator.Delete(this);
        }

        protected virtual void OnDeleting()
        {
        }

        public virtual ValidateHandler Validate()
        {
            var v = new ValidateHandler();
            v.ValidateObject(this);
            return v;
        }

        public virtual bool IsValid()
        {
            return Validate().IsValid;
        }
    }
}
