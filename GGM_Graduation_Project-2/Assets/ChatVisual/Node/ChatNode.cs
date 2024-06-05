using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChatVisual
{
    public class ChatNode : Node
    {
        public List<Node> childList = new List<Node>();

        public EChatState state; 
        public EChatType type;
        public string text;    
        public EFace face; 
        public List<EChatEvent> textEvent = new List<EChatEvent>();
    }
}
