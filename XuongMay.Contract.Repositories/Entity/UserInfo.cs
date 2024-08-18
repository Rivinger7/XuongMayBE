using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XuongMay.Core.Base;

namespace XuongMay.Contract.Repositories.Entity
{
    public class UserInfo : BaseEntity
    {
        public string FullName { get; set; } = string.Empty;
        public string? BankAccount { get; set; }
        public string? BankAccountName { get; set; }
        public string? Bank { get; set; }
    }
}
