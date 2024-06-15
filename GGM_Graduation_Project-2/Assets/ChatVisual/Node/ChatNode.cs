using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChatVisual
{
    [Serializable]
    public class ChatNode : Node
    {
        public Node parent;
        public List<Node> childList = new List<Node>();

        public EChatState state; 
        public EChatType type;
        public string chatText;    
        public EFace face; 

        public List<EChatEvent> textEvent = new List<EChatEvent>();
        public string LoadNextDialog;
    }
}
