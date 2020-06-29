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
        public Transform observed;  //观察对象
        public float maxDeep;       //最大深度


        public Vector3 forcusPoint;   //焦点坐标
        public float forcusRadius;    //聚焦半径

        private float m_oriZVal;
        private TweenScript m_tweener;
        private Vector3 m_velocitv = Vector3.zero;

        [ContextMenu("Forcus")]
        public void Forcus()
        {
            Force(maxDeep);
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

            //范围外
            if (dLen < 0f)
                return;

            //将焦点拉回到镜头中心
            var position = transform.position;
            position.x = Mathf.SmoothDamp(position.x, forcusPoint.x, ref m_velocitv.x, 0.618f);
            position.y = Mathf.SmoothDamp(position.y, forcusPoint.y, ref m_velocitv.y, 0.618f);
            transform.position = position;

            //
            float val = Mathf.SmoothStep(LoadDeep(), maxDeep, (dLen / forcusRadius));
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
