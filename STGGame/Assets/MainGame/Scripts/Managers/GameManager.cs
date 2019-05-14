
using UnityEngine;
namespace THGame
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance { get; private set; } = null;
        void Awake()
        {
            instance = this;
        }
    }
}