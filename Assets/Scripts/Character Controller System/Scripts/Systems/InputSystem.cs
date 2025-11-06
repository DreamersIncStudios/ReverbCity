using System.Linq;
using Dreamers.InventorySystem;
using Dreamers.InventorySystem.Interfaces;
using DreamersInc.ComboSystem;
using DreamersInc.InputSystems;
using MotionSystem.Components;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DreamersInc.Global
{
    [UpdateInGroup(typeof(InitializationSystemGroup), OrderLast = true)]
    [UpdateAfter(typeof(ButtonInputSystem))]
    public partial class InputSystem : SystemBase
    {

        private PlayerControls playerControls;

        protected override void OnCreate()
        {
            //Todo make this part of the start up system
            RequireForUpdate<Player_Control>();
            RequireForUpdate<InputSingleton>();
            if (SystemAPI.ManagedAPI.TryGetSingleton<InputSingleton>(out var inputSingle))
            {
                playerControls = inputSingle.ControllerInput;
            }

        }



        protected override void OnUpdate()
        {


        }



    }
}
