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
    public abstract class UIManager : MonoBehaviour
    {
        protected const string HideClass = "hide";
        [SerializeField] protected UIDocument document;
        [SerializeField] protected StyleSheet _styleSheet;
        protected VisualElement ActiveElement, PreviousElement;
        protected VisualElement InfoPanel;
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

        protected virtual void TogglePause(InputAction.CallbackContext obj)
        {
            var em = World.DefaultGameObjectInjectionWorld.EntityManager;
            paused = !paused;

            if (paused)
            {
                em.RemoveComponent<RunningTag>(runningEntitySingleton);
            }
            else
            {
                em.AddComponent<RunningTag>(runningEntitySingleton);
            }
        }

        public void RunGame()
        {
            var em = World.DefaultGameObjectInjectionWorld.EntityManager;
            em.AddComponent<RunningTag>(runningEntitySingleton);
        }

        protected VisualElement CreateModal()
        {
            var back = UIExtensionMethods.Create("background", "hide");
            back.CreateChild("modal");
            return back;
        }


        /// <summary>
        /// Registers navigation callbacks for a given VisualElement to allow directional navigation using keyboard or controller.
        /// </summary>
        /// <param name="assigner">The VisualElement to which navigation event handlers will be assigned.</param>
        /// <param name="left">The VisualElement to navigate to when moving left. Can be null if no navigation is required in this direction.</param>
        /// <param name="right">The VisualElement to navigate to when moving right. Can be null if no navigation is required in this direction.</param>
        /// <param name="up">The VisualElement to navigate to when moving up. Can be null if no navigation is required in this direction.</param>
        /// <param name="down">The VisualElement to navigate to when moving down. Can be null if no navigation is required in this direction.</param>
        protected static void RegisterNavigationCallbacks(VisualElement assigner, [CanBeNull] VisualElement left,
            [CanBeNull] VisualElement right,
            [CanBeNull] VisualElement up, [CanBeNull] VisualElement down)
        {
            assigner.RegisterCallback<NavigationMoveEvent>(e =>
            {
                switch (e.direction)
                {
                    case NavigationMoveEvent.Direction.Down:
                        down?.Focus();
                        break;
                    case NavigationMoveEvent.Direction.Left:
                        left?.Focus();
                        break;
                    case NavigationMoveEvent.Direction.Up:
                        up?.Focus();
                        break;
                    case NavigationMoveEvent.Direction.Right:
                        right?.Focus();
                        break;
                }

                e.PreventDefault();
            });
        }

        protected void CancelMenuAction(InputAction.CallbackContext obj)
        {
            if (PreviousElement == null) return;
            PreviousElement.RemoveFromClassList(HideClass);
            ActiveElement.AddToClassList(HideClass);
            ActiveElement = PreviousElement;
        }

        protected void ShowInfoPanel(VisualElement button)
        {
            // Position the info panel relative to the button
            var buttonWorldBound = button.worldBound;
            InfoPanel.style.left = buttonWorldBound.x;
            InfoPanel.style.top = buttonWorldBound.y + buttonWorldBound.height;

            // Display the panel
            InfoPanel.RemoveFromClassList(HideClass);
            InfoPanel.BringToFront();
        }

        protected void HideInfoPanel()
        {
            // Hide the panel
            InfoPanel.AddToClassList(HideClass);
            InfoPanel.SendToBack();
        }
    }
}