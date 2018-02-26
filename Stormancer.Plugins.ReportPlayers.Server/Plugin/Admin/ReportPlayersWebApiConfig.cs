using Server.Plugins.AdminApi;
using System.Web.Http;

namespace Stormancer.Server.ReportPlayers
{
    class ReportPlayersWebApiConfig : IAdminWebApiConfig
    {
        public void Configure(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute("report","_report/{types}", new {Controller = "ReportPlayersAdmin"});           
        }
    }
}
