
using UnityEngine;
using Unity.Entities;
namespace STGGame
{
    [RequireComponent(typeof(GameObjectEntity))]
    public class ModelComponent : InputCompnent
    {
        //玩家类型
        public int type;


    }

}
