using System.Collections.Generic;
using Entitas;

namespace STGU3D
{
    [Game]
    public class BuffComponent : IComponent
    {
        public List<BuffDataModel> buffList = new List<BuffDataModel>();
    }
}
