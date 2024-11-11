using System.Collections;
using UnityEngine;

namespace Smart.Extensions
{
    public static class AnimatorExtensions
    {
        public static IEnumerator SetTriggerAndWaitForEnd(this Animator animator, int trigger, int layer = 0)
        {
            yield return animator.SetTriggerAndWaitForStart(trigger, layer);
            yield return animator.WaitForAnimationEnd(trigger, layer);
        }
        
        public static IEnumerator SetTriggerAndWaitForStart(this Animator animator, int trigger, int layer = 0)
        {
            animator.SetTrigger(trigger);

            while (animator.GetCurrentAnimatorStateInfo(layer).shortNameHash != trigger)
            {
                yield return null;
            }
        }
        
        public static IEnumerator WaitForAnimationEnd(this Animator animator, int trigger, int layer = 0)
        {
            while (true)
            {
                var stateInfo = animator.GetCurrentAnimatorStateInfo(layer);
                
                if (stateInfo.shortNameHash == trigger && stateInfo.normalizedTime < stateInfo.length)
                {
                    yield return null;
                }
                else
                {
                    break;
                }
            }
        }
        
        public static bool IsAnimationEnded(this Animator animator, int trigger, int layer = 0)
        {
            var stateInfo = animator.GetCurrentAnimatorStateInfo(layer);
            return stateInfo.shortNameHash != trigger || stateInfo.normalizedTime >= stateInfo.length;
        }
        
        public static float GetCurrentStateDuration(this Animator animator, int layer = 0)
        {
            var stateInfo = animator.GetCurrentAnimatorStateInfo(layer);
            return stateInfo.length;
        }
    }
}