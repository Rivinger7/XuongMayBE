using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XuongMay.ModelViews.UserModelViews
{
    public class UserSummaryModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Role { get; set; } = null!;
    }
}
