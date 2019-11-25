using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace STGU3D
{
    public class CageSystem : IExecuteSystem
    {
        private IGroup<GameEntity> __cageGroup;
        public CageSystem(Contexts contexts)
        {
            __cageGroup = Contexts.sharedInstance.game.GetGroup(
               GameMatcher.AllOf(
                    GameMatcher.Cage,
                    GameMatcher.Transform
               ));
        }

        public void Execute()
        {
            //移动
            foreach (var entity in __cageGroup.GetEntities())
            {
                //边界判断,不能出边界
                
            }
        }
    }
}

