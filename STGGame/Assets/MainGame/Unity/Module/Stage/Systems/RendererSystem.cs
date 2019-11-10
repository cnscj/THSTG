using Unity.Entities;
using UnityEngine;

namespace STGU3D
{
    public class RendererSystem : ComponentSystem
    {
        struct RendererGroup
        {
            public RendererComponent rendererCom;
            public Transform transform;
        }

        protected override void OnUpdate()
        {
            foreach (var entity in GetEntities<RendererGroup>())
            {
                if (!string.IsNullOrEmpty(entity.rendererCom.spriteCode))
                {
                    //需要更换code
                    if (entity.rendererCom.curSpriteCode != entity.rendererCom.spriteCode)
                    {
                        //移除掉之前的
                        if (entity.rendererCom.renderer != null)
                        {
                            entity.rendererCom.renderer = null;
                        }

                        var bodyNode = entity.transform.Find("Body");
                        if (bodyNode != null) GameObject.Destroy(bodyNode);

                        bodyNode = new GameObject("Body").transform;
                        bodyNode.SetParent(entity.transform, false);

                        //

                        var sprite = AssetManager.GetInstance().LoadSprite(entity.rendererCom.spriteCode);
                        if (sprite)
                        {
                            var displayGO = GameObject.Instantiate(sprite, bodyNode.transform);
                            entity.rendererCom.renderer = displayGO.GetComponent<Renderer>();

                            entity.rendererCom.curSpriteCode = entity.rendererCom.spriteCode;
                        }
                        else
                        {
                            entity.rendererCom.spriteCode = "";    //失败code直接置空
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(entity.rendererCom.curSpriteCode))
                    {
                        if (entity.rendererCom.renderer != null)
                        {
                            entity.rendererCom.renderer = null;
                        }

                        var bodyNode = entity.transform.Find("Body");
                        if (bodyNode != null) GameObject.Destroy(bodyNode);

                        entity.rendererCom.curSpriteCode = "";
                    }
                }
            }
        }
    }
}

