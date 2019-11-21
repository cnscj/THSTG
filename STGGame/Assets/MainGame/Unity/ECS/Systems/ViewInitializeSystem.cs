using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace STGU3D
{
    public class ViewInitializeSystem : IInitializeSystem
    {
        public ViewInitializeSystem(Contexts contexts)
        {

        }

        public void Initialize()
        {
            var group = Contexts.sharedInstance.game.GetGroup(
                GameMatcher.AllOf(
                     GameMatcher.View
                ));

            foreach (var entity in group.GetEntities())
            {
                ViewSystemHelper.TryCreateView(entity);

            }
        }
    }
}

