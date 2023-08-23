using UnityEngine;

namespace Platformer
{
    public class PlatformCollisionHandler : MonoBehaviour
    {
        private Transform _platform; // The platform, if any, we are on top of

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("MovingPlatform"))
            {
                // If the contact normal is pointing up, we've collided with the top of the platform
                var contact = other.GetContact(0);
                if (contact.normal.y  < 0.5f) return;

                _platform = other.transform;
                transform.SetParent(_platform);
            }
        }

        private void OnCollisionExit(Collision other)
        {
            if (other.gameObject.CompareTag("MovingPlatform"))
            {
                transform.SetParent(null);
                _platform = null;
            }
        }
    }
}