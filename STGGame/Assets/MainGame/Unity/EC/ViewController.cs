using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLibGame;

namespace STGU3D
{
    public delegate void ViewLoadComplete(GameObject view);
    public class ViewController : MonoBehaviour
    {
        public static readonly string bodyName = "Body";
        public UnityView unityView;

        private GameObject body;
        public List<GameObject> showGOs;

        public ViewRenderer rendererCom;
        public ViewCollider colliderCom;
        public ViewAnimator animatorCom;
        public ViewShaderEffect shaderEffectCom;

        public ViewLoadComplete onComplete;

        public void Ceate(UnityView u3dView)
        {
            TryNewBody();

            unityView = u3dView;
            rendererCom = rendererCom == null ? body.AddComponent<ViewRenderer>() : body.GetComponent<ViewRenderer>();
            colliderCom = colliderCom == null ? body.AddComponent<ViewCollider>() : body.GetComponent<ViewCollider>();
            animatorCom = animatorCom == null ? body.AddComponent<ViewAnimator>() : body.GetComponent<ViewAnimator>();
            shaderEffectCom = shaderEffectCom == null ? body.AddComponent<ViewShaderEffect>() : body.GetComponent<ViewShaderEffect>();
        }

        public void AddView(string code, string name = null)
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

                name = name ?? showGO.name;
                showGO.transform.SetParent(body.transform, false);

                rendererCom.Add(showGO);
                colliderCom.Add(showGO);
                animatorCom.Add(showGO);
                shaderEffectCom.Add(showGO);

                onComplete?.Invoke(showGO);
            }
        }

        public void Destroy()
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

        private void TryNewBody()
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
