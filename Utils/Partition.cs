using EFCorePartitioner.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCorePartitioner.Utils
{
    /// <summary>
    /// Represents information about a partitioned entity, including the entity type, partition strategy, and table name format.
    /// </summary>
    public class Partition
    {
        /// <summary>
        /// The type of the entity to be partitioned.
        /// </summary>
        public Type EntityType { get; set; }

        /// <summary>
        /// The partition strategy used to determine the table name for the entity.
        /// </summary>
        public IPartitionStrategy PartitionStrategy { get; set; }

        /// <summary>
        /// The format string used to generate the table name for the entity.
        /// </summary>
        public string TableNameFormat { get; set; }
    }


}
