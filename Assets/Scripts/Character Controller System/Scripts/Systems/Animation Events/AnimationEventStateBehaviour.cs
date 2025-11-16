                                                        using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DreamersInc.AnimationEventSystem
{
    public class AnimationEventStateBehaviour : StateMachineBehaviour
    {
        [Serializable]
        public class EventInfo
        {
            public string EventName;
            [Range(0f, 1f)] public float TriggerTime;
            public bool HasTriggered { get; set; }
        }

        [Serializable]
        public class TriggerEventInfo
        {
            public string EventName;
            [Range(0f, 1f)] public float OnTriggerTime;
            [Range(0f, 1f)] public float OffTriggerTime;
            public bool hasToggleOn { get; set; }
            public bool hasToggleOff { get; set; }
        }
        
        [SerializeField] private List<EventInfo> events;
        [SerializeField] private TriggerEventInfo triggerEvent;
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            foreach (EventInfo eventInfo in events)
            {
                eventInfo.HasTriggered = false;
            }
            triggerEvent.hasToggleOn= false;
            triggerEvent.hasToggleOff = true;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var currentTime = stateInfo.normalizedTime % 1f;
            EventTrigger(animator, currentTime);
            
            ToggleEventHandling(animator, currentTime);
        }

        private void ToggleEventHandling(Animator animator, float currentTime)
        {
            if (triggerEvent.hasToggleOff && currentTime >= triggerEvent.OnTriggerTime && 
                currentTime <= triggerEvent.OffTriggerTime)
            {
                NotifyReceiver(animator, triggerEvent.EventName+"On");
                triggerEvent.hasToggleOn = true;
                triggerEvent.hasToggleOff = false;

            }

            if (triggerEvent.hasToggleOn && currentTime >= triggerEvent.OffTriggerTime)
            {
                NotifyReceiver(animator, triggerEvent.EventName + "Off");
                triggerEvent.hasToggleOff = true;
                triggerEvent.hasToggleOn = false;
            }
        }

        private void EventTrigger(Animator animator, float currentTime)
        {
            foreach (var eventInfo in events.Where(eventInfo => !eventInfo.HasTriggered && currentTime >= eventInfo.TriggerTime))
            {
                NotifyReceiver(animator, eventInfo.EventName);
                eventInfo.HasTriggered = true;
            }
        }

        private void NotifyReceiver(Animator animator, string eventName)
        {
             var receiver = animator.GetComponent<AnimationEventReceiver>();
             if (receiver == null) return;
             receiver.OnAnimationEventTriggered(eventName);
        }
    }
}