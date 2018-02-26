using Stormancer;

namespace Stormancer.Server.ReportPlayers
{
    public class App
    {
        public void Run(IAppBuilder builder)
        {
            builder.AddPlugin(new ReportPlayersPlugin());
        }
    }
}
