using System;
using System.Collections;
using DreamersInc.ReverbCity.GameCode.UI;
using JetBrains.Annotations;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

#pragma warning disable CS0618 // Type or member is obsolete

namespace DreamersInc.UIToolkitHelpers
{
    public abstract class UIController : MonoBehaviour
    {
        protected const string HideClass = "hide";
        [SerializeField] protected UIDocument document;
        [SerializeField] protected StyleSheet _styleSheet;
        protected bool paused = false;
        protected VisualElement Root;
        protected Entity runningEntitySingleton;

        public virtual IEnumerator Generate()
        {
            Root = document.rootVisualElement;

            Root.styleSheets.Add(_styleSheet);
            //var em = World.DefaultGameObjectInjectionWorld.EntityManager;
            //runningEntitySingleton = em.CreateEntityQuery(typeof(RunningTag)).GetSingletonEntity();
            yield return new WaitForEndOfFrame();
        }


    }
}