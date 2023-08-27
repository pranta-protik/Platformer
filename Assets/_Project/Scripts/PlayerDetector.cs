using UnityEngine;
using Utilities;

namespace Platformer
{
    public class PlayerDetector : MonoBehaviour
    {
        [SerializeField] private float _detectionAngle = 60f; // Cone in front of enemy
        [SerializeField] private float _detectionRadius = 10f; // Large circle around enemy
        [SerializeField] private float _innerDetectionRadius = 5f; // Small circle around enemy
        [SerializeField] private float _detectionCooldown = 1f; // Time between detections
        [SerializeField] private float _attackRange = 2f; // Distance from enemy to player to attack

        public Transform Player { get; private set; }

        private CountdownTimer _detectionTimer;
        private IDetectionStrategy _detectionStrategy;

        private void Awake()
        {
            Player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        private void Start()
        {
            _detectionTimer = new CountdownTimer(_detectionCooldown);
            _detectionStrategy = new ConeDetectionStrategy(_detectionAngle, _detectionRadius, _innerDetectionRadius);
        }

        private void Update() => _detectionTimer.Tick(Time.deltaTime);

        public bool CanDetectPlayer()
        {
            return _detectionTimer.IsRunning || _detectionStrategy.Execute(Player, transform, _detectionTimer);
        }

        public bool CanAttackPlayer()
        {
            var directionToPlayer = Player.position - transform.position;
            return directionToPlayer.magnitude <= _attackRange;
        }

        public void SetDetectionStrategy(IDetectionStrategy detectionStrategy) => _detectionStrategy = detectionStrategy;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            // Draw a sphere for the radius
            Gizmos.DrawWireSphere(transform.position, _detectionRadius);
            Gizmos.DrawWireSphere(transform.position, _innerDetectionRadius);

            // Calculate our cone directions
            var forwardConeDirection = Quaternion.Euler(0, _detectionAngle / 2f, 0f) * transform.forward * _detectionRadius;
            var backwardConeDirection = Quaternion.Euler(0, -_detectionAngle / 2f, 0f) * transform.forward * _detectionRadius;

            // Draw lines to represent the cone
            Gizmos.DrawLine(transform.position, transform.position + forwardConeDirection);
            Gizmos.DrawLine(transform.position, transform.position + backwardConeDirection);
        }
    }
}