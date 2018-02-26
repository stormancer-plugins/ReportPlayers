using System;
using System.Threading.Tasks;
using Stormancer.Diagnostics;
using Server.Database;
using Server.Plugins.Configuration;
using Stormancer.Server.Components;
using System.Collections.Generic;
using System.Linq;
using Stormancer;
using System.Collections.Concurrent;
using Newtonsoft.Json.Linq;

namespace Stormancer.Server.ReportPlayers
{
    class ESReportRepository : IReportRepository
    {
        private const string LOG_CATEGORY = "ESChatLogRepository";
        private const string TABLE_NAME = "report";
        private readonly IConfiguration _conf;
        private readonly IESClientFactory _ESFactory;
        private readonly ILogger _log;
        private readonly IEnvironment _env;

        public ESReportRepository(
            IEnvironment env,
            IConfiguration conf,
            IESClientFactory ESFactory,
            ILogger log)
        {
            _conf = conf;
            _ESFactory = ESFactory;
            _log = log;
            _env = env;
        }

        private Task CreateReportIndicMapping(Nest.IElasticClient esClient)
        {
            return esClient.MapAsync<Report>(m =>
                m.Properties(pd =>
                    pd.Object<JObject>(otd => otd.Name(r => r.CustomData).Dynamic(false))
                )
            );
        }

        private static readonly ConcurrentDictionary<string, bool> _initializedIndices = new ConcurrentDictionary<string, bool>();
        private async Task<Nest.IElasticClient> CreateESClient<T>(string name = "")
        {
            var result = await _ESFactory.CreateClient<T>(TABLE_NAME, name);

            if (_initializedIndices.TryAdd(TABLE_NAME, true))
            {
                await CreateReportIndicMapping(result);
            }
            return result;
        }

        private List<string> GetIndexes<T>(IEnumerable<string> types)
        {
            List<string> indexesPool = new List<string>();

            foreach (string t in types)
            {
                indexesPool.Add(_ESFactory.GetIndex<T>(t));
            }

            return indexesPool;
        }

        public async Task SaveReport(Report report, string type)
        {
            var esClient = await CreateESClient<Report>(type);
                       
            var result = await esClient.IndexAsync(report, _=>_);

            if (!result.IsValid)
            {
                _log.Log(LogLevel.Error, LOG_CATEGORY, "Error occurred, report can't be indexed in database", result.OriginalException);
                throw new ClientException("Error occurred, report can't be indexed in database");
            }
        }

        public async Task<List<Report>> ESQuery(DateTime start, DateTime end, string[] types)
        {
            var esClient = await CreateESClient<Report>();
            var indexes = GetIndexes<Report>(types);
            List<Report> reports = new List<Report>();

            var scanResult = await esClient.SearchAsync<Report>(s => s
                .Query(q => q
                    .MatchAll()
                )
                .Index(Nest.Indices.Index(indexes))
                .Scroll("1s")
                .Size(50)
                .IgnoreUnavailable()
            );

            if (!scanResult.IsValid)
            {
                _log.Log(LogLevel.Error, LOG_CATEGORY, $"An error occured when the server try to request database", scanResult.OriginalException);
                throw new ClientException("Database error please check server log");
            }

            reports.AddRange(scanResult.Documents);
            while (scanResult.Documents.Any())
            {
                scanResult = await esClient.ScrollAsync<Report>("1s", scanResult.ScrollId);
                reports.AddRange(scanResult.Documents);
            }

            await esClient.ClearScrollAsync(d => d.ScrollId(scanResult.ScrollId), default(System.Threading.CancellationToken));

            return reports.Where((x) =>
            {
                return x.ReportDate >= start && x.ReportDate <= end;
            }).ToList<Report>();
        }
    }
}
