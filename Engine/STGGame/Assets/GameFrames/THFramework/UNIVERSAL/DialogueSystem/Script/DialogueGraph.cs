using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XNode;

namespace THGame
{
    [CreateAssetMenu(fileName = "DialogGraph", menuName = "THGame/DialogEditor/DialogGraph")]
    public class DialogueGraph : NodeGraph
    {
        public DialogueChatNode current;

        public void Restart()
        {
            //Find the first DialogueNode without any inputs. This is the starting node.
            current = nodes.Find(x => x is DialogueChatNode && x.Inputs.All(y => !y.IsConnected)) as DialogueChatNode;
        }

        public DialogueChatNode AnswerQuestion(int i)
        {
            current.AnswerQuestion(i);
            return current;
        }
    }
}
