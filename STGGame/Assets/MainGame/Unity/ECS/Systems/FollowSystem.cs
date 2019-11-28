using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace STGU3D
{
    public class FollowSystem : IExecuteSystem
    {
        private IGroup<GameEntity> __followGroup;
        public FollowSystem(Contexts contexts)
        {
            //移动
            __followGroup = Contexts.sharedInstance.game.GetGroup(
                GameMatcher.AllOf(
                     GameMatcher.Follow,
                     GameMatcher.Movement,
                     GameMatcher.Transform
                ));
        }

        public void Execute()
        {
            foreach (var entity in __followGroup.GetEntities())
            {
                if (entity.follow.isFollowing)
                {
                    //是否在目标点半径内
                    float distance = (entity.transform.position - entity.follow.destination).magnitude;
                    if (distance <= entity.follow.followRadius)
                    {
                        entity.follow.isFollowing = false;
                    }
                }
            }

        }
    }
}

