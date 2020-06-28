using UnityEngine;
using XLibrary.Package;

/* TODO:
 * 2d摄像机,始终对准主角
 * 存在动画效果
 * 能够向各个方向观察一段距离
 */
public class FollowCamera2D : MonoBehaviour
{
    public new Camera camera;                                   //必要

    public Rect followRect = new Rect(0, 0, 1, 1);              //聚焦区域
    public Rect forcusRect = new Rect(0, 0, 1, 1);              //聚焦区域
    public bool isWideAngle = false;                            //广角模式
    public float reboundTime = 0.5f;                            //回弹时间

    public Transform observed;                                  //被观察的对象

    private Vector3 m_velocity;                                 //
    private Vector3 m_tempPosition = Vector3.zero;              //
    private Vector3 m_lastObservedPosition = Vector3.zero;

    private Vector3 m_moveDirection = Vector3.zero;
    public void SetTarget(Transform target)
    {
        observed = target;
    }

    public Transform GetTarget()
    {
        return observed;
    }

    private void LateUpdate()
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
        var observedPosition = observed.transform.position;
        var destPosition = new Vector3(observedPosition.x - followRect.x, observedPosition.y - followRect.y, observedPosition.z);
        var srcPoint = transform.position;

        //边界判断,越界就不用再平滑了
        m_tempPosition = srcPoint;
        if (destPosition.x < srcPoint.x - followRect.width / 2)
        {
            m_tempPosition.x = destPosition.x + followRect.width / 2;
        }
        else if (destPosition.x > srcPoint.x + followRect.width / 2)
        {
            m_tempPosition.x = destPosition.x - followRect.width / 2;
        }

        if (destPosition.y < srcPoint.y - followRect.height / 2)
        {
            m_tempPosition.y = destPosition.y + followRect.height / 2;
        }
        else if (destPosition.y > srcPoint.y + followRect.height / 2)
        {
            m_tempPosition.y = destPosition.y - followRect.height / 2;
        }
        transform.position = m_tempPosition;    //限制摄像机范围
        srcPoint = transform.position;

        //平滑过渡
        if (Mathf.Approximately(reboundTime,0f))
        {
            m_tempPosition = destPosition;
        }
        else
        {
            if (isWideAngle) //以边界作为准点,获得更多的视野
            {
                //求出要移动的方向,将扩展目标
                if(destPosition.x - m_lastObservedPosition.x > 0)
                {
                    m_moveDirection.x = 1;
                    
                }
                else if(destPosition.x - m_lastObservedPosition.x < 0)
                {
                    m_moveDirection.x = -1;
                    
                }

                if (m_moveDirection.x > 0)
                {
                    destPosition.x += followRect.width / 2;
                }
                else if (m_moveDirection.x < 0)
                {
                    destPosition.x -= followRect.width / 2;
                }
            }
            m_tempPosition.x = Mathf.SmoothDamp(srcPoint.x, destPosition.x, ref m_velocity.x, reboundTime);
            m_tempPosition.y = Mathf.SmoothDamp(srcPoint.y, destPosition.y, ref m_velocity.y, reboundTime);
        }

        transform.position = m_tempPosition;
        m_lastObservedPosition = observedPosition;
    }

#if UNITY_EDITOR
    //画出摄像机范围
    private void OnDrawGizmos()
    {
        //摄像机范围
        DrawForceRect(transform.position, followRect, Color.yellow);
    }

    private void DrawCameraSize(Vector3 center,Vector3 size, Color color)
    {
        Vector3 leftTop = new Vector3(center.x - size.x / 2, center.y + size.y / 2);
        Vector3 rightTop = new Vector3(center.x + size.x / 2, center.y + size.y / 2);
        Vector3 leftBottom = new Vector3(center.x - size.x / 2, center.y - size.y / 2);
        Vector3 rightBottom = new Vector3(center.x + size.x / 2, center.y - size.y / 2);
        DrawPoints(new Vector3[] { leftTop, rightTop, rightBottom,leftBottom}, color, true);
    }

    private void DrawForceRect(Vector3 center, Rect rect, Color color)
    {
        float newCenterX = center.x + rect.x;
        float newCenterY = center.y + rect.y;

        DrawRect(new Rect(newCenterX, newCenterY, rect.width, rect.height), color);
    }

    private void DrawRect(Rect rect, Color color)
    {
        Vector3 leftTop = new Vector3(rect.xMin, rect.yMax);
        Vector3 leftTopRight = new Vector3(rect.xMax, rect.yMax);
        Vector3 leftBottom = new Vector3(rect.xMin, rect.yMin);
        Vector3 leftBottomRight = new Vector3(rect.xMax, rect.yMin);

        DrawPoints(new Vector3[] { leftTopRight, leftTop, leftBottom, leftBottomRight }, color, false);
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
