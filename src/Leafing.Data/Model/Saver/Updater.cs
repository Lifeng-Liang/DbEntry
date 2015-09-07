using System;
using Leafing.Data.Builder;
using Leafing.Data.Builder.Clause;
using System.Collections.Generic;
using Leafing.Data.Common;
using Leafing.Data.Model.Member;
using Leafing.Data.Definition;

namespace Leafing.Data
{
	[Serializable]
	public class Updater
	{
		public static Updater CreateUpdater()
		{
			return DataSettings.PartialUpdate ? new PartialUpdater() : new Updater();
		}

		public virtual void InitLoadedColumns(DbObjectSmartUpdate model) {
		}

		public bool FindUpdateColumns(DbObjectSmartUpdate model, UpdateStatementBuilder builder) {
			var autoList = new List<KeyOpValue> ();
			foreach (var m in model.Context.Info.Members) {
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
					object n = m.GetValue(model);
					ProcessSimpleMember(builder, m, n);
				} else if (m.Is.LazyLoad) {
					var ll = (ILazyLoading)m.GetValue (model);
					if (ll.IsLoaded) {
						var value = ll.Read ();
						var type = m.MemberType.GetGenericArguments () [0];
						ProcessLazyLoad(builder, m, value, type);
					}
				} else if (m.Is.BelongsTo) {
					var bt = (IBelongsTo)m.GetValue(model);
					var fk = bt.ForeignKey;
					ProcessBelongsTo(builder, m, fk);
				}
			}
			if (builder.Values.Count > 0) {
				builder.Values.AddRange (autoList);
				return true;
			}
			return false;
		}

		public virtual void ProcessSimpleMember(UpdateStatementBuilder builder, MemberHandler m, object n)
		{
			builder.Values.Add(new KeyOpValue(m.Name, n, m.MemberType));
		}

		public virtual void ProcessLazyLoad(UpdateStatementBuilder builder, MemberHandler m, object value, Type type)
		{
			builder.Values.Add (new KeyOpValue (m.Name, value, type));
		}

		public virtual void ProcessBelongsTo(UpdateStatementBuilder builder, MemberHandler m, object fk)
		{
			builder.Values.Add(new KeyOpValue(m.Name, fk, fk == null ? typeof(long) : fk.GetType()));
		}
	}
}
