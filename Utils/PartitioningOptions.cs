using EFCorePartitioner.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCorePartitioner.Utils
{
    public class PartitioningOptions
    {
        public bool EnablePartitioning { get; set; } = false;
        public IPartitionStrategy PartitionStrategy { get; set; }
    }

}
