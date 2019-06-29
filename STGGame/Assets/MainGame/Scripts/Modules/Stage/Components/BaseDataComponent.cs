
using System;
using Unity.Entities;

namespace STGGame
{
    [Serializable]
    public struct BaseData : IComponentData
    {
        public int code;                    //代表code
        public EEntityType entityType;      //实体类型
    }
    public class BaseDataComponent : ComponentDataWrapper<BaseData> { }

}
