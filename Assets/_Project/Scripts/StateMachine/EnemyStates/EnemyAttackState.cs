using UnityEngine;
using UnityEngine.AI;

namespace Platformer
{
    public class EnemyAttackState : EnemyBaseState
    {
        private readonly NavMeshAgent _agent;
        private readonly Transform _player;

        public EnemyAttackState(Enemy enemy, Animator animator, NavMeshAgent agent, Transform player) : base(enemy, animator)
        {
            _agent = agent;
            _player = player;
        }

        public override void OnEnter()
        {
            Debug.Log("Attack");
            _animator.CrossFade(AttackHash, crossFadeDuration);
        }

        public override void Update()
        {
            _agent.SetDestination(_player.position);
            _enemy.Attack();
        }
    }
}