using Entitas;

namespace STGU3D
{
    public class InputFeature : Feature
    {
        public InputFeature(Contexts contexts)
        {
            Add(new InputSystem(contexts));
        }
    }
}

