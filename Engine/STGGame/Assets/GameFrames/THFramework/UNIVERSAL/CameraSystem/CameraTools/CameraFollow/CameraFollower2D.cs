using UnityEngine;
using XLibrary.Package;
using THGame.Tween;

/* TODO:
 * 2d摄像机,始终对准主角
 * 存在动画效果
 * 
 */
[RequireComponent(typeof(Camera))]
public class CameraFollower2D : MonoSingleton<CameraFollower2D>
{
    public Rect rangeArea = new Rect(10,10,10,10);      //跟随区域      --超过这个区域会进行跟随
    public float reboundSpeed = 2;                      //回弹速度

    public GameObject observed;                         //被观察的对象

    public void SetTarget(GameObject target)
    {
        observed = target;
    }

    public GameObject GetTarget()
    {
        return observed;
    }

    private void Update()
    {
        if (observed == null)
            return;

        UpdatePosition();

    }

    //如果目标离摄像机中心过远,将镜头拉回到人物身上
    private void UpdatePosition()
    {
        //将人物固定会中心

        //如果超出安全区,将人物拉回摄像机中心
    }
}
