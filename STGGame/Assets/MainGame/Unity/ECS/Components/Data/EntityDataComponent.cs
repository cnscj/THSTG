
using Entitas;

namespace STGU3D
{
    [Game]
    public class EntityDataComponent : IComponent
    {
        public string entityCode;           //实体Code
        public EEntityType entityType;      //实体类型

        public int moveSpeed = 1;           //实体速度
        public string viewCode;             //viewCode
    }

}
