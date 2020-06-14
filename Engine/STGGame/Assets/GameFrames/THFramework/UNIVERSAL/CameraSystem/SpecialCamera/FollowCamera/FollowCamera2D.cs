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
    public Vector2 cameraSize = new Vector2(17, 8);      //摄像机区域尺寸
    public Vector2 forceSize = new Vector2(5,2.7f);          //聚焦区域尺寸
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

#if UNITY_EDITOR
    //画出摄像机范围
    private void OnDrawGizmos()
    {
        //摄像机范围
        DrawSize(transform.position, cameraSize, Color.red);
        DrawSize(transform.position, forceSize, Color.yellow);
    }

    private void DrawSize(Vector3 center,Vector3 size, Color color)
    {
        Vector3 leftTop = new Vector3(center.x - size.x / 2, center.y + size.y / 2);
        Vector3 rightTop = new Vector3(center.x + size.x / 2, center.y + size.y / 2);
        Vector3 leftBottom = new Vector3(center.x - size.x / 2, center.y - size.y / 2);
        Vector3 rightBottom = new Vector3(center.x + size.x / 2, center.y - size.y / 2);

        Gizmos.color = color;
        Gizmos.DrawLine(leftTop, rightTop); // UpperLeft -> UpperRighty
        Gizmos.DrawLine(rightTop, rightBottom); // UpperRight -> LowerRight
        Gizmos.DrawLine(rightBottom, leftBottom); // LowerRight -> LowerLeft
        Gizmos.DrawLine(leftBottom, leftTop); // LowerLeft -> UpperLeft
    }
#endif
}
