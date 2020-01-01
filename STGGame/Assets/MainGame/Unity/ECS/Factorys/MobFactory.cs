using System.Collections;
using System.Collections.Generic;
using STGGame;
using UnityEngine;

namespace STGU3D
{
    public class MobFactory : BaseEntityFactory
    {
        protected override GameEntity OnCreate(string code)
        {
            var entity = CreateGameEntity(code);

            var mobDataCom = entity.CreateComponent<MobDataComponent>(GameComponentsLookup.MobData);
            var recycleCom = entity.CreateComponent<RecycleComponent>(GameComponentsLookup.Recycle);

            entity.AddComponent(GameComponentsLookup.MobData, mobDataCom);
            entity.AddComponent(GameComponentsLookup.Recycle, recycleCom);

            if (entity.hasEntityData)
            {
                entity.view.view = ComponentUtil.CreateView(entity);
                entity.ReplaceComponent(GameComponentsLookup.View, entity.view);

                {
                    recycleCom.maxStayTime = 3f;
                    recycleCom.stayTime = 0f;
                    recycleCom.isRecycled = false;
                    recycleCom.boundary = DirectorUtil.ScreenRectInWorld(DirectorUtil.GetScreenRect());
                }

                {
                    entity.collider.obj.data = entity;
                    entity.collider.tag = ColliderType.Mob;
                    entity.collider.mask = 0;//不碰撞其他
                    entity.collider.obj.AddShape(new CircleCollider()
                    {
                        radius = 0.1f
                    });
                }
            }
            return entity;
        }
        
    }

}
