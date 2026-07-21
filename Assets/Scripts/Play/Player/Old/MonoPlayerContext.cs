using Player.Item;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class MonoPlayerContext : MonoBehaviour, IPlayerContext, IDamageable
    {
        public List<PlayerItemState> PlayerItemSlots
        {
            get { return null; }
        }

        public void OnDamaged(float damageAmount) { }
    }
}