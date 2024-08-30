using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XuongMay.ModelViews.ChamberModelViews
{
    public class ChamberProductResponseModel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int ItemPerBox { get; set; }
        public string CreatedTime = string.Empty;
    }
}
