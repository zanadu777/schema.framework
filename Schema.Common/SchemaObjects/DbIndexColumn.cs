using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schema.Common.SchemaObjects
{
   public class DbIndexColumn
    {
        public int Ordinal { get; set; }
        public string Name { get; set; }
        public bool IsDescending { get; set; }
    }
}
