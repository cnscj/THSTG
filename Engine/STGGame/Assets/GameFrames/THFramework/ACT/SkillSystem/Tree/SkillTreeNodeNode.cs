
using UnityEngine.Playables;
using XNode;

namespace THGame
{
	public class SkillTreeNodeNode : Node
	{
		public SkillBean skillBean;

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
