using Server.Plugins.ReportPlayers;
using Stormancer.Core;


namespace Stormancer.Server.ReportPlayers
{
    public static class ReportPlayersExtensions
    {
        public static void AddReportPlayers(this ISceneHost scene)
        {
            scene.Metadata[ReportPlayersPlugin.METADATA_KEY] = "enabled";
        }
    }
}
