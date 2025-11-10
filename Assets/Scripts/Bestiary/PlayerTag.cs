using System;
using DreamersInc.ServiceLocatorSystem;
using UnityEngine;

public class PlayerTag : MonoBehaviour
{
    private void Start()
    {
        if (ServiceLocator.Global.TryGet<PlayerTag>(out var service))
            ServiceLocator.Global.Unregister(this.GetType(), service);

        ServiceLocator.Global.Register(this.GetType(), this);
    }

    private void OnDestroy()
    {
        if (ServiceLocator.Global.TryGet<PlayerTag>(out _))
            ServiceLocator.Global.Unregister(this.GetType(), this);
    }
}