
using THGame;
using UnityEngine;

namespace STGGame
{
    public class LevelRootInfo : MonoBehaviour
    {
        public int randomRoot;  //随机数种子
        public Rect region;


        private void Awake()
        {
            ObjectEmitManager.GetInstance().UpdateRandom(randomRoot);
        }

        //绘制一个安全框,确保美术K帧不超过这个区域
        private void OnDrawGizmos()
        {
            //获取关卡范围大小
            Camera theCamera = Camera.main;
            if (theCamera != null)
            {
                Transform tx = theCamera.transform;

                Vector3 startPos = theCamera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, -tx.position.z));

                Vector3 center = theCamera.ScreenToWorldPoint(new Vector3(region.x + region.width / 2f, region.y + region.height / 2f, -tx.position.z));
                Vector3 size = theCamera.ScreenToWorldPoint(new Vector3(startPos.x + region.width, startPos.y + region.height, -tx.position.z));

                Gizmos.color = Color.red;//为随后绘制的gizmos设置颜色。
                Gizmos.DrawWireCube(center, size);
 
            }
        }
    }
}
