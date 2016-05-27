using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using CheckmarxReports.CxWsResolver;

namespace CheckmarxReports
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ReportFactory reportFactory = new ReportFactory("checkmarx.gmgmt.dimensiondata.com", "username", "password");
            reportFactory.Run();
        }
    }
}
