using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLibGame;

namespace STGU3D
{
    public class ViewControllerMono : MonoBehaviour
    {
        public static readonly string bodyName = "Body";

        private GameObject body;
        public List<GameObject> showGOs;
        public UnityView unityView { get; protected set; }

        //记录一些常用的脚本
        public ViewRenderer rendererCom { get; protected set; }
        public ViewCollider colliderCom { get; protected set; }
        public ViewAnimator animatorCom { get; protected set; }
        public ViewShaderEffect shaderEffectCom { get; protected set; }

        public void Ceate(UnityView u3dView)
        {
            TryNewBody();

            unityView = u3dView;
            rendererCom = (ViewRenderer)(rendererCom?.Bind(u3dView) ?? new ViewRenderer()?.Bind(u3dView));
            colliderCom = (ViewCollider)(colliderCom?.Bind(u3dView) ?? new ViewCollider()?.Bind(u3dView));
            animatorCom = (ViewAnimator)(animatorCom?.Bind(u3dView) ?? new ViewAnimator()?.Bind(u3dView));
            shaderEffectCom = (ViewShaderEffect)(shaderEffectCom?.Bind(u3dView) ?? new ViewShaderEffect()?.Bind(u3dView));

            Init();
        }

        public void AddView(string code, Action<GameObject> callback = null)
        {
            GameObject showGO; 
            EEntityType entityType = EntityUtil.GetEntityTypeByCode(code);
            if (entityType == EEntityType.Hero ||
                entityType == EEntityType.Boss
               )
            {
                showGO = SpriteManager.GetInstance().GetOrNewSprite(code, true);
            }
            else
            {
                int maxCount = 50;
                showGO = SpriteManager.GetInstance().GetOrNewSprite(code, true, maxCount);
            }

            if (showGO != null)
            {
                showGOs = showGOs ?? new List<GameObject>();
                showGOs.Add(showGO);

                showGO.transform.SetParent(body.transform, false);

                rendererCom.Add(showGO);
                colliderCom.Add(showGO);
                animatorCom.Add(showGO);
                shaderEffectCom.Add(showGO);

                callback?.Invoke(showGO);
            }
        }

        public void Destroy()
        {
            UnityEngine.Object.Destroy(this);
        }

        void Init()
        {
            string color = unityView.entity.entityData.entityCode.Substring(unityView.entity.entityData.entityCode.Length - 2, 2);
            string viewCode = string.Format(unityView.entity.entityData.entityData["viewCode"], color);
            AddView(viewCode);
        }

        void OnDestroy()
        {
            if (showGOs != null)
            {
                foreach (var showGO in showGOs)
                {
                    var goPoolMgr = GameObjectPoolManager.GetInstance();
                    goPoolMgr?.ReleaseGameObject(showGO);
                }
                showGOs.Clear();
            }
        }

        void TryNewBody()
        {
            if (body == null)
            {
                body = new GameObject(bodyName);
                body.transform.localEulerAngles = Vector3.zero;
                body.transform.localPosition = Vector3.zero;
                body.transform.SetParent(gameObject.transform, false);
            }
        }
    }

}
