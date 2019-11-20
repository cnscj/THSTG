using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace STGU3D
{
    public class ViewInitializeSystem : IInitializeSystem
    {
        public static readonly string viewName = "View";
        public ViewInitializeSystem(Contexts contexts)
        {

        }

        public void Initialize()
        {
            var moveGroup = Contexts.sharedInstance.game.GetGroup(
                GameMatcher.AllOf(
                     GameMatcher.View
                ));

            foreach (var entity in moveGroup.GetEntities())
            {
                ViewSystemHelper.TryCreateView(entity);

            }
        }
    }
}

