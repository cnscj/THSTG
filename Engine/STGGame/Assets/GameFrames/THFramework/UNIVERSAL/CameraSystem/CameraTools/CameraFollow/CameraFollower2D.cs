using UnityEngine;

/*
 * 2d摄像机,始终对准主角
 * 存在动画效果
 * 
 */
[RequireComponent(typeof(Camera))]
public class CameraFollower2D : MonoBehaviour
{
    private static CameraFollower2D s_instance;
    public static CameraFollower2D GetInstance()
    {
        return s_instance;
    }

    public float distance = 0;          //人物到摄像机距离
    public float maxLeftDistance = 0;   //
    public float maxRightDistance = 0;  //
    public float reboundSpeed = 0;      //回弹速度

    public GameObject observed;         //被观察的对象



    public void SetTarget(GameObject target)
    {
        observed = target;
    }

    public GameObject GetTarget()
    {
        return observed;
    }


    private void Awake()
    {
        s_instance = this;
    }

    private void Update()
    {
        if (observed == null)
            return;

    }

    private void Tween2Target()
    {
        if (observed == null)
            return;
    }
}
