using System.Collections.Generic;

namespace eAndon.Models
{
    public class AndonTerminalModel
    {
        public string WorkcenterID { get; set; }
        public string WorkcenterName { get; set; }
        public List<string> StatusValues { get; set; }
    }

}
