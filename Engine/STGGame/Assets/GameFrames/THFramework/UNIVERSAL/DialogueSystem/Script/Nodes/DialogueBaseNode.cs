using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace THGame
{
    public abstract class DialogueBaseNode : Node
    {
		[Input(backingValue = ShowBackingValue.Never)] public DialogueBaseNode input;
		[Output(backingValue = ShowBackingValue.Never)] public DialogueBaseNode output;

		abstract public void Trigger();

		public override object GetValue(NodePort port)
		{
			return null;
		}
	}
}
