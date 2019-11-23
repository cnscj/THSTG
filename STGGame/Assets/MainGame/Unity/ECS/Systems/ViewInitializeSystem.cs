using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace STGU3D
{
    public class ViewInitializeSystem : IInitializeSystem
    {
        private IGroup<GameEntity> __group;
        public ViewInitializeSystem(Contexts contexts)
        {
            __group = Contexts.sharedInstance.game.GetGroup(
                GameMatcher.AllOf(
                     GameMatcher.View
                ));
        }

        public void Initialize()
        {
            foreach (var entity in __group.GetEntities())
            {
                ViewSystemHelper.TryCreateView(entity);

            }
        }
    }
}

