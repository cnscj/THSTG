using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLibGame;

namespace STGU3D
{
    public class BodyBehaviour : MonoBehaviour
    {
        public GameObject body;
        public GameObject showGO;

        public new Renderer renderer;
        public Animator animator;
        public new Collider collider;

        public void Create(string code)
        {
            TryNewBody();

            EEntityType entityType = EntityUtil.GetEntityTypeByCode(code);
            if (entityType == EEntityType.Hero ||
                entityType == EEntityType.Boss
               )
            {
                showGO = NewRendererNode(true, code);
            }
            else
            {
                int maxCount = 50;
                showGO = NewRendererNode(true, code, maxCount);
                
            }


            {
                showGO.transform.SetParent(body.transform, false);

                renderer = renderer != null ? renderer : showGO.GetComponentInChildren<Renderer>();
                animator = animator != null ? animator : showGO.GetComponentInChildren<Animator>();
                collider = collider != null ? collider : showGO.GetComponentInChildren<Collider>();
            }

            TryAddEffect(showGO);
        }

        public void Destroy()
        {
            if (GameObjectPoolManager.GetInstance())
            {
                GameObjectPoolManager.GetInstance().ReleaseGameObject(showGO);
            }
        }

        private void TryNewBody()
        {
            if (body == null)
            {
                body = new GameObject("Body");
                body.transform.localEulerAngles = Vector3.zero;
                body.transform.localPosition = Vector3.zero;
                body.transform.SetParent(gameObject.transform, false);
            }

        }

        private void TryAddEffect(GameObject go)
        {
            if (go != null)
            {
                var shaderEffectCom = go.GetComponent<ViewShaderEffect>();
                if (shaderEffectCom == null)
                {
                    shaderEffectCom = go.AddComponent<ViewShaderEffect>();
                }
                shaderEffectCom.target = go;
            }
        }

        private GameObject NewRendererNode(bool usePool, string viewCode, int maxCount = 20)
        {
            string viewName = null;
            GameObject viewGO = null;
            GameObject prefabInstance = null;
            if (usePool)
            {
                if (!GameObjectPoolManager.GetInstance().HasGameObjectPool(viewCode))
                {
                    var prefab = AssetManager.GetInstance().LoadSprite(viewCode);
                    if (prefab)
                    {
                        GameObjectPoolManager.GetInstance().NewGameObjectPool(viewCode, prefab, maxCount);
                    }
                }
                prefabInstance = GameObjectPoolManager.GetInstance().GetGameObject(viewCode);
            }
            else
            {
                var prefab = AssetManager.GetInstance().LoadSprite(viewCode);
                if (prefab)
                {
                    prefabInstance = GameObject.Instantiate(prefab);
                }
            }


            if (!string.IsNullOrEmpty(viewName))
            {
                viewGO = new GameObject(viewName);
                prefabInstance.transform.SetParent(viewGO.transform);
            }
            else
            {
                viewGO = prefabInstance;
            }

            return viewGO;
        }
    }

}
