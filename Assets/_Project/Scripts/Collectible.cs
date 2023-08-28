using UnityEngine;

namespace Platformer
{
    public class Collectible : Entity
    {
        [SerializeField] private int _score = 10; // FIXME set using factory
        [SerializeField] private IntEventChannel _scoreChannel;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _scoreChannel.Invoke(_score);
                Destroy(gameObject);
            }
        }
    }
}