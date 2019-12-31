using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masterpiece.Domain.Entity
{
    [Table("Product")]
    public class Product:EntityBase
    {
        [Key]
        public int Id { get; set; }

        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                AuditCheck("Name", value);
                name = value;
            }
        }

        private int? age;
        public int? Age
        {
            get
            {
                return age;
            }
            set
            {
                AuditCheck("Age", value);
                age = value;
            }
        }

        private DateTime? createTime;
        public DateTime? CreateTime
        {
            get
            {
                return createTime;
            }
            set
            {
                AuditCheck("CreateTime", value);
                createTime = value;
            }
        }
    }
}
