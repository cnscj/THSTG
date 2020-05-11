using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace STGU3D
{
    public class RecycleSystem : IExecuteSystem
    {
        private IGroup<GameEntity> __recycleGroup;
        public RecycleSystem(Contexts contexts)
        {
            __recycleGroup = Contexts.sharedInstance.game.GetGroup(
                GameMatcher.AllOf(
                     GameMatcher.Recycle
            ));
        }
        public void Recycle(GameEntity entity)
        {
            entity.recycle.isRecycled = true;
            EntityManager.GetInstance().DestroyEntity(entity);

        }

        public void Execute()
        {
            foreach (var entity in __recycleGroup.GetEntities())
            {
                if (entity.hasRecycle)
                {
                    //主要回收那些已经无效了的组件
                    if(!entity.recycle.isRecycled)
                    {
                        bool isNeedCheck = false;               //需要检测

                        //XXX:超出屏幕,这里是粗略计算的
                        if (entity.hasTransform)                
                        {
                            isNeedCheck = !entity.recycle.boundary.Contains(entity.transform.position);
                        }

                        /////
                        if (isNeedCheck)
                        {
                            if (entity.recycle.stayTime < entity.recycle.maxStayTime)
                            {
                                entity.recycle.stayTime += Time.deltaTime;
                            }
                            else
                            {
                                Recycle(entity);
                            }
                        }
                        else
                        {
                            entity.recycle.stayTime = 0f;
                        }

                    }

                }
            }
        }
    }
}

