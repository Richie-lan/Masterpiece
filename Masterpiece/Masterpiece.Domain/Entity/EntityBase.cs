using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Masterpiece.Domain.Entity
{
    public abstract class EntityBase : IChangeTrack
    {
        private Dictionary<string,AuditTrail> changedFields { get; set; }

        private bool enableAudit = false;

        public EntityBase()
        {
            changedFields = new Dictionary<string, AuditTrail>();
        }

        public List<string> ChangedFields()
        {
            return changedFields.Where(x => x.Value.HasChanged()).Select(x => x.Key).ToList();
        }

        public void EnableAudit()
        {
            enableAudit = true;
        }

        public bool IsEnableAudit()
        {
            return enableAudit;
        }

        public void ReAudit()
        {
            this.enableAudit = true;
            foreach (KeyValuePair<string,AuditTrail> item in changedFields)
            {
                item.Value.OldValue = item.Value.NewValue;
            }
        }

        public void ForceAudit()
        {
            changedFields.Clear();
            this.enableAudit = true;
            PropertyInfo[] properties = this.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                object value = property.GetValue(this);
                changedFields.Add(property.Name, new AuditTrail(property.Name, value, value));
            }
        }
        
        protected void AuditCheck(string fieldName,object newValue)
        {
            AuditTrail trail = null;

            if(changedFields.ContainsKey(fieldName))
            {
                trail = changedFields[fieldName];
            }
            else
            {
                trail = new AuditTrail(fieldName, newValue, newValue);
                changedFields.Add(fieldName, trail);
            }

            if(enableAudit)
            {
                trail.NewValue = newValue;
            }
        }
    }
}
