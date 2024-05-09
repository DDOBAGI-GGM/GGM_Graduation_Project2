using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChatVisual
{
    public class AskNode : Node
    {
        public Node child;

        public string ask;        // ���� �� �ִ� ������
        public List<Chat> reply = new List<Chat>();     // �׿� ���� ����
        public bool is_UseThis;     // ����ߴ���

        protected override void OnStart()
        {
        }
    }
}
