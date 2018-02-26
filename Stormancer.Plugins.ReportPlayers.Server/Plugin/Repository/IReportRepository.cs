using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stormancer.Server.ReportPlayers
{
    interface IReportRepository
    {
        Task SaveReport(Report report, string type);
        Task<List<Report>> ESQuery(DateTime start, DateTime end, string[] types);        
    }
}
