using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System;

namespace ChatVisual
{
    public class ChatView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<ChatView, UxmlTraits> { }
        public class UmalTraits : UxmlTraits { }

        private ChatContainer chatContainer;        // ���õ� ����ִ� ��.

        public Action<NodeView> OnNodeSelected;     // ���� ���ȴٰ� �˷���.

        public ChatView()
        {
            Insert(0, new GridBackground());        // �׸��� �ֱ�

            this.AddManipulator(new ContentZoomer());       // �ܱ�� ���� �߰�
            this.AddManipulator(new ContentDragger());  // ������ �巡�� ����
            this.AddManipulator(new SelectionDragger());    // �������ذ� �����̱�
            //this.AddManipulator(new RectangleSelector());   // �׸� ������ֱ�  ���۵� �߰�
        }

/*        private void CreateNode(Type type, Vector2 position)
        {
            Node node = chatContainer.CreateNode(type);      // ��� ����
            node.position = position;
            CreateNodeView(node);       // ���̴� �� ������ �߰�����.
        }

        private void CreateNodeView(Node n)
        {
            NodeView nodeView = new NodeView(n);
            nodeView.OnNodeSelected = OnNodeSelected;
            AddElement(nodeView);
        }*/

        public void PopulateView(ChatContainer chatContainer)
        {
            this.chatContainer = chatContainer;

            // �� �����̳� ������.
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)       // ��Ŭ�� ���� �� ���� �޴���
        {
            if (chatContainer == null)      // ���� ������. Ʈ���� ������
            {
                Debug.Log("�����̳� �����!");
                evt.StopPropagation();      // �̺�Ʈ ���� ����
                return;
            }

            Vector2 nodePosition = this.ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);      // ��Ŭ���� ��ġ ��������, �� ��ġ�� ��� ���� ����

            var types = TypeCache.GetTypesDerivedFrom<Node>();      // ��ӹ��� �ֵ� ��� ������ ����
            foreach (var type in types)
            {
                //evt.menu.AppendAction("ChatNode", (a) => { CreateNode(type, nodePosition); });
            }
        }

    }
}