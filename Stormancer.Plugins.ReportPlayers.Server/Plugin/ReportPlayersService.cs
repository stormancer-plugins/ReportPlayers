using Stormancer.Diagnostics;
using Server.Plugins.Configuration;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace Stormancer.Server.ReportPlayers
{    
    class ReportPlayersService : IReportPlayersService
    {              
        private readonly ILogger _logger;
        private readonly IReportRepository _reportRepo;
  
        public ReportPlayersService(
            ILogger logger,
            IReportRepository reportRepo,
            IConfiguration configuration)
        {
            _logger = logger;
            _reportRepo = reportRepo;                    
        }

        public async Task ReportPlayer(Report report)
        {    
            await _reportRepo.SaveReport(report, report.Category);
        }

        public Task<List<Report>> SeekReport(DateTime start, DateTime end, string[] types)
        {            
            if(end < start)
            {
                var temp = start;
                start = end;
                end = temp;
            }
            return _reportRepo.ESQuery(start, end, types);
        }
        
    }
}