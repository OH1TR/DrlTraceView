using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrlTraceView
{
    public class APICall
    {
        public int Pid { get; set; }
        public string Name { get; set; }
        public List<APICallParameter> Parameters { get; set; }
    }
}
