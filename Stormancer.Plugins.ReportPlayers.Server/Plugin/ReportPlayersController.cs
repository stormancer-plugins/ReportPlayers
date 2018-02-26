using Server.Plugins.API;
using Server.Users;
using Stormancer;
using Stormancer.Diagnostics;
using Stormancer.Plugins;
using System;
using System.Threading.Tasks;

namespace Stormancer.Server.ReportPlayers
{
    class ReportPlayersController : ControllerBase
    {
        private const string _logCategory = "ReportPlayerController";
        private readonly IReportPlayersService _reportPlayers;
        private readonly ILogger _logger;
        private readonly IUserSessions _userSessions;

        public ReportPlayersController(IReportPlayersService reportService, IUserSessions userSessions, ILogger logger)
        {
            _reportPlayers = reportService;
            _logger = logger;
            _userSessions = userSessions;
        }

        public async Task Report(RequestContext<IScenePeerClient> ctx)
        {
            try
            {
                var user = await _userSessions.GetUser(ctx.RemotePeer);
                var reportDto = ctx.ReadObject<ReportDto>();
                var report = new Report
                {
                    ReportUserId = user.Id,
                    ReportedUserId = reportDto.ReportedUserId,
                    ReportDate = DateTimeOffset.FromUnixTimeSeconds(reportDto.Timestamp).DateTime,
                    Message = reportDto.Message,
                    Category = reportDto.Category,
                    CustomData = reportDto.CustomData,                
                };            
                await _reportPlayers.ReportPlayer(report);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, _logCategory, "Server error, report doesn't save", ex.Message);
                throw new ClientException("Server error, report doesn't save");
            }

            ctx.SendValue<string>("Report saved");
        }
    }
}
