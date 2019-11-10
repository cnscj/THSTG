using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    public int maxHealth = 100; //最大生命值
    public int maxArmor = 100;  //最大护甲值

    public int health = 100;    //当前生命值
    public int armor = 100;     //当前护甲值
}
