using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSalary
{
    public class SimpleSalaryConfiguration : IRocketPluginConfiguration
    {
        public HashSet<SalaryInfo> SalaryInfoList;
        public int SalaryPayoutInterval;
        public void LoadDefaults()
        {
            SalaryPayoutInterval = 5;
            SalaryInfoList = new HashSet<SalaryInfo>
            {
               new SalaryInfo { PermissionsGroupId = "default", SalaryPayout = 1000 },
               new SalaryInfo { PermissionsGroupId = "police", SalaryPayout = 5000 }
            };
        }

        public class SalaryInfo
        {
            public string PermissionsGroupId { get; set; }
            public uint SalaryPayout { get; set; }
        }
    }
}
