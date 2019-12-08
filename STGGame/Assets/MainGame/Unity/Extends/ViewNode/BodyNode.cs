using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLibGame;

namespace STGU3D
{
    public class BodyNode : MonoBehaviour
    {
        public GameObject body;
        public GameObject showGO;
        public Animator animator;

        void Start()
        {
            NewBody();
        }

        public void Create(string code)
        {
            NewBody();

            EEntityType entityType = EntityUtil.GetEntityTypeByCode(code);
            if (entityType == EEntityType.Bullet)
            {
                showGO = ViewUtil.NewRendererNode(true, code);
            }
            else
            {
                showGO = ViewUtil.NewRendererNode(false, code);
            }
            showGO.transform.localPosition = body.transform.localPosition;
            showGO.transform.SetParent(body.transform);
        }

        private void OnDestroy()
        {
            if (GameObjectPoolManager.GetInstance())
            {
                GameObjectPoolManager.GetInstance().ReleaseGameObject(showGO);
            }
        }

        private void NewBody()
        {
            if (body == null)
            {
                body = new GameObject("Body");
                body.transform.SetParent(gameObject.transform);
            }

        }

    }

}
