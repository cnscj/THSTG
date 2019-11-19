
using Entitas;
using XLibrary;

namespace STGU3D
{
    [Game]
    public class EntityDataComponent : IComponent
    {
        public string entityCode;           //实体Code
        public EEntityType entityType;      //实体类型

        public CSVObject entityData;        //实体数据
    }

}
