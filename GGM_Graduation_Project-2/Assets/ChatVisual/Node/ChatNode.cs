using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChatVisual
{
    public class ChatNode : Node
    {
        public List<Node> child = new List<Node>();

        public EChatState state;     // ���ϴ� ���� Ÿ��
        public string text;        // �� �ϴ� ��.
        public EFace face;       // �� �� ���� ǥ��
        public List<EChatEvent> textEvent = new List<EChatEvent>();

        protected override void OnStart()
        {
        }
    }
}
