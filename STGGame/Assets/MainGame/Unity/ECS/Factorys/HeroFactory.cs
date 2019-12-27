using System.Collections;
using System.Collections.Generic;
using STGGame;
using UnityEngine;

namespace STGU3D
{
    public class HeroFactory : BaseEntityFactory
    {
        public GameEntity CreateHero(EHeroType heroType, EPlayerType playerType = EPlayerType.Player01)
        {
            string code = EntityUtil.GetHeroCode(heroType);
            var entity = CreateEntity(code);

            var playerDataCom = entity.GetComponent(GameComponentsLookup.PlayerData) as PlayerDataComponent;
            if (playerDataCom != null)
            {
                playerDataCom.playerType = playerType;
                EntityCache.GetInstance().SetHero(playerType, entity);
            }

            return entity;
        }

        //创建实体以及相应的组件
        protected override GameEntity OnCreate(string code)
        {
            var entity = CreateGameEntity(code);

            var commandCom = entity.CreateComponent<CommandComponent>(GameComponentsLookup.Command);
            var playerDataCom = entity.CreateComponent<PlayerDataComponent>(GameComponentsLookup.PlayerData);
            var shotCom = entity.CreateComponent<ShotComponent>(GameComponentsLookup.Shot);
            var bombCom = entity.CreateComponent<BombComponent>(GameComponentsLookup.Bomb);
            var decelerateCom = entity.CreateComponent<DecelerateComponent>(GameComponentsLookup.Decelerate);
            var eliminateCom = entity.CreateComponent<EliminateComponent>(GameComponentsLookup.Eliminate);
            var lifeCom = entity.CreateComponent<LifeComponent>(GameComponentsLookup.Life);
            var healthCom = entity.CreateComponent<HealthComponent>(GameComponentsLookup.Health);
            var cageCom = entity.CreateComponent<CageComponent>(GameComponentsLookup.Cage);
            var invincibleCom = entity.CreateComponent<InvincibleComponent>(GameComponentsLookup.Invincible);

            entity.AddComponent(GameComponentsLookup.Command, commandCom);
            entity.AddComponent(GameComponentsLookup.PlayerData, playerDataCom);
            entity.AddComponent(GameComponentsLookup.Shot, shotCom);
            entity.AddComponent(GameComponentsLookup.Bomb, bombCom);
            entity.AddComponent(GameComponentsLookup.Decelerate, decelerateCom);
            entity.AddComponent(GameComponentsLookup.Eliminate, eliminateCom);
            entity.AddComponent(GameComponentsLookup.Life, lifeCom);
            entity.AddComponent(GameComponentsLookup.Health, healthCom);
            entity.AddComponent(GameComponentsLookup.Invincible, invincibleCom);
            entity.AddComponent(GameComponentsLookup.Cage, cageCom);

            return entity;
        }

        //初始化组件
        protected override void OnInit(GameEntity entity)
        {
            if (entity.hasEntityData)
            {
                var heroType = EntityUtil.GetHeroTypeByCode(entity.entityData.entityCode);
                entity.playerData.heroType = heroType;

                //根据
                entity.shot.action = ShotBullet;

                {
                    entity.view.view = ComponentUtil.CreateView(entity);
                    entity.ReplaceComponent(GameComponentsLookup.View, entity.view);
                }

                {
                    entity.cage.movableArea = DirectorUtil.ScreenRectInWorld(DirectorUtil.GetScreenRect());
                    entity.cage.bodySize = DirectorUtil.ScreenSizeInWorld(new Vector2(32, 48)); //TODO:这里应该查找hitbox
                }

                entity.playerData.moveSpeed = entity.entityData.entityData["speed"].ToFloat();
                entity.health.maxBlood = entity.entityData.entityData["blood"].ToInt();

                //不同主角特有的
                if (heroType == EHeroType.Reimu)
                {
                    //Reimu持有僚机total台:onmyougyoku
                    float[] d = { -0.5f, 0.5f };
                    int total = 2;
                    for (int i = 0; i < total; i++)
                    {
                        var onmyougyokuWingman = EntityManager.GetInstance().GetOrNewEntityFactory(EEntityType.Wingman).AsWingman().CreateWingman(EWingmanType.Onmyougyoku);
                        if (onmyougyokuWingman.hasEntityData)
                        {
                            onmyougyokuWingman.onmyougyokuWingman.id = i;
                            onmyougyokuWingman.movement.rotationSpeed.z = 100f;                         //自旋
                            onmyougyokuWingman.transform.parent = entity.transform;
                            onmyougyokuWingman.transform.localPosition = new Vector3(d[i], 0.5f, 0);       //偏移一点
                        }
                    }

                }

            }
        }

        protected override void OnDestroy(GameEntity entity)
        {
            if (entity.hasPlayerData)
            {
                EPlayerType playerType = entity.playerData.playerType;
                EntityCache.GetInstance().SetHero(playerType, null);
            }
            base.DestroyEntity(entity);
        }



        private GameEntity ShotBullet(GameEntity shotEntity)
        {
            var bulletEntity = EntityManager.GetInstance().GetOrNewEntityFactory(EEntityType.Bullet).AsBullet().CreateBullet(ECampType.Hero, shotEntity.entityData.entityData["bulletCode"]);
            bulletEntity.transform.position = shotEntity.transform.position;                                                                                                          //在自机处生成

            return bulletEntity;
        }
    }

}
