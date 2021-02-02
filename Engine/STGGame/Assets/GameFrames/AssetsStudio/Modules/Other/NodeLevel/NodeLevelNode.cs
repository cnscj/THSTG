using UnityEngine;

namespace ASGame
{
    public class NodeLevelNode : MonoBehaviour
    {
        [Range(1, 5)] public int level;
        public int CurLevel { get; protected set; } = -1;

        public void Start()
        {
            AdjustSelf();
            NodeLevelManager.GetInstance().AddNode(this);
        }

        public void OnDestroy()
        {
            NodeLevelManager.GetInstance().RemoveNode(this);
        }

        public void Adjust(int lv)
        {
            if (lv == CurLevel)
                return;

            gameObject.SetActive(lv >= level);

            CurLevel = lv;
        }

        private void AdjustSelf()
        {
            Adjust(NodeLevelManager.GetInstance().Level);
        }
    }

}

