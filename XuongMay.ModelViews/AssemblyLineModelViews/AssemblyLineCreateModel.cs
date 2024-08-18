using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XuongMay.ModelViews.AssemblyLineModelViews
{
    public class AssemblyLineCreateModel
    {
        public required int ManagerID { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required int NumberOfStaffs { get; set; }
    }
}
