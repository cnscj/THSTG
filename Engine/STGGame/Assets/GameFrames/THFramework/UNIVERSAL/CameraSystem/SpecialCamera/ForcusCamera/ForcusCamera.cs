using THGame.Tween;
using UnityEngine;
using XLibrary.Package;

namespace THGame
{
    //聚焦相机:聚焦主角,改变深度
    public class ForcusCamera : MonoBehaviour
    {
        public static float FORCE_TIME = 0.618f;

        public new Camera camera;   //摄像机
        public float maxDeep;       //最大深度

        private float m_oriZVal;
        private TweenScript m_tweener;

        [ContextMenu("Forcus")]
        public void Forcus()
        {
            Force(maxDeep);
        }

        public void Start()
        {
            camera = camera ?? Camera.main;
            Svae();
        }

        public void Svae()
        {
            if (camera == null)
                return;

            if (camera.orthographic) //正交相机通过修改size达到深度
            {
                m_oriZVal = camera.orthographicSize;
            }
            else //透视相机修改Z轴坐标
            {
                m_oriZVal = camera.transform.localPosition.z;
            }
        }

        public void Force(float deep)
        {
            if (camera == null)
                return;

            //保存属性变更
            StopAllTweener();
            if (camera.orthographic) //正交相机通过修改size达到深度
            {
                m_tweener = TweenUtil.CustomTweenFloat((val) =>
                {
                    camera.orthographicSize = val;
                }, m_oriZVal, deep, FORCE_TIME).SetEase(Ease.OutCubic);
            }
            else //透视相机修改Z轴坐标
            {
                m_tweener = TweenUtil.CustomTweenFloat((val) =>
                {
                    var localPosition = camera.transform.localPosition;
                    localPosition.z = val;
                    camera.transform.localPosition = localPosition;
                }, m_oriZVal, deep, FORCE_TIME).SetEase(Ease.OutCubic);
            }
        }

        //失焦还原回去
        [ContextMenu("Defocus")]
        public void Defocus()
        {
            StopAllTweener();
            if (camera.orthographic)
            {
                var oldVal = camera.orthographicSize;
                m_tweener = TweenUtil.CustomTweenFloat((val) =>
                {
                    camera.orthographicSize = val;
                }, oldVal, m_oriZVal, FORCE_TIME).SetEase(Ease.OutCubic);
            }
            else
            {
                var oldVal = camera.transform.localPosition.z;
                m_tweener = TweenUtil.CustomTweenFloat((val) =>
                {
                    var localPosition = camera.transform.localPosition;
                    localPosition.z = val;
                    camera.transform.localPosition = localPosition;
                }, oldVal, m_oriZVal, FORCE_TIME).SetEase(Ease.OutCubic);
            }
        }

        private void StopAllTweener()
        {
            if (m_tweener != null)
            {
                TweenUtil.StopAnim(m_tweener);
                m_tweener = null;
            }
        }
    }

}
