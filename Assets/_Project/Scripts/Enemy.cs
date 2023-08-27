using KBCore.Refs;
using UnityEngine;
using UnityEngine.AI;
using Utilities;

namespace Platformer
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(PlayerDetector))]
    public class Enemy : MonoBehaviour
    {
        [SerializeField, Self] private NavMeshAgent _agent;
        [SerializeField, Self] private PlayerDetector _playerDetector;
        [SerializeField, Child] private Animator _animator;
        [SerializeField] private float _wanderRadius = 10f;
        [SerializeField] private float _timeBetweenAttacks = 1f;

        private StateMachine _stateMachine;
        private CountdownTimer _attackTimer;

        private void OnValidate() => this.ValidateRefs();

        private void Start()
        {
            _stateMachine = new StateMachine();

            _attackTimer = new CountdownTimer(_timeBetweenAttacks);

            var wanderState = new EnemyWanderState(this, _animator, _agent, _wanderRadius);
            var chaseState = new EnemyChaseState(this, _animator, _agent, _playerDetector.Player);
            var attackState = new EnemyAttackState(this, _animator, _agent, _playerDetector.Player);

            At(wanderState, chaseState, new FuncPredicate(() => _playerDetector.CanDetectPlayer()));
            At(chaseState, wanderState, new FuncPredicate(() => !_playerDetector.CanDetectPlayer()));
            At(chaseState, attackState, new FuncPredicate(() => _playerDetector.CanAttackPlayer()));
            At(attackState, chaseState, new FuncPredicate(() => !_playerDetector.CanAttackPlayer()));

            _stateMachine.SetState(wanderState);
        }

        private void At(IState from, IState to, IPredicate condition) => _stateMachine.AddTransition(from, to, condition);
        private void Any(IState to, IPredicate condition) => _stateMachine.AddAnyTransition(to, condition);

        private void Update()
        {
            _stateMachine.Update();
            _attackTimer.Tick(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            _stateMachine.FixedUpdate();
        }

        public void Attack()
        {
            if (_attackTimer.IsRunning) return;

            _attackTimer.Start();
            Debug.Log("Attacking");
        }
    }
}
