using Entitas;
namespace STGU3D
{
    [Game]
    public class HealthComponent : IComponent
    {
        public int maxHealth = 100; //最大生命值
        public int maxArmor = 100;  //最大护甲值

        public int health = 100;    //当前生命值
        public int armor = 100;     //当前护甲值
    }

}
