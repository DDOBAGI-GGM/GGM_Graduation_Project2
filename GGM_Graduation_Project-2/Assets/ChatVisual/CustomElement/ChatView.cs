using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System;
using System.Linq;

namespace ChatVisual
{
    public class ChatView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<ChatView, UxmlTraits> { }
        public class UmalTraits : GraphView.UxmlTraits { }

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

        public void PopulateView(ChatContainer chatContainer)
        {
            this.chatContainer = chatContainer;
            graphViewChanged -= OnGraphViewChanged;

            DeleteElements(graphElements);       // ������ �׷����� �ֵ� ��� ����

            graphViewChanged += OnGraphViewChanged;

            if (chatContainer.rootNode == null)     // ��Ʈ ��尡 ���ٸ�
            {
                this.chatContainer.rootNode = chatContainer.CreateNode(typeof(RootNode)) as RootNode;
                Debug.Log(this.chatContainer.rootNode);
                EditorUtility.SetDirty(this.chatContainer);      // �����Ϳ��� �ٽ� �׸���
                AssetDatabase.SaveAssets();
            }

            this.chatContainer.nodes.ForEach(n => CreateNodeView(n));

            this.chatContainer.nodes.ForEach(n =>
            {
                var children = this.chatContainer.GetChildren(n);
                NodeView parent = FindNodeView(n);
                children.ForEach(c =>
                {
                    NodeView child = FindNodeView(c);
                    Edge edge = parent.output.ConnectTo(child.input);
                    AddElement(edge);
                });
            });
        }

        private NodeView FindNodeView(Node node)
        {
            return GetNodeByGuid(node.guid) as NodeView;
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChanged)
        {
            if (graphViewChanged.edgesToCreate != null)
            {
                graphViewChanged.edgesToCreate.ForEach(edge =>
                {
                    NodeView parent = edge.output.node as NodeView;
                    NodeView child = edge.input.node as NodeView;

                    chatContainer.AddChild(parent.node, child.node);
                });
            }

            return graphViewChanged;
        }

        private void CreateNode(Type type, Vector2 position)
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
                if (type.Name == "RootNode") continue;
                evt.menu.AppendAction($"{type.Name}", (a) => { CreateNode(type, nodePosition); });
            }
        }

        // �巡���� ���۵� �� ���ᰡ���� ��Ʈ�� �������� �Լ�
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList().Where(x => x.direction != startPort.direction /*&& x.node != startPort.node*/).ToList();      // input output�� �ٸ��� ���� ���� (���� ���� �Ȱ����� �ƴѰ� �ּ�ó��)
        }
    }
}