

using UnityEngine;

namespace STGGame
{
    //只做记录,不做具体逻辑实现
    public class CameraEffect : MonoBehaviour
    {
        //相机震屏
        [Header("震屏（上下、远近、摇头）")]
        public bool shakerEnabled;
        public Vector3 shakeParams = Vector3.forward;
        public float shakePeriod = 0.12f;
        public int shakeCount = 6;

        //场景压黑
        [Header("场景压黑")]
        public bool sceneColorEnabled;
        public Color sceneColor = Color.white;

        //径向模糊
        [Header("径向模糊（变化，形状，不透明度）")]
        public bool radialBlurEnabled;
        [Range(0, 5)] public float radialBlurChange = 1;
        [Range(0.01f, 2)] public float radialBlurShape = 1;
        [Range(0, 1)] public float radialBlurAlpha = 1;

        private void Start()
        {
            
        }
    }
}

