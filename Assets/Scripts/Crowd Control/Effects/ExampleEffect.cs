using CrowdControl.Client.Unity;
using CrowdControl.Common;
using UnityEngine;
using EffectResponse = CrowdControl.Client.WebSocket.EffectResponse;

namespace Assets.Scripts.Crowd_Control.Effects
{
    internal class ExampleEffect : UnityEffectBase
    {
        public override EffectResponse StartEffect(EffectRequest request)
        {
            //example with a single parameter called "itemType" of type **string**
            Debug.Log($"ExampleEffect started with itemType: {(string)request.Parameters["itemType"].Value}.");

            return EffectStatus.Success;
        }

        public override EffectResponse? TickEffect(EffectRequest request)
        {
            Debug.Log("ExampleEffect tick...");

            //this function should normally return null for standard effect implementations
            //returning EffectStatus.TimedPause indicates that the effect should be paused until //TODO

            //if an exception is thrown here, the effect will be automatically canceled with EffectStatus.TimedAborted, which indicates that the effect failed to perform its tick, cannot be ticked later, and should be cleaned up immediately

            return null;
        }

        public override EffectResponse? PauseEffect(EffectRequest request)
        {
            Debug.Log("ExampleEffect paused...");

            //returning null here is equivalent to returning EffectStatus.TimedPause

            //if an exception is thrown here, the effect will be automatically canceled with EffectStatus.TimedAborted, which indicates that the effect failed to pause and cannot be paused, resumed, or ended later and should be cleaned up immediately
            return null;
        }

        public override EffectResponse? ResumeEffect(EffectRequest request)
        {
            Debug.Log("ExampleEffect resumed...");

            //returning null here is equivalent to returning EffectStatus.TimedResume

            //if an exception is thrown here, the effect will be automatically canceled with EffectStatus.TimedAborted, which indicates that the effect failed to resume and cannot be resumed, paused, or ended later and should be cleaned up immediately

            return null;
        }

        public override EffectResponse? StopEffect(EffectRequest request)
        {
            Debug.Log("ExampleEffect stopped...");

            //returning null here is equivalent to returning EffectStatus.TimedEnd or EffectStatus.TimedCanceled automatically, as-appropriate
            //returning EffectStatus.FailTemporary indicates that the effect failed to be stopped, but may be able to be stopped later (e.g. if the effect is waiting for an animation to finish before it can be stopped).

            //if an exception is thrown here, the effect will be automatically canceled with EffectStatus.TimedAborted, which indicates that the effect failed to stop and cannot be stopped or ended later and should be cleaned up immediately

            return null;
        }
    }
}
