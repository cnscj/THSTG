using System.Collections;
using System.Collections.Generic;
using STGGame;
using UnityEngine;

namespace STGU3D
{
    public class HeroFactory : BaseEntityFactory
    {
        
        public override GameEntity CreateEntity(string code)
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

            if (entity.hasEntityData)
            {
                var heroType = EntityUtil.GetHeroTypeByCode(code);
                playerDataCom.heroType = heroType;

                //根据
                shotCom.action = (shotEntity) =>
                {
                    var bulletEntity = EntityManager.GetInstance().GetOrNewEntityFactory(EEntityType.Bullet).AsBullet().CreateBullet(ECampType.Hero, shotEntity.entityData.entityData["bulletCode"]);
                    bulletEntity.transform.localPosition = shotEntity.transform.localPosition;                          //在自机处生成
                    bulletEntity.view.viewGO.transform.localPosition = shotEntity.transform.localPosition;              //覆盖第一帧刷新


                    return bulletEntity;
                };

                {
                    entity.view.viewGO = NewViewNode(false, entity.entityData.entityData["viewCode"], entity.transform.localPosition, entity.transform.localRotation);
                    entity.view.animator = entity.view.viewGO.GetComponentInChildren<Animator>();
                    entity.view.renderer = entity.view.viewGO.GetComponentInChildren<Renderer>();
                    entity.view.collider = entity.view.viewGO.GetComponentInChildren<Collider>();

                    entity.ReplaceComponent(GameComponentsLookup.View, entity.view);
                }

                {
                    cageCom.movableArea = DirectorUtil.ScreenToWorldRect(DirectorUtil.GetScreenRect());
                    cageCom.bodySize = DirectorUtil.ScreenToWorldPoint(new Vector2(32, 48)); //TODO:
                }

                entity.playerData.moveSpeed = entity.entityData.entityData["speed"].ToFloat();
                healthCom.maxBlood = entity.entityData.entityData["blood"].ToInt();

                //不同主角特有的
                if (heroType == EHeroType.Reimu)
                {
                    //Reimu持有僚机1台:onmyougyoku
                    var onmyougyokuWingman = EntityManager.GetInstance().GetOrNewEntityFactory(EEntityType.Wingman).AsWingman().CreateWingman(EWingmanType.Onmyougyoku);
                    if (onmyougyokuWingman.hasEntityData)
                    {
                        //TODO:
                        onmyougyokuWingman.movement.rotationSpeed.z = 100f;       //自旋
                        onmyougyokuWingman.transform.parent = entity.transform;
                    }
                }

            }

            return entity;
        }
        public override void DestroyEntity(GameEntity entity)
        {
            if (entity.hasPlayerData)
            {
                EPlayerType playerType = entity.playerData.playerType;
                EntityCache.GetInstance().SetHero(playerType, null);
            }
            base.DestroyEntity(entity);
        }

        public GameEntity CreateHero(EHeroType heroType, EPlayerType playerType = EPlayerType.Player01)
        {
            string code = string.Format("{0}", 10000000 + 100000 * (int)EEntityType.Hero + 1000 * (int)heroType + 1);
            var entity = CreateEntity(code);

            var playerDataCom = entity.GetComponent(GameComponentsLookup.PlayerData) as PlayerDataComponent;
            if (playerDataCom != null)
            {
                playerDataCom.playerType = playerType;
                EntityCache.GetInstance().SetHero(playerType, entity);
            }

            return entity;
        }

        
    }

}
