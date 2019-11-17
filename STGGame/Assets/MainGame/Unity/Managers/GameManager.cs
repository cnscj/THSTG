
using UnityEngine;
using Unity.Entities;
using XLibrary.Package;

namespace STGU3D
{
    public class GameManager : MonoSingleton<GameManager>
    {
        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            STGService.Main.InitAwake();
        }
        private void Start()
        {
            STGService.Main.InitStart();
        }

        private void Update()
        {
            STGService.Main.Update();
        }
        private GameManager()
        {

        }
    }
}