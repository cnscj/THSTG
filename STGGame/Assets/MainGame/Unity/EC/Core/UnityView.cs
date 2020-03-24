
using Entitas.Unity;
using STGGame;
using UnityEngine;
using XLibGame;

namespace STGU3D
{
    public class UnityView : IView
    {
        public delegate void Callback0();
        public GameEntity entity;           //GE
        public GameObject node;             //与Unity关联的节点
        public ViewControllerMono viewCtrl; //View 控制器
        public Callback0 onComplete;        //加载完成回调

        private System.Numerics.Vector3 m_pos = System.Numerics.Vector3.Zero;
        private System.Numerics.Vector3 m_rot = System.Numerics.Vector3.Zero;
        private System.Numerics.Vector3 m_scale = System.Numerics.Vector3.One;
        public static UnityEngine.Vector3 SysVec3ToU3dVec3(ref System.Numerics.Vector3 sysVec3, ref UnityEngine.Vector3 u3dVec3)
        {
            u3dVec3.x = sysVec3.X;
            u3dVec3.y = sysVec3.Y;
            u3dVec3.z = sysVec3.Z;
            return u3dVec3;
        }

        public static System.Numerics.Vector3 U3dVec3ToSysVec3(ref UnityEngine.Vector3 u3dVec3, ref System.Numerics.Vector3 sysVec3)
        {
            sysVec3.X = u3dVec3.x;
            sysVec3.Y = u3dVec3.y;
            sysVec3.Z = u3dVec3.z;
            return sysVec3;
        }

        public System.Numerics.Vector3 Position
        {
            get
            {
                if (node == null) return m_pos;
                var position = node.transform.position;
                return U3dVec3ToSysVec3(ref position, ref m_pos);
            }
            set
            {
                m_pos = value;
                if (node == null) return;
                var position = node.transform.position;
                node.transform.position = SysVec3ToU3dVec3(ref m_pos, ref position);
            }
        }
        public System.Numerics.Vector3 Rotation
        {
            get
            {
                if (node == null) return m_rot;
                var rotation = node.transform.eulerAngles;
                return U3dVec3ToSysVec3(ref rotation, ref m_rot);
            }
            set
            {
                m_rot = value;
                if (node == null) return;
                var rotation = node.transform.eulerAngles;
                node.transform.eulerAngles = SysVec3ToU3dVec3(ref m_rot, ref rotation);
            }
        }
        public System.Numerics.Vector3 Scale
        {
            get
            {
                if (node == null) return m_scale;
                var scale = node.transform.localScale;
                return U3dVec3ToSysVec3(ref scale, ref m_scale); ;
            }
            set
            {
                m_scale = value;
                if (node == null) return;
                var localScale = node.transform.localScale;
                node.transform.localScale = SysVec3ToU3dVec3(ref m_scale, ref localScale);
            }
        }

        public void Clear()
        {
            if (node != null)
            {
                var entityLink = node.GetComponent<EntityLink>();
                if (entityLink != null)
                {
                    if (entityLink.entity != null)
                    {
                        entityLink.Unlink();
                    }
                }
                if (viewCtrl != null)
                {
                    viewCtrl.Destroy();
                }
                //TODO:应该送入缓存区
                Object.Destroy(node);
            }
            node = null;
            entity = null;
        }

        //这里需要用协程延迟一帧
        //原因:关系到node是否绑定,如果不为空,则直接绑定
        public IView Create(GameEntity ent)
        {
            Clear();

            entity = ent;

            Timer.GetInstance().ScheduleNextFrame(InitView);

            return this;
        }

        public void AddView(string code, System.Action<GameObject> callback = null)
        {
            if (viewCtrl != null && entity != null)
            {
                viewCtrl.AddView(code, callback);
            }
        }

        ///

        public object Execute(int operate, object data = null)
        {

            return null;
        }

        bool LinkGOAndEntity(GameObject go, GameEntity ent)
        {
            if (go != null && ent != null)
            {
                var entityLink = go.GetComponent<EntityLink>();
                if (entityLink != null)
                {
                    entityLink.Unlink();
                }
                else
                {
                    entityLink = go.AddComponent<EntityLink>();
                }
                entityLink.Link(ent);
                return true;
            }
            return false;

        }

        //这里用协程关系到node是否绑定,如果不为空,则直接绑定
        void InitView()
        {
            if (node == null)
            {
                node = new GameObject("View");
            }
           
            if (LinkGOAndEntity(node, entity))
            {
                if (entity.hasEntityData)
                {
                    MoveNode(entity);
                    AddViewController(entity);
                    InitNode(entity);

                    onComplete?.Invoke();
                    onComplete = null;
                }
            }
        }

        void AddViewController(GameEntity ent)
        {
            if (viewCtrl != null)
            {
                Object.Destroy(viewCtrl);
            }

            if (node != null)
            {
                viewCtrl = node.AddComponent<ViewControllerMono>();
                viewCtrl.Ceate(this);
            }
        }

        void MoveNode(GameEntity ent)
        {
            if (ent.isEditorEntity)
                return;
  
            var entityType = ent.entityData.entityType;

            switch (entityType)
            {
                case EEntityType.Hero:
                    node.transform.SetParent(EntityManager.GetInstance().heroRoot.transform, false);
                    break;
                case EEntityType.Mob:
                    node.transform.SetParent(EntityManager.GetInstance().mobRoot.transform, false);
                    break;
                case EEntityType.Bullet:
                    node.transform.SetParent(EntityManager.GetInstance().bulletRoot.transform, false);
                    break;
                case EEntityType.Prop:
                    node.transform.SetParent(EntityManager.GetInstance().propRoot.transform, false);
                    break;
                case EEntityType.Wingman:
                    node.transform.SetParent(EntityManager.GetInstance().wingmanRoot.transform, false);
                    break;
            }
        }

        void InitNode(GameEntity ent)
        {
            var position = node.transform.position;
            var rotation = node.transform.eulerAngles;
            var localScale = node.transform.localScale;
            node.transform.position = SysVec3ToU3dVec3(ref m_pos, ref position);
            node.transform.eulerAngles = SysVec3ToU3dVec3(ref m_rot, ref rotation);
            node.transform.localScale = SysVec3ToU3dVec3(ref m_scale, ref localScale);

            if (ent.hasTransform)
            {
                if (ent.isEditorEntity)
                {
                    ent.transform.position = node.transform.position;
                    ent.transform.rotation = node.transform.eulerAngles;
                }
                else
                {
                    node.transform.position = ent.transform.position;
                    node.transform.eulerAngles = ent.transform.rotation;
                }
            }
        }
    }
}
