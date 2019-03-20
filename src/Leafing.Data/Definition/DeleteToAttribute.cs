using System;

namespace Leafing.Data.Definition {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class DeleteToAttribute : Attribute {
        public string TableName;

        public DeleteToAttribute(string TableName) {
            this.TableName = TableName;
        }
    }
}