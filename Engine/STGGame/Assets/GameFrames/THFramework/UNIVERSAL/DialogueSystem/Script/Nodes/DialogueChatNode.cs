using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace THGame
{
    public class DialogueChatNode : DialogueBaseNode
    {
        public DialogueCharacterInfo character;
        [TextArea] public string content;
        [Output(dynamicPortList = true)] public List<DialogueChatAnswer> answers = new List<DialogueChatAnswer>();

        public void AnswerQuestion(int index)
        {
            NodePort port = null;
            if (answers.Count == 0)
            {
                port = GetOutputPort("output");
            }
            else
            {
                if (answers.Count <= index) return;
                port = GetOutputPort("answers " + index);
            }

            if (port == null) return;
            for (int i = 0; i < port.ConnectionCount; i++)
            {
                NodePort connection = port.GetConnection(i);
                (connection.node as DialogueBaseNode).Trigger();
            }
        }
    

        public override void Trigger()
        {
            (graph as DialogueGraph).current = this;
        }
    }
}