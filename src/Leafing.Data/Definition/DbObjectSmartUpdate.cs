using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Leafing.Data.Builder;
using Leafing.Data.Builder.Clause;

namespace Leafing.Data.Definition
{
    [Serializable]
    [XmlRoot("DbObject")]
    public abstract class DbObjectSmartUpdate : DbObjectBase
    {
        [Exclude]
        internal abstract ModelContext Context { get; }

		private Dictionary<string, object> _LoadedColumns = new Dictionary<string, object>();

		internal protected void InitLoadedColumns()
		{
			_LoadedColumns = new Dictionary<string, object>();
			foreach (var m in Context.Info.Members) {
				if (m.Is.AutoSavedValue || m.Is.Key) {
					continue;
				}
				if (m.Is.SimpleField) {
					_LoadedColumns.Add (m.Name, m.GetValue (this));
				} else if (m.Is.LazyLoad) {
					var ll = (ILazyLoading)m.GetValue (this);
					if (ll.IsLoaded) {
						_LoadedColumns.Add (m.Name, ll.Read ());
					}
				} else if (m.Is.BelongsTo) {
					var bt = (IBelongsTo)m.GetValue (this);
					_LoadedColumns.Add (m.Name, bt.ForeignKey);
				}
			}
		}

		internal protected bool FindUpdateColumns(UpdateStatementBuilder builder)
		{
			var autoList = new List<KeyOpValue> ();
			foreach (var m in Context.Info.Members) {
				if (m.Is.DbGenerate || m.Is.HasOne || m.Is.HasMany || 
					m.Is.HasAndBelongsToMany || m.Is.CreatedOn || m.Is.Key) {
					continue;
				}
				if (m.Is.Count) {
					autoList.Add (new KeyOpValue(m.Name, 1, KvOpertation.Add));
					continue;
				}
				if (m.Is.UpdatedOn || m.Is.SavedOn) {
					autoList.Add (new KeyOpValue(m.Name, null, KvOpertation.Now));
					continue;
				}
				if (m.Is.SimpleField) {
					object v;
					if (_LoadedColumns.TryGetValue (m.Name, out v)) {
						object n = m.GetValue(this);
						if (NotEqual (v, n)) {
							builder.Values.Add (new KeyOpValue (m.Name, n, m.MemberType));
						}
					}
				} else if (m.Is.LazyLoad) {
					var ll = (ILazyLoading)m.GetValue (this);
					if (ll.IsLoaded) {
						var value = ll.Read ();
						var type = m.MemberType.GetGenericArguments () [0];
						object v;
						if(_LoadedColumns.TryGetValue(m.Name, out v)) {
							if(!NotEqual(value, v)) {
								continue;
							}
						}
						builder.Values.Add (new KeyOpValue (m.Name, ll.Read(), type));
					}
				} else if (m.Is.BelongsTo) {
					object v;
					if (_LoadedColumns.TryGetValue (m.Name, out v)) {
						var bt = (IBelongsTo)m.GetValue (this);
						var fk = bt.ForeignKey;
						if (NotEqual (v, fk)) {
							builder.Values.Add (new KeyOpValue (m.Name, fk, fk == null ? typeof(long) : fk.GetType()));
						}
					}
				}
			}
			if (builder.Values.Count > 0) {
				builder.Values.AddRange (autoList);
				return true;
			}
			return false;
		}

		private bool NotEqual(object v, object n)
		{
			if (v == null && n == null) {
				return false;
			}
			if (v == null) {
				return true;
			}
			return !v.Equals (n);
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
