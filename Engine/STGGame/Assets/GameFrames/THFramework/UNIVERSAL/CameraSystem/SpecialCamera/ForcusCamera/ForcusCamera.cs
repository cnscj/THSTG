using THGame.Tween;
using UnityEngine;
using XLibrary.Package;

namespace THGame
{
    //聚焦相机:聚焦主角,改变深度
    public class ForcusCamera : MonoBehaviour
    {
        public static float FORCE_TIME = 0.618f;

        public new Camera camera;       //摄像机
        public Transform observed;      //观察对象
        public float minDepth;          //最小深度
        public float forcusTime = 0.2f; //聚焦耗时

        public Vector3 forcusPoint;   //焦点坐标
        public float forcusRadius;    //聚焦半径

        private float m_oriZVal;
        private TweenScript m_tweener;
        private Vector3 m_velocitv = Vector3.zero;
        private float m_cameraVelocity;

        [ContextMenu("Forcus")]
        public void Forcus()
        {
            Force(minDepth);
        }

        public void Start()
        {
            camera = camera ?? Camera.main;
            observed = (observed != null) ? observed : (camera ? camera.transform : null);
            SvaeDeep();
        }

        public void SvaeDeep()
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

        public float LoadDeep()
        {
            return m_oriZVal;
        }

        public void Force(float deep)
        {
            if (camera == null)
                return;

            //保存属性变更
            StopAllTweener();
            m_tweener = TweenUtil.CustomTweenFloat((val) =>
            {
                SetForcus(val);
            }, LoadDeep(), deep, FORCE_TIME).SetEase(Ease.OutCubic);
        }

        //失焦还原回去
        [ContextMenu("Defocus")]
        public void Defocus()
        {
            StopAllTweener();
            m_tweener = TweenUtil.CustomTweenFloat((val) =>
            {
                SetForcus(val);
            }, GetForcus(), LoadDeep(), FORCE_TIME).SetEase(Ease.OutCubic);
        }

        protected void LateUpdate()
        {
            if (observed == null)
                return;

            if (Mathf.Approximately(forcusRadius, 0f))
                return;

            //根据聚焦半径平滑过渡到最大聚焦深度
            var displacement = forcusPoint - observed.position;
            var displacementLen = displacement.magnitude;
            var dLen = forcusRadius - displacementLen;


            //将焦点拉回到镜头中心
            //TODO:与跟随镜头存在冲突
            if (dLen > 0f)
            {
                var position = transform.position;
                position.x = Mathf.SmoothDamp(position.x, forcusPoint.x, ref m_velocitv.x, 0.618f);
                position.y = Mathf.SmoothDamp(position.y, forcusPoint.y, ref m_velocitv.y, 0.618f);
                transform.position = position;
            }



            //缩放缓动
            var maxDepth = LoadDeep();
            var newDepth = maxDepth - ((maxDepth - minDepth) * (dLen / forcusRadius));
            newDepth = Mathf.Max(newDepth, minDepth);
            newDepth = Mathf.Min(newDepth, maxDepth);
            float val = Mathf.SmoothDamp(GetForcus(), newDepth, ref m_cameraVelocity, forcusTime);
            SetForcus(val);
            
        }

        private void StopAllTweener()
        {
            if (m_tweener != null)
            {
                TweenUtil.StopAnim(m_tweener);
                m_tweener = null;
            }
        }

        public void SetForcus(float deep)
        {
            if (camera.orthographic)
            {
                camera.orthographicSize = deep;
            }
            else
            {
                var localPosition = camera.transform.localPosition;
                localPosition.z = deep;
                camera.transform.localPosition = localPosition;
            }
        }

        public float GetForcus()
        {
            if (camera.orthographic)
            {
                return camera.orthographicSize;
            }
            else
            {
                return camera.transform.localPosition.z;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(forcusPoint, forcusRadius);
        }
    }

}
