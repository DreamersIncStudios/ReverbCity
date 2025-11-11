using Unity.Entities;

namespace DreamersIncStudio.GAIACollective
{
    internal partial class GaiaUpdateGroup : ComponentSystemGroup
    {
        public GaiaUpdateGroup()
        {
            RateManager = new RateUtils.VariableRateManager(80);
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            RequireForUpdate<RunningTag>();
            RequireForUpdate<GaiaTime>();
            RequireForUpdate<WorldManager>();
        }
    }
}