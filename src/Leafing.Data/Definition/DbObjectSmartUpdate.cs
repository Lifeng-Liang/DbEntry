using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Leafing.Data.Definition
{
    [Serializable]
    [XmlRoot("DbObject")]
    public abstract class DbObjectSmartUpdate : DbObjectBase
    {
        [Exclude]
        internal abstract ModelContext Context { get; }

        [Exclude]
        internal protected Dictionary<string, object> m_UpdateColumns;

        [Exclude]
        internal bool m_InternalInit;

        internal protected void m_InitUpdateColumns()
        {
            m_UpdateColumns = new Dictionary<string, object>();
        }

        protected internal void m_ColumnUpdated(string columnName)
        {
            if (m_UpdateColumns != null && !m_InternalInit)
            {
                m_UpdateColumns[columnName] = 1;
            }
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
