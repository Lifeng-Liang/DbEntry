using System;
using System.Collections.Generic;
using Leafing.Data.Definition;
using Leafing.Data.Builder.Clause;
using Leafing.Data.Builder;
using Leafing.Data.Model.Member;

namespace Leafing.Data {
    public class PartialUpdateHelper : UpdateHelper {
        private Dictionary<string, object> _LoadedColumns = new Dictionary<string, object>();

        public override void InitLoadedColumns(DbObjectSmartUpdate model) {
            _LoadedColumns = new Dictionary<string, object>();
            foreach (var m in model.Context.Info.Members) {
                if (m.Is.AutoSavedValue || m.Is.Key) {
                    continue;
                }
                if (m.Is.SimpleField) {
                    _LoadedColumns.Add(m.Name, m.GetValue(model));
                } else if (m.Is.LazyLoad) {
                    var ll = (ILazyLoading)m.GetValue(model);
                    if (ll.IsLoaded) {
                        _LoadedColumns.Add(m.Name, ll.Read());
                    }
                } else if (m.Is.BelongsTo) {
                    var bt = (IBelongsTo)m.GetValue(model);
                    _LoadedColumns.Add(m.Name, bt.ForeignKey);
                }
            }
        }

        private bool NotEqual(object v, object n) {
            if (v == null && n == null) {
                return false;
            }
            if (v == null) {
                return true;
            }
            return !v.Equals(n);
        }

        public override void ProcessSimpleMember(UpdateStatementBuilder builder, MemberHandler m, object n) {
            object v;
            if (_LoadedColumns.TryGetValue(m.Name, out v)) {
                if (NotEqual(v, n)) {
                    base.ProcessSimpleMember(builder, m, n);
                }
            }
        }

        public override void ProcessLazyLoad(UpdateStatementBuilder builder, MemberHandler m, object value, Type type) {
            object v;
            if (_LoadedColumns.TryGetValue(m.Name, out v)) {
                if (!NotEqual(value, v)) {
                    return;
                }
            }
            base.ProcessLazyLoad(builder, m, value, type);
        }

        public override void ProcessBelongsTo(UpdateStatementBuilder builder, MemberHandler m, object fk) {
            object v;
            if (_LoadedColumns.TryGetValue(m.Name, out v)) {
                if (NotEqual(v, fk)) {
                    base.ProcessBelongsTo(builder, m, fk);
                }
            }
        }
    }
}