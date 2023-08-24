using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Platformer
{
    public class RandomSpawnPointStrategy : ISpawnPointStrategy
    {
        private List<Transform> _unusedSpawnPointsList;
        private Transform[] _spawnPoints;

        public RandomSpawnPointStrategy(Transform[] spawnPoints)
        {
            _spawnPoints = spawnPoints;
            _unusedSpawnPointsList = new List<Transform>(spawnPoints);
        }
        
        public Transform NextSpawnPoint()
        {
            if (!_unusedSpawnPointsList.Any())
            {
                _unusedSpawnPointsList = new List<Transform>(_spawnPoints);
            }

            var randomIndex = Random.Range(0, _unusedSpawnPointsList.Count);
            var result = _unusedSpawnPointsList[randomIndex];
            _unusedSpawnPointsList.RemoveAt(randomIndex);
            return result;
        }
    }
}