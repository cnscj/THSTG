using Unity.Entities;
using UnityEngine;

namespace STGU3D
{
    public class RendererSystem : ComponentSystem
    {
        public static readonly string nodeNama = "Renderer";
        struct RendererGroup
        {
            public RendererComponent rendererCom;
        }

        protected override void OnUpdate()
        {
            foreach (var entity in GetEntities<RendererGroup>())
            {
                var entityTrans = entity.rendererCom.gameObject.transform;
                if (!string.IsNullOrEmpty(entity.rendererCom.rendererCode))
                {
                    //需要更换code
                    if (entity.rendererCom.curRendererCode != entity.rendererCom.rendererCode)
                    {
                        //移除掉之前的
                        if (entity.rendererCom.renderer != null)
                        {
                            entity.rendererCom.renderer = null;
                        }

                        var bodyNode = entityTrans.Find(nodeNama);
                        if (bodyNode != null) GameObject.Destroy(bodyNode);

                        bodyNode = new GameObject("Body").transform;
                        bodyNode.SetParent(entityTrans, false);

                        //

                        var sprite = AssetManager.GetInstance().LoadSprite(entity.rendererCom.rendererCode);
                        if (sprite)
                        {
                            var displayGO = GameObject.Instantiate(sprite, bodyNode.transform);
                            entity.rendererCom.renderer = displayGO.GetComponent<Renderer>();

                            entity.rendererCom.curRendererCode = entity.rendererCom.rendererCode;
                        }
                        else
                        {
                            entity.rendererCom.rendererCode = "";    //失败code直接置空
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(entity.rendererCom.curRendererCode))
                    {
                        if (entity.rendererCom.renderer != null)
                        {
                            entity.rendererCom.renderer = null;
                        }

                        var bodyNode = entityTrans.Find(nodeNama);
                        if (bodyNode != null) GameObject.Destroy(bodyNode);

                        entity.rendererCom.curRendererCode = "";
                    }
                }
            }
        }
    }
}

