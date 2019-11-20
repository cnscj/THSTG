using Entitas;
using UnityEngine;

namespace STGU3D
{
    //回收组件
    [Game]
    public class RecycleComponent : IComponent
    {
		public Vector2 boundary;    //最大可活动边界
        public float maxStayTime;   //在边界最大停留时间

        public float stayTime;      //在边界停留时间
    }
}
