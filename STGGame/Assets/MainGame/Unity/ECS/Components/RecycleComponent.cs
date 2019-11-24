using Entitas;
using UnityEngine;

namespace STGU3D
{
    //回收组件
    [Game]
    public class RecycleComponent : IComponent
    {
		public Rect boundary;                   //最大可活动边界
        public float maxStayTime = 3f;          //在边界外最大停留时间

        public float stayTime;                  //在边界停留的时间
        public bool isRecycled;                 //是否已被回收
    }
}
