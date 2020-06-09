using UnityEngine;

/*
 * 2d摄像机,始终对准主角
 * 存在动画效果
 * 
 */
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

    private void Awake()
    {
        s_instance = this;
    }

}
