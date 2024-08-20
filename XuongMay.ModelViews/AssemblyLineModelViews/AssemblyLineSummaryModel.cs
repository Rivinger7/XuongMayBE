using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XuongMay.ModelViews.AssemblyLineModelViews
{
    public class AssemblyLineSummaryModel
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string ManagerFullName { get; set; }
        public required int NumberOfStaffs { get; set; }
        public required string CreatedBy { get; set; }
    }
}
