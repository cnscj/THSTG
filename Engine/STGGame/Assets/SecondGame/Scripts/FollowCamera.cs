using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 相机从一个点到另一个点的平滑过渡,平滑缓冲
/// </summary>
public class FollowCamera : MonoBehaviour
{
    public GameObject cameraon;//初始摄像机的位置
    public GameObject camerto;//另一个点的位置
    public float speed = 1f;//缓冲的时间  时间越大缓冲速度越慢
    private Vector3 velocity;//如果是3D场景就用Vector3,2D用Vector2
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {

        cameraon.transform.position = new Vector3(
            SmoothDamp(cameraon.transform.position.x, camerto.transform.position.x, ref velocity.x, 0.5f, speed, Time.deltaTime),
            SmoothDamp(cameraon.transform.position.y, camerto.transform.position.y, ref velocity.y, 0.5f, speed, Time.deltaTime),
            cameraon.transform.position.z);

    }

    public static float Clamp(float value,float min,float max)
    {
        value = Mathf.Max(value, min);
        value = Mathf.Min(value, max);
        return value;
    }



    // current: 当前的位置
    // target: 我们试图接近的位置
    // currentVelocity: 当前速度，这个值由你每次调用这个函数时被修改
    // smoothTime: 到达目标的大约时间，较小的值将快速到达目标
    // maxSpeed: 选择允许你限制的最大速度
    // deltaTime: 自上次调用这个函数的时间。默认为 Time.deltaTime
    public static float SmoothDamp(float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed, float deltaTime)
    {
        smoothTime = Mathf.Max(0.0001f, smoothTime);
        float num1 = 2f / smoothTime;
        float num2 = num1 * deltaTime;
        float num3 = (float)(1.0 / (1.0 + (double)num2 + 0.479999989271164 * (double)num2 * (double)num2 + 0.234999999403954 * (double)num2 * (double)num2 * (double)num2));
        float num4 = current - target;
        float num5 = target;
        float max = maxSpeed * smoothTime;
        float num6 = Clamp(num4, -max, max);
        target = current - num6;
        float num7 = (currentVelocity + num1 * num6) * deltaTime;
        currentVelocity = (currentVelocity - num1 * num7) * num3;
        float num8 = target + (num6 + num7) * num3;
        if ((double)num5 - (double)current > 0.0 == (double)num8 > (double)num5)
        {
            num8 = num5;
            currentVelocity = (num8 - num5) / deltaTime;
        }
        return num8;
    }

}