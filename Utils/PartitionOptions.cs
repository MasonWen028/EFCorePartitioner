using EFCorePartitioner.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCorePartitioner.Utils
{
    public class PartitionOptions
    {
        public bool EnableSharding { get; set; }
        public IPartitionStrategy PartitionStrategy { get; set; }
        public string TableNameFormat { get; set; }
        public int TableCount { get; set; }
    }
}
