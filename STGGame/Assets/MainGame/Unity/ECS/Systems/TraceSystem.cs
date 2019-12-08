using Entitas;
using STGGame;
using UnityEngine;

namespace STGU3D
{
    public class TraceSystem : IExecuteSystem
    {
        private IGroup<GameEntity> __traceGroup;
        public TraceSystem(Contexts contexts)
        {
            //移动
            __traceGroup = Contexts.sharedInstance.game.GetGroup(
                GameMatcher.AllOf(
                     GameMatcher.Trace,
                     GameMatcher.Movement,
                     GameMatcher.Transform
                ));
        }
        

        public void Execute()
        {
            foreach (var entity in __traceGroup.GetEntities())
            {
               if (entity.trace.target != null)
                {

                }
            }

        }
    }
}

