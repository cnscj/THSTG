
using UnityEngine;
using Unity.Entities;

[RequireComponent(typeof(GameObjectEntity))]
public class RotateComponent : MonoBehaviour
{
    //旋转速度
    public int speed;
}
