using UnityEngine;
using Utilities;

namespace Platformer
{
    public class ConeDetectionStrategy : IDetectionStrategy
    {
        private readonly float _detectionAngle;
        private readonly float _detectionRadius;
        private readonly float _innerDetectionRadius;

        public ConeDetectionStrategy(float detectionAngle, float detectionRadius, float innerDetectionRadius)
        {
            _detectionAngle = detectionAngle;
            _detectionRadius = detectionRadius;
            _innerDetectionRadius = innerDetectionRadius;
        }

        public bool Execute(Transform player, Transform detector, CountdownTimer timer)
        {
            if (timer.IsRunning) return false;

            var directionToPlayer = player.position - detector.position;
            var angleToPlayer = Vector3.Angle(directionToPlayer, detector.forward);

            // If the player is not within the detection angle + outer radius (aka the cone in front of the enemy),
            // or is within the inner radius. return false
            if ((!(angleToPlayer < _detectionAngle / 2f) || !(directionToPlayer.magnitude < _detectionRadius))
                && !(directionToPlayer.magnitude < _innerDetectionRadius))
            {
                return false;
            }

            timer.Start();
            return true;
        }
    }
}