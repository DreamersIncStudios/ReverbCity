using System;
using Stats.Entities;
using UnityEngine;

namespace DreamersInc.CombatSystem
{
    public class WeaponEventTrigger: MonoBehaviour
    {
        public event EventHandler<CastingArg> OnFireProjectile ;
        public event EventHandler<CastingArg> OnFireChargedProjectile ;
        public event EventHandler<CastingArg> OnHitEnemy;
        public event EventHandler<CastingArg> OnHitByEnemy;
        public event EventHandler<AnimatorArgs> OnAnimationEvent; 
        public event EventHandler<CastingInputArgs> OnCastingInput;
        
        private BaseCharacterComponent characterStats;

        public void Set(ref BaseCharacterComponent characterStats)
        {
            this.characterStats = characterStats;
        }

        public class CastingArg : EventArgs
        {
            public bool IsCharged;
            public BaseCharacterComponent CharacterStats;

            public CastingArg(BaseCharacterComponent characterStats, bool charged)
            {
                IsCharged = charged;
                CharacterStats = characterStats;
            }
        }
        public class AnimatorArgs : EventArgs
        {
            public uint AnimID;
            public float Duration;  
            public float TransitionOffset;
            public float EndofCurrentAnim;

            public AnimatorArgs(uint animID, float duration, float transitionOffset, float endofCurrentAnim)
            {
                AnimID = animID;
                Duration = duration;
                TransitionOffset = transitionOffset;
                EndofCurrentAnim = endofCurrentAnim;
            }

            public AnimatorArgs()
            {
                AnimID = 0;
            }

        }

        public AnimatorArgs AnimatorArgsToPass;


        public void FireSpell()
        {
            OnFireProjectile?.Invoke(this, new CastingArg(characterStats, false));
        }
        public void ChargedFireSpell()
        {
            OnFireProjectile?.Invoke(this, new CastingArg(characterStats, true));
        }
        public void CastSpell(string inputCode)
        {
            OnCastingInput?.Invoke(this, new CastingInputArgs() {CharacterStats = characterStats, InputCode = inputCode});
            OnAnimationEvent?.Invoke(this,AnimatorArgsToPass);
            AnimatorArgsToPass = new AnimatorArgs();
        }
    }

    public class CastingInputArgs : EventArgs
    {
        public BaseCharacterComponent CharacterStats;
        public string InputCode;

    }
}