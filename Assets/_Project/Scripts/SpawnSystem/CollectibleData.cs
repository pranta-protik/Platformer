﻿using UnityEngine;

namespace Platformer
{
    [CreateAssetMenu(fileName = "CollectibleData", menuName = "Platformer/Collectible Data")]
    public class CollectibleData : EntityData
    {
        public int score;
        // Additional properties specific to collectibles
    }
}