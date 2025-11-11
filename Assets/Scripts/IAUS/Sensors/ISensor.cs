using System;
using Unity.Entities;

namespace IAUS.Core.GOAP
{
    public interface ISensor : IComponentData
    {
        public float DetectionRange { get; set; }
        float Timer { get; set; } // consider using Variable Rate Manager;
    }

    public partial class SensorEventManagement : SystemBase
    {
        public static event EventHandler<OnTargetChanged> SensorChange;

        protected override void OnUpdate()
        {
        }

        public class OnTargetChanged : EventArgs
        {
            public ISensor SensorComponent;
        }
    }
}