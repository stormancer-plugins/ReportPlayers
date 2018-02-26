using Newtonsoft.Json.Linq;
using System;

namespace Stormancer.Server.ReportPlayers
{
    public class Report
    {        
        public string ReportUserId { get; set; }
        public string ReportedUserId { get; set; }
        public string Category { get; set; }
        public string Message { get; set; }
        public DateTime ReportDate { get; set; }        
        public JObject CustomData { get; set; }
    }
}