using Entitas;
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

        public float Angle360(Vector3 from, Vector3 to)
        {
            Vector3 v3 = Vector3.Cross(from, to);
            if (v3.z >= 0f)
            {
                return Vector3.Angle(from, to);
            }
            else
            {
                return 360 - Vector3.Angle(from, to);
            }
        }

        public float Angle180(Vector3 from, Vector3 to)
        {
            return Vector3.Angle(from, to); 
        }

        //精度化
        public float NormalZero(float val)
        {
            return Mathf.Abs(val) < PRECISION ? 0f : val;
        }

        public void Execute()
        {
            foreach (var entity in __followGroup.GetEntities())
            {
                if (entity.follow.isFollowing)
                {
                    //是否在目标点半径内
                    Vector3 abVec = (entity.follow.destination - entity.transform.localPosition);
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

                        Vector3 direction = abVec.normalized;
                        float theta = Angle360(Vector3.right, direction) * Mathf.PI / 180f;     //与X轴夹角
                        float lambda = Angle180(Vector3.forward, direction) * Mathf.PI / 180f;  //与平面XOY的夹角

                        //球的参数方程
                        float newX = NormalZero(newMoveSpeed * Mathf.Cos(theta) * Mathf.Sin(lambda));
                        float newY = NormalZero(newMoveSpeed * Mathf.Sin(theta) * Mathf.Sin(lambda));
                        float newZ = NormalZero(newMoveSpeed * Mathf.Cos(lambda));

                        Vector3 newSpeed = new Vector3(newX, newY, newZ);
                        entity.movement.moveSpeed = newSpeed;
                    }
                }
            }

        }
    }
}

