using Entitas;
using STGGame;
using UnityEngine;

namespace STGU3D
{
    public class FollowSystem : IExecuteSystem
    {
        private static readonly float PRECISION = 0.01f;    //精度:如果设置的太小会导致浮点精度比较的BUG,永远无法到达真实的0f
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

        //精度化
        public float NormalizeZero(float val)
        {
            return Mathf.Abs(val) < PRECISION ? 0f : val;
        }

        public Vector3 NormalizeZero(ref Vector3 inVec, ref Vector3 outVec)
        {
            outVec.x = NormalizeZero(inVec.x);
            outVec.y = NormalizeZero(inVec.y);
            outVec.z = NormalizeZero(inVec.z);
            return outVec;
        }

        public void Execute()
        {
            foreach (var entity in __followGroup.GetEntities())
            {
                if (entity.follow.isFollowing)
                {
                    //是否在目标点半径内
                    Vector3 abVec = (entity.follow.destination - entity.transform.position);
                    float distance = abVec.magnitude;
                    float safeRadius = Mathf.Max(PRECISION, entity.follow.followRadius);
                    if (distance <= safeRadius)
                    {
                        entity.follow.isFollowing = false;
                        entity.movement.moveSpeed = Vector3.zero;
                    }
                    else
                    {
                        float newMoveSpeed = entity.follow.followStep;     //默认步长
                        //如果检测半径小于步长,则步长变小
                        float dStep = Mathf.Abs(distance - (entity.follow.followStep * Time.deltaTime));
                        if (dStep < safeRadius)
                        {
                            newMoveSpeed = dStep;
                        }

                        //修改长度
                        MathUtil.ChangeVectorLength(newMoveSpeed, ref abVec, ref abVec);
                        NormalizeZero(ref abVec, ref abVec);

                        //球的参数方程
                        entity.movement.moveSpeed = abVec;

                    }
                }
            }

        }
    }
}

