
using XLibrary;

namespace STGU3D
{
    public abstract class BaseEntityFactory
    {
        //唯一实体创建
        public GameEntity CreateEntity(string code)
        {
            var entity = OnCreate(code);
            OnInit(entity);
            return entity;
        }

        //唯一实体销毁
        public void DestroyEntity(GameEntity entity)
        {
            OnDestroy(entity);
        }

        //转换方法
        public HeroFactory AsHero(){ return (HeroFactory)this; }
        public BulletFactory AsBullet() { return (BulletFactory)this; }
        public WingmanFactory AsWingman() { return (WingmanFactory)this; }
        public MobFactory AsMob() { return (MobFactory)this; }
        public BossFactory AsBoss() { return (BossFactory)this; }
        public PropFactory AsProp() { return (PropFactory)this; }

        //创建实体以及相应的组件
        protected virtual GameEntity OnCreate(string code)
        {

            return null;
        }

        //初始化组件
        protected virtual void OnInit(GameEntity entity)
        {
            
        }
        //销毁实体
        protected virtual void OnDestroy(GameEntity entity)
        {
            if (entity.hasDestroyed)
            {
                var destroyedCom = entity.GetComponent(GameComponentsLookup.Destroyed) as DestroyedComponent;
                destroyedCom.isDestroyed = true;
                entity.ReplaceComponent(GameComponentsLookup.Destroyed, destroyedCom);
            }
            else
            {
                entity.Destroy();
            }
        }

        //通用方法
        public GameEntity CreateEmptyEntity()
        {
            var entity = Contexts.sharedInstance.game.CreateEntity();

            return entity;
        }

        public GameEntity CreateGameEntity(string code)
        {
            var entity = CreateEmptyEntity();
            AddCommonComponent(entity, code);
            InitCommonComponent(entity, code);
            return entity;
        }


        protected void AddCommonComponent(GameEntity entity, string code)
        {
            //用这个法子创建的组件是复用原有的,因此必须手动初始化下
            var transCom = entity.CreateComponent<TransformComponent>(GameComponentsLookup.Transform);
            var colliderCom = entity.CreateComponent<ColliderComponent>(GameComponentsLookup.Collider);
            var movementCom = entity.CreateComponent<MovementComponent>(GameComponentsLookup.Movement);
            var viewCom = entity.CreateComponent<ViewComponent>(GameComponentsLookup.View);
            var destroyCom = entity.CreateComponent<DestroyedComponent>(GameComponentsLookup.Destroyed);

            EEntityType type = EntityUtil.GetEntityTypeByCode(code);
            CSVObject infos = EntityConfiger.GetEntityInfo(code);
            if (infos != null)
            {
                var entityDataCom = entity.CreateComponent<EntityDataComponent>(GameComponentsLookup.EntityData);
                entityDataCom.entityCode = code;
                entityDataCom.entityType = type;
                entityDataCom.entityData = infos;
                entity.AddComponent(GameComponentsLookup.EntityData, entityDataCom);
            }

            ////
            entity.AddComponent(GameComponentsLookup.Transform, transCom);
            entity.AddComponent(GameComponentsLookup.Collider, colliderCom);
            entity.AddComponent(GameComponentsLookup.Movement, movementCom);
            entity.AddComponent(GameComponentsLookup.View, viewCom);
            entity.AddComponent(GameComponentsLookup.Destroyed, destroyCom);
        }

        protected void InitCommonComponent(GameEntity entity, string code)
        {
            ComponentUtil.ClearTransform(entity.transform);
            ComponentUtil.ClearCollider(entity.collider);
            ComponentUtil.ClearMovement(entity.movement);
            ComponentUtil.ClearView(entity.view);
            ComponentUtil.ClearDestroyed(entity.destroyed);
        }
    }
}
