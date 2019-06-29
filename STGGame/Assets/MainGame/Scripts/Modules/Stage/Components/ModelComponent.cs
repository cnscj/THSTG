using System;
using Unity.Entities;
namespace STGGame
{
    [Serializable]
    public struct Model : IComponentData
    {
        //玩家类型
        public int modelId;
    }
    public class ModelComponent : ComponentDataWrapper<Model> { }
}
