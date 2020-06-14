using UnityEngine;
using XLibrary.Package;
using THGame.Tween;

/* TODO:
 * 2d摄像机,始终对准主角
 * 存在动画效果
 * 
 */
public class FollowCamera2D : MonoSingleton<FollowCamera2D>
{
    //public new Camera camera;
    public Rect mapBound = new Rect(100, 100, 100, 100);    //摄像机可移动区域
    public Rect rangeArea = new Rect(10,10,10,10);          //跟随区域
    public float reboundSpeed = 3f;                         //回弹速度

    public GameObject observed;                             //被观察的对象

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
        UpdateDebug();
    }

    //采用 双向正前聚焦 模式
    //如果目标离摄像机中心过远,将镜头拉回到人物身上
    //采用Vector3.SmoothDamp()平滑过渡

    private void UpdatePosition()
    {
        //将人物固定回中心
        var destPoint = observed.transform.position;
        var srcPoint = transform.position;
        var shiftVec = destPoint - srcPoint;
        var shiftLen = shiftVec.magnitude;
        var moveStepVec = shiftVec.normalized * reboundSpeed;
        var moveStepLen = moveStepVec.magnitude;

        //如果超出安全区,将人物拉回摄像机中心
        if (Mathf.Approximately(shiftLen, moveStepLen))
            return;

        //边界判断


        if (moveStepLen > shiftLen)
        {
            destPoint.z = transform.position.z;
            transform.position = destPoint;
        }
        else
        {
            moveStepVec.z = 0;
            transform.position += moveStepVec;

        }

    }

    private void UpdateDebug()
    {
        DrawCameraRect();
    }

    //画出摄像机范围
    private void DrawCameraRect()
    {
        //摄像机范围

    }
}
