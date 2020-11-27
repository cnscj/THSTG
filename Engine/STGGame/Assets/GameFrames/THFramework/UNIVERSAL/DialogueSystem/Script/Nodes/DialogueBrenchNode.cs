using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace THGame
{
    
    public class DialogueBrenchNode : DialogueBaseNode
    {
        [Output] public DialogueBaseNode pass;
        [Output] public DialogueBaseNode fail;
        private bool success;

        public override void Trigger()
        {
            // Perform condition
            bool success = true;
            //TODO:

          
            //Trigger next nodes
            NodePort port;
            if (success) port = GetOutputPort("pass");
            else port = GetOutputPort("fail");
            if (port == null) return;
            for (int i = 0; i < port.ConnectionCount; i++)
            {
                NodePort connection = port.GetConnection(i);
                (connection.node as DialogueBaseNode).Trigger();
            }
        }
    }
}