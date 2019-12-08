using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace STGGame
{
    public class MathUtil
    {
        public static readonly float FLOAT_PRECISION = 0.01f;     //浮点比较精度

        /// <summary>
        /// 浮点0比较
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool CompareZero(float val)
        {
            return Mathf.Abs(val) < FLOAT_PRECISION ? true: false;
        }

        /// <summary>
        /// 浮点0精度规范化化
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static float NormalizeZero(float val)
        {
            return Mathf.Abs(val) < FLOAT_PRECISION ? 0f : val;
        }

        public static Vector3 NormalizeZero(in Vector3 inVec, out Vector3 outVec)
        {
            outVec.x = NormalizeZero(inVec.x);
            outVec.y = NormalizeZero(inVec.y);
            outVec.z = NormalizeZero(inVec.z);
            return outVec;
        }

        /// <summary>
        /// 弧度转角度
        /// </summary>
        /// <param name="radian"></param>
        /// <returns></returns>
        public static float Rad2Deg(float radian)
        {
            return radian / Mathf.Deg2Rad;
        }

        /// <summary>
        /// 角度转弧度
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static float Deg2Rad(float angle)
        {
            return angle * Mathf.Deg2Rad;
        }

        /// <summary>
        /// 向量夹角360
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static float Angle360(Vector3 from, Vector3 to)
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

        /// <summary>
        /// 向量夹角180
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static float Angle180(Vector3 from, Vector3 to)
        {
            return Vector3.Angle(from, to);
        }

        /// <summary>
        /// 修改向量长度
        /// </summary>
        /// <param name="radius">大小</param>
        /// <param name="inVec">原向量</param>
        /// <param name="outVec">输出向量</param>
        /// <returns></returns>
        public static Vector3 ChangeVectorLength(float radius, in Vector3 inVec, out Vector3 outVec)
        {
            Vector3 direction = inVec.normalized;
            float theta = Deg2Rad(Angle360(Vector3.right, direction));     //与X轴夹角
            float lambda = Deg2Rad(Angle180(Vector3.forward, direction));  //与平面XOY的夹角

            //球的参数方程
            outVec.x = radius * Mathf.Cos(theta) * Mathf.Sin(lambda);
            outVec.y = radius * Mathf.Sin(theta) * Mathf.Sin(lambda);
            outVec.z = radius * Mathf.Cos(lambda);

            return outVec;
        }


        /// <summary>
        /// 修改向量方向
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="inVec">原向量</param>
        /// <param name="outVec">输出向量</param>
        /// <returns></returns>
        public static Vector3 ChangeVectorDirection(Vector3 newVec, in Vector3 inVec, out Vector3 outVec)
        {
            float oldLength = inVec.magnitude;

            return ChangeVectorLength(oldLength, in inVec, out outVec);
        }
    }

}

