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

                //不想发生碰撞
                if (ownerEntity.collider.mask == 0)
                    continue;

                //挂了的实体不检测
                if (ownerEntity.hasHealth)
                    if (ownerEntity.health.blood <= 0)
                        continue;

                //这里可以进行筛选啥的,比如只找在同一个格子中的
                ownerEntity.collider.obj.BeginCollide(ownerEntity.transform.position);
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

                    //碰撞不相干
                    if ((ownerEntity.collider.mask & otherEntity.collider.tag) == 0)
                        continue;

                    // 挂了的实体不检测
                    if (otherEntity.hasHealth)
                            if (otherEntity.health.blood <= 0)
                                continue;

                    //进行碰撞检测
                    ownerEntity.collider.obj.Collide(otherEntity.collider.obj,otherEntity.transform.position);
                }

                //一帧中可能与多个物体发生碰撞,所以应该在这里执行
                var content = ownerEntity.collider.obj.GetContent();
                if (content != null)
                {
                    OnCollide(ownerEntity, content);
                }
            }

        }

        private void OnCollide(GameEntity owner, ColliderContent content)
        {
            //碰撞处理
            OnHeroCollide(owner, content);
            OnHeroBulletCollde(owner, content);
        }

        ///////////////////////////////////
        private void OnHeroCollide(GameEntity owner, ColliderContent content)
        {
            if (owner.hasPlayerData)
            {
                var ownerEntity = content.owner.data as GameEntity;
                ownerEntity.health.blood = 0;
            }
        }

        private void OnHeroBulletCollde(GameEntity owner, ColliderContent content)
        {
            if (owner.isHeroBulletFlag)
            {
                var ownerEntity = content.owner.data as GameEntity;
                foreach(var collision in content.collisions)
                {
                    var otherEntity = collision.Key.data as GameEntity;

                    if (otherEntity.hasHealth)
                    {
                        if (otherEntity.hasMobData)
                        {
                            //TODO:
                            otherEntity.health.blood -= 20;
                            otherEntity.ReplaceComponent(GameComponentsLookup.Health, otherEntity.health);
                        }
                        else if (otherEntity.hasBossData)
                        {
                            //TODO:
                            otherEntity.health.blood -= 20;
                            otherEntity.ReplaceComponent(GameComponentsLookup.Health, otherEntity.health);
                        }

                    }
                    
                    //自己的
                    if (ownerEntity.hasHealth)
                    {
                        //TODO:根据不同子弹处理,如穿甲弹不应该消亡
                        ownerEntity.health.blood = 0;   //子弹直接消亡

                        if (ownerEntity.health.blood <=0 )
                            break; 
                    }
                }

            }
        }
    }
}

