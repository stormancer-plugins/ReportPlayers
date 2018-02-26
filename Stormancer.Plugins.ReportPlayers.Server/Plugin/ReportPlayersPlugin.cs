using Stormancer.Core;
using Stormancer.Plugins;
using Stormancer;
using Server.Plugins.AdminApi;

namespace Stormancer.Server.ReportPlayers
{
    class ReportPlayersPlugin : IHostPlugin
    {
        internal const string METADATA_KEY = "stormancer.reportplayers";

        public void Build(HostPluginBuildContext ctx)
        {
            ctx.HostDependenciesRegistration += (IDependencyBuilder builder) =>
            {
                builder.Register<ESReportRepository>().As<IReportRepository>();
                builder.Register<ReportPlayersService>().As<IReportPlayersService>();
                builder.Register<ReportPlayersWebApiConfig>().As<IAdminWebApiConfig>(); ;
                builder.Register<ReportPlayersAdminController>();
            };

            ctx.SceneDependenciesRegistration += (IDependencyBuilder builder, ISceneHost scene) =>
            {
                if (scene.Metadata.ContainsKey(METADATA_KEY))
                {
                    builder.Register<ReportPlayersController>();
                }
            };

            ctx.SceneCreated += (ISceneHost scene) =>
            {
                if (scene.Metadata.ContainsKey(METADATA_KEY))
                {
                    scene.AddController<ReportPlayersController>();
                    //scene.DependencyResolver.Resolve<ReportPlayersController>();
                }
            };
        }
    }
}
