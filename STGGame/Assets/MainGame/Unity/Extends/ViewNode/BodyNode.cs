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

        public new Renderer renderer;
        public Animator animator;
        public new Collider collider;

        public void Create(string code)
        {
            TryNewBody();

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

            renderer = renderer != null ? renderer : showGO.GetComponentInChildren<Renderer>();
            animator = animator != null ? animator : showGO.GetComponentInChildren<Animator>();
            collider = collider != null ? collider : showGO.GetComponentInChildren<Collider>();

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
                body.transform.SetParent(gameObject.transform);
            }

        }

    }

}
