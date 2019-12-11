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
                    var curPos = entity.transform.position;
                    var destPos = entity.trace.target.transform.position;
                    var abVec = destPos - curPos;
                    var curSpeed = entity.movement.moveSpeed.magnitude;
                    //方向向量转角度值
                    var angle_1 = 360 - Mathf.Atan2(abVec.x, abVec.y) * Mathf.Rad2Deg;
                    //将当前物体的角度设置为对应角度
                    entity.movement.rotationSpeed = new Vector3(0, 0, angle_1);
                    entity.movement.moveSpeed = MathUtil.ChangeVectorDirection(entity.movement.rotationSpeed, entity.movement.moveSpeed, out entity.movement.moveSpeed);
                }
                else
                {
                    if (entity.trace.isAutoSearch)
                    {
                        //TODO:从KD树中取得最近点

                    }
                }
            }

        }
    }
}

