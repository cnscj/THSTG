
using UnityEngine.Playables;
using XNode;

namespace THGame
{
	public class SkillTreeNodeNode : Node
	{
		public string skillName;
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
