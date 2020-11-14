
using UnityEngine.Playables;
using XNode;

namespace THGame
{
	public class SkillTreeNodeNode : Node
	{
		public int skillName;
        public SkillTreePrecondt[] precondts;
		public PlayableAsset skillBehaviour;

		protected override void Init()
		{
			base.Init();
		}

		
		public override object GetValue(NodePort port)
		{
			return null;
		}
	}
}
