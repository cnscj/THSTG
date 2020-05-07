
using XLibrary.Package;

namespace STGU3D
{
    public class GameManager : MonoSingleton<GameManager>
    {
        private void Awake()
        {
            STGRuntime.Main.InitAwake();
        }
        private void Start()
        {
            STGRuntime.Main.InitStart();

        }

        private void Update()
        {
            STGRuntime.Main.Update();
        }
        private GameManager()
        {

        }
    }
}