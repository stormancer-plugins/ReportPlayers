using Stormancer.Diagnostics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Server.Helpers;

namespace Stormancer.Server.ReportPlayers
{
    public class ReportPlayersAdminController : ApiController
    {
        private const string _logCategory = "ReportPlayersAdminController";
        private readonly IReportPlayersService _reportPlayers;
        private readonly ILogger _logger;

        public ReportPlayersAdminController(IReportPlayersService reportPlayers, ILogger log)
        {
            _reportPlayers = reportPlayers;
            _logger = log;
        }

        [HttpGet]
        public async Task<List<Report>> SeekReportHistory(string types, string dateStart, string dateEnd)
        {
            string[] splittedType = types.Split('-');
            List<Report> reports = new List<Report>();
            try
            {
                DateTime start = DateTime.ParseExact(dateStart, "ddMMyyyyHH:mm:ss", CultureInfo.InvariantCulture);
                DateTime end = DateTime.ParseExact(dateEnd, "ddMMyyyyHH:mm:ss", CultureInfo.InvariantCulture);
                reports = await _reportPlayers.SeekReport(start, end, splittedType);
            }
            catch (ArgumentNullException argumentEx)
            {
                throw HttpHelper.HttpError(System.Net.HttpStatusCode.BadRequest, "Some argument are null");
            }
            catch (FormatException formatEx)
            {
                throw HttpHelper.HttpError(System.Net.HttpStatusCode.BadRequest, "Date format not supported");
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, _logCategory, "An error occured when seek history in chat repository", ex.Message);
                throw HttpHelper.HttpError(System.Net.HttpStatusCode.InternalServerError, "Server Internal error(s)");
            }

            if (reports.Count == 0)
            {
                throw HttpHelper.HttpError(System.Net.HttpStatusCode.NoContent, $"No data found in request date range DateStart={dateStart}, DateEnd={dateEnd}");
            }
            else
            {
                return reports;
            }
        }

    }
}
