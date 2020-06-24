using UnityEngine;
using XLibrary.Package;

/* TODO:
 * 2d摄像机,始终对准主角
 * 存在动画效果
 * 能够向各个方向观察一段距离
 */
public class FollowCamera2D : MonoBehaviour
{
    public new Camera camera;                                   //非必要
    public Vector2 cameraSize = new Vector2(17, 8);             //摄像机区域尺寸
    public Vector2 forceSize = new Vector2(5,2.7f);             //聚焦区域尺寸
    public float reboundTime = 0.5f;                            //回弹时间

    public Transform observed;                                  //被观察的对象

    private Vector3 m_velocity;                                 //

    public void SetTarget(Transform target)
    {
        observed = target;
    }

    public Transform GetTarget()
    {
        return observed;
    }

    private void FixedUpdate()
    {
        if (observed == null)
            return;

        UpdatePosition();
    }

    //采用 双向正前聚焦 模式
    //如果目标离摄像机中心过远,将镜头拉回到人物身上
    //采用Vector3.SmoothDamp()平滑过渡
    //为了获取更大视野,靠近最左边界时,摄像机向右移动
    private void UpdatePosition()
    {
        ////将人物固定到聚焦区域,并缓慢拉回中心
        //var destPoint = observed.transform.position;
        //var srcPoint = transform.position;
        //var shiftVec = destPoint - srcPoint;
        //var shiftLen = shiftVec.magnitude;
        //var moveStepVec = shiftVec.normalized * reboundSpeed;
        //var moveStepLen = moveStepVec.magnitude;

        ////如果超出安全区,将人物拉回摄像机中心
        //if (Mathf.Approximately(shiftLen, moveStepLen))
        //    return;

        ////边界判断



        //平滑过渡,这里如果速度过快会发生抖动
        transform.position = new Vector3(
            Mathf.SmoothDamp(transform.position.x, observed.position.x, ref m_velocity.x, reboundTime),
            Mathf.SmoothDamp(transform.position.y, observed.position.y, ref m_velocity.y, reboundTime),
           transform.position.z);
    }

    
    [ContextMenu("Calculate")]
    private void Calculate()
    {
        //自动设置CameraSize
        if (camera == null)
        {
            Debug.Log("[FollowCamera] Must need a camera");
            return;
        }

        var corners = GetCorners(camera, 10f);
        cameraSize.x = corners[1].x - corners[0].x;
        cameraSize.y = corners[0].y - corners[2].y;
    }

    Vector3[] GetCorners(Camera theCamera, float distance)
    {
        if (theCamera == null)
            return null;

        Transform tx = theCamera.transform;
        Vector3[] corners = new Vector3[4];

        float height = 0f;
        bool orthographic = theCamera.orthographic;  //是否是正交相机
        if (orthographic)
        {
            height = theCamera.orthographicSize;
        }
        else
        {
            float halfFOV = (theCamera.fieldOfView * 0.5f) * Mathf.Deg2Rad;
            height = distance * Mathf.Tan(halfFOV);
        }

        float aspect = theCamera.aspect;
        float width = height * aspect;

        // UpperLeft
        corners[0] = tx.position - (tx.right * width);
        corners[0] += tx.up * height;
        corners[0] += tx.forward * distance;

        // UpperRight
        corners[1] = tx.position + (tx.right * width);
        corners[1] += tx.up * height;
        corners[1] += tx.forward * distance;

        // LowerLeft
        corners[2] = tx.position - (tx.right * width);
        corners[2] -= tx.up * height;
        corners[2] += tx.forward * distance;

        // LowerRight
        corners[3] = tx.position + (tx.right * width);
        corners[3] -= tx.up * height;
        corners[3] += tx.forward * distance;

        return corners;
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
