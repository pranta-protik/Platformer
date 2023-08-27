using UnityEngine;
using Utilities;

namespace Platformer
{
    public interface IDetectionStrategy
    {
        public bool Execute(Transform player, Transform detector, CountdownTimer timer);
    }
}