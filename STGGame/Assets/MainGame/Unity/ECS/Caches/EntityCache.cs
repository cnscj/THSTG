using System.Collections.Generic;
using STGGame;
using XLibrary.Collection;
using XLibrary.Package;

namespace STGU3D
{
    public class EntityCache : Singleton<EntityCache>
    {
        private Grid2D<GameEntity> m_grid;   //启用网格空间结构
        public Dictionary<EPlayerType, GameEntity> heroMap;
        private EntityCache()
        {
            var sceenRect = DirectorUtil.GetScreenRect();
            m_grid = new Grid2D<GameEntity>();
            m_grid.Init(6, 6, (int)sceenRect.width, (int)sceenRect.height);

            heroMap = new Dictionary<EPlayerType, GameEntity>();
        }

        public void SetHero(EPlayerType ePlayerType, GameEntity entity)
        {
            if (entity == null)
            {
                heroMap.Remove(ePlayerType);
            }
            else
            {
                heroMap[ePlayerType] = entity;
            }

        }

        public GameEntity GetHero(EPlayerType ePlayerType)
        {
            return heroMap[ePlayerType];
        }

        //public void UpdateGrid(GameEntity entity)
        //{
        //    if (entity.hasTransform)
        //    {
        //        var sceenPoint = DirectorUtil.WorldToScreenPoint(entity.transform.position);
        //        m_grid.Update(entity,(int) (sceenPoint.x), (int)(sceenPoint.y));
        //    }
        //}
    }
}