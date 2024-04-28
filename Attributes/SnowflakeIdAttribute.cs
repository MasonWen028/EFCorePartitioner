using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCorePartitioner.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SnowflakeIdAttribute : Attribute
    {
        // Indicates whether the property should be treated as a primary key.
        public bool IsPrimaryKey { get; }

        // Constructor to set whether the property is a primary key.
        public SnowflakeIdAttribute(bool isPrimaryKey = false)
        {
            IsPrimaryKey = isPrimaryKey;
        }
    }
}
