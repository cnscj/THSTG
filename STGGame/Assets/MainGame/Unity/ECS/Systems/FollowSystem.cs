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
             
                if (!Vector3.Equals(entity.transform.position, entity.follow.destination))
                {

                }

            }

        }
    }
}

