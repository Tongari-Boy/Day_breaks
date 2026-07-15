using Player.Item;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public interface IPlayerContext
    {
        List<PlayerItemState> PlayerItemSlots { get; }
    }
}
