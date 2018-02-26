using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stormancer.Server.ReportPlayers
{
    public interface IReportPlayersService
    {
        Task ReportPlayer(Report report);
        Task<List<Report>> SeekReport(DateTime start, DateTime end, string[] types);
    }
}
