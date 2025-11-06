using Unity.Entities;


public partial class VisionTargetingUpdateGroup : ComponentSystemGroup
{
    public VisionTargetingUpdateGroup()
    {
        RateManager = new RateUtils.VariableRateManager(125, true);
    }

}