using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessorsSimulator
{
    class Task
    {
        public int id { get; set; }
        public int operationsAmont { get; set; }
        public int[] supportedProcessors { get; set; }
        public string getSupportedProcessors()
        {
            string str = "";
            foreach (int elem in supportedProcessors)
            {
                str = str + elem.ToString() + ",";
            }
            return str.Remove(str.Length - 1, 1); 
        }
    }
}
