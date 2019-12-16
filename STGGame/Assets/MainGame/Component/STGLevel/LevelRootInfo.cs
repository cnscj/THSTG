using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STGGame
{
    public class LevelRootInfo : MonoBehaviour
    {
        public Vector3 center = Vector3.zero;
        public Vector3 size = new Vector3(1024, 576, 0);

        //绘制一个安全框,确保美术K帧不超过这个区域
        private void OnDrawGizmos()
        {
            //TODO:获取关卡范围大小
            Gizmos.color = Color.red;                       //为随后绘制的gizmos设置颜色。
            Gizmos.DrawWireCube(Vector3.zero, DirectorUtil.ScreenSizeInWorld(size));
        }
    }
}
