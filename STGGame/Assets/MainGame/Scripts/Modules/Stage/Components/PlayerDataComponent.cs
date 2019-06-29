using System;
using Unity.Entities;

namespace STGGame
{
    [Serializable]
    public struct PlayerData : IComponentData
    {
        public EPlayerType playerType;
    }
    public class PlayerDataComponent : ComponentDataWrapper<PlayerData> { }

}
