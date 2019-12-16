using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace STGU3D
{
    public class BombReactiveSystem : ReactiveSystem<GameEntity>
    {
        public BombReactiveSystem(Contexts contexts) : base(contexts.game)
        {

        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(
                GameMatcher.AllOf(GameMatcher.Bomb)
            );
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.hasBomb;
        }

        public void Bome(GameEntity entity)
        {
            Debug.Log("Bomb");
            //决死Time消耗2颗Bomb,不够则无效Bomb
            if (entity.hasHealth)
            {
                if (entity.health.blood <= 0 && !entity.health.isTrueDied)
                {
                    if (entity.bomb.times - 2 >= 0)   //资源是否充足
                    {
                        //决死复活
                        entity.health.blood = entity.health.maxBlood;
                        entity.ReplaceComponent(GameComponentsLookup.Health, entity.health);

                        entity.bomb.times = entity.bomb.times - 2;
                    }

                }
            }
            else
            {
                entity.bomb.times--;
            }
        }

        protected override void Execute(List<GameEntity> entities)
        {
            foreach (var entity in entities)
            {
                if (entity.bomb.times > 0)
                {
                    if (entity.bomb.isBombing)
                    {
                        if (entity.bomb.nextBombTime <= Time.fixedTime)
                        {
                            Bome(entity);
                            entity.bomb.nextBombTime = Time.fixedTime + entity.bomb.cdTime;
                        }
                    }
                }
                entity.bomb.isBombing = false;
            }
        }
    }
}

