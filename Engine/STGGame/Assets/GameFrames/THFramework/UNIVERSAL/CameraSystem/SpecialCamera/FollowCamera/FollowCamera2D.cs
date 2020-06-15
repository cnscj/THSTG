using UnityEngine;
using XLibrary.Package;
using THGame.Tween;

/* TODO:
 * 2d摄像机,始终对准主角
 * 存在动画效果
 * 能够向各个方向观察一段距离
 */
public class FollowCamera2D : MonoSingleton<FollowCamera2D>
{
    //public new Camera camera;
    public Vector2 cameraSize = new Vector2(17, 8);             //摄像机区域尺寸
    public Vector2 forceSize = new Vector2(5,2.7f);             //聚焦区域尺寸
    public float reboundSpeed = 3f;                             //回弹速度

    public GameObject observed;                                 //被观察的对象

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
        //将人物固定到聚焦区域,并缓慢拉回中心
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
        DrawCameraSize(transform.position, cameraSize, Color.red);
        DrawForceSize(transform.position, forceSize, Color.yellow);
    }

    private void DrawCameraSize(Vector3 center,Vector3 size, Color color)
    {
        Vector3 leftTop = new Vector3(center.x - size.x / 2, center.y + size.y / 2);
        Vector3 rightTop = new Vector3(center.x + size.x / 2, center.y + size.y / 2);
        Vector3 leftBottom = new Vector3(center.x - size.x / 2, center.y - size.y / 2);
        Vector3 rightBottom = new Vector3(center.x + size.x / 2, center.y - size.y / 2);
        DrawPoints(new Vector3[] { leftTop, rightTop, rightBottom,leftBottom}, color, true);
    }

    private void DrawForceSize(Vector3 center, Vector3 size, Color color)
    {
        Vector3 leftTop = new Vector3(center.x - size.x / 2, center.y + size.y / 2);
        Vector3 leftTopRight = new Vector3(center.x - size.x / 3, center.y + size.y / 2);
        Vector3 leftBottom = new Vector3(center.x - size.x / 2, center.y - size.y / 2);
        Vector3 leftBottomRight = new Vector3(center.x - size.x / 3, center.y - size.y / 2);
        DrawPoints(new Vector3[] { leftTopRight, leftTop, leftBottom, leftBottomRight }, color, false);

        Vector3 rightTop = new Vector3(center.x + size.x / 2, center.y + size.y / 2);
        Vector3 rightTopLeft = new Vector3(center.x + size.x / 3, center.y + size.y / 2);
        Vector3 rightBottom = new Vector3(center.x + size.x / 2, center.y - size.y / 2);
        Vector3 rightBottomLeft = new Vector3(center.x + size.x / 3, center.y - size.y / 2);
        DrawPoints(new Vector3[] { rightTopLeft, rightTop, rightBottom, rightBottomLeft }, color, false);
    }

    private void DrawPoints(Vector3[] points, Color color, bool isClose)
    {
        if (points == null || points.Length <= 1)
            return;

        Gizmos.color = color;
        int i;
        for (i = 0; i < points.Length - 1; i++)
        {
            Gizmos.DrawLine(points[i], points[i+1]);
        }
        if (isClose)
        {
            Gizmos.DrawLine(points[i], points[0]);
        }
    }
#endif
}
