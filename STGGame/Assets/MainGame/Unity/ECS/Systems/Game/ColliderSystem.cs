using System.Collections.Generic;
using Entitas;
using STGGame;
using UnityEngine;

namespace STGU3D
{
    public class ColliderSystem : IExecuteSystem
    {
        private IGroup<GameEntity> __colliderGroup;

        public ColliderSystem(Contexts contexts)
        {
            //移动
            __colliderGroup = Contexts.sharedInstance.game.GetGroup(
                GameMatcher.AllOf(
                     GameMatcher.Collider,
                     GameMatcher.Transform
                ));
        }
        

        public void Execute()
        {
            
            foreach (var ownerEntity in __colliderGroup.GetEntities())
            {
                //没有形状
                if (ownerEntity.collider.obj == null)
                    continue;

                //没有数据
                if (!ownerEntity.collider.obj.IsReady())
                    continue;

                //不想发生碰撞
                if (ownerEntity.collider.mask == 0)
                    continue;

                //这里可以进行筛选啥的,比如只找在同一个格子中的
                foreach (var otherEntity in __colliderGroup.GetEntities())
                {
                    //自己与自己
                    if (ownerEntity == otherEntity)
                        continue;

                    //不想被别人碰撞
                    if (otherEntity.collider.tag == 0)
                        continue;

                    //没有形状
                    if (otherEntity.collider.obj == null)
                        continue;

                    //没有数据
                    if (!otherEntity.collider.obj.IsReady())
                        continue;

                    //碰撞不相干
                    if ((ownerEntity.collider.mask & otherEntity.collider.tag) == 0)
                        continue;

                    //进行碰撞检测
                    ownerEntity.collider.obj.Update(ownerEntity.transform.position);
                    otherEntity.collider.obj.Update(otherEntity.transform.position);
                    var content = ownerEntity.collider.obj.Collide(otherEntity.collider.obj);
                    if (content != null)
                    {
                        //发生碰撞,执行回调
                        ownerEntity.collider.onCollide?.Invoke(content);
                    }
                }

            }

        }
    }
}

