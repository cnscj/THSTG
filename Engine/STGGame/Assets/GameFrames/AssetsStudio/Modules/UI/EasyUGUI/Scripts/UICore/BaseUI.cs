using UnityEngine;
using System.Collections;

namespace ASGame.UI
{
    public class BaseUI : MonoBehaviour
    {
        /// <summary>
        /// 当前界面名称
        /// </summary>
        [HideInInspector]
        public string UIName;

        private Transform mTransform;
        public Transform CacheTransform
        {
            get
            {
                if (mTransform == null) mTransform = this.transform;
                return mTransform;
            }
        }

        private GameObject mGo;
        public GameObject CacheGameObject
        {
            get
            {
                if (mGo == null) mGo = this.gameObject;
                return mGo;
            }
        }

        /// <summary>
        /// 显示当前UI
        /// </summary>
        /// <param name="param">附加参数</param>
        public void Show(object param = null)
        {
            CacheGameObject.SetActive(true);
        }

        /// <summary>
        /// 隐藏当前界面
        /// </summary>
        public void Hide()
        {
            CacheGameObject.SetActive(false);
        }



        /// <summary>
        /// 绑定脚本并且激活游戏物体会调用的方法
        /// </summary>
        void Awake()
        {
            OnAwake();
        }

        /// <summary>
        /// 初始化UI主要用于寻找组件等
        /// </summary>
        public void UIInit()
        {
            OnInit();
        }

        /// <summary>
        /// 显示当前界面
        /// </summary>
        /// <param name="param">附加参数</param>
        protected virtual void OnShow(object param) { }

        /// <summary>
        /// 隐藏当前界面
        /// </summary>
        protected virtual void OnHide() { }

        /// <summary>
        /// 初始化当前界面
        /// </summary>
        protected virtual void OnInit() { }

        protected virtual void OnAwake() { }

        /// <summary>
        /// 删除当前UI 
        /// </summary>
        protected virtual void OnDestroy() { }
    }


}
