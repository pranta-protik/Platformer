using UnityEngine;

namespace Platformer
{
    public class DashState : BaseState
    {
        public DashState(PlayerController player, Animator animator) : base(player, animator)
        {
        }

        public override void OnEnter()
        {
            Debug.Log("DashState.OnEnter");
            _animator.CrossFadeInFixedTime(DashHash, crossFadeDuration);
        }

        public override void FixedUpdate()
        {
            _player.HandleMovement();
        }
    }
}