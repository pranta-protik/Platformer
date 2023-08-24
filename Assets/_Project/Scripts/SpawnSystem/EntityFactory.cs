using UnityEngine;

namespace Platformer
{
    public class EntityFactory<T> : IEntityFactory<T> where T : Entity
    {
        private EntityData[] _data;

        public EntityFactory(EntityData[] data)
        {
            _data = data;
        }
        
        public T Create(Transform spawnPoint)
        {
            var entityData = _data[Random.Range(0, _data.Length)];
            var instance = GameObject.Instantiate(entityData.prefab, spawnPoint.position, spawnPoint.rotation);
            return instance.GetComponent<T>();
        }
    }
}