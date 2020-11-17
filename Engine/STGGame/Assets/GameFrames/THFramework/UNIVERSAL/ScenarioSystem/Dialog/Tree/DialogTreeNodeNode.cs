using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace THGame
{
	public class DialogTreeNodeNode : Node
	{
		public int dialogId;
		public int language;


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
