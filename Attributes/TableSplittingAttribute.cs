using EFCorePartitioner.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCorePartitioner.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class TableSplittingAttribute : Attribute
    {
        // Enable Partition or not
        public bool EnablePartitioning { get; set; }

        // what strategy would be applied
        public PartitionStrategy Strategy { get; set; }

        // when the staregy is timebase
        public TimePartitionUnit PartitionUnit { get; set; }

        public TableSplittingAttribute()
        {
            EnablePartitioning = false;
            Strategy = PartitionStrategy.None;
            PartitionUnit = TimePartitionUnit.None;
        }
    }
}
