using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XuongMay.ModelViews.AssemblyLineModelView
{
    public class AssemblyLineModelView
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required int ManagerID { get; set; }
        public required string Description { get; set; }
        public required int NumberOfStaffs { get; set; }
        public required string CreatedBy { get; set; }
        public required DateTime CreatedTime { get; set; }
        public DateTime? LastUpdatedTime { get; set; }
    }
}
