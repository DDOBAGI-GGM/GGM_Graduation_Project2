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
            this.AddManipulator(new RectangleSelector());   // �׸� ������ֱ�  ���۵� �߰�
        }

        public void LoadChatSystem(ChatContainer chatContainer)     // �ε����ֱ�
        {
            this.chatContainer = chatContainer;

            // ��Ʈ ��尡 ���ٸ� �����Ϳ��� �ٽ� �׸���
            if (chatContainer.rootNode == null)     
            {
                this.chatContainer.rootNode = chatContainer.CreateNode(typeof(RootNode)) as RootNode;
                Debug.Log(this.chatContainer.rootNode);
                EditorUtility.SetDirty(this.chatContainer);    
                AssetDatabase.SaveAssets();
            }

            // ä�� ��� ������ֱ�
            for (int i = 0; i < this.chatContainer.NowChapter.chat.Count; i++)
            {
                if (this.chatContainer.nodes.Count() - 1 > i) continue;     // ī��Ʈ�� i���� ũ�� ��尡 �ִ� ����. - 1�ϴ� ������ ��Ʈ��尡 �����ϴϱ�.

                ChatNode chatNode = chatContainer.CreateNode(typeof(ChatNode)) as ChatNode;
                chatNode.state = this.chatContainer.NowChapter.chat[i].state;
                chatNode.text = this.chatContainer.NowChapter.chat[i].text;
                chatNode.face = this.chatContainer.NowChapter.chat[i].face;
                chatNode.textEvent = this.chatContainer.NowChapter.chat[i].textEvent;

                if (i == 0)     // ��Ʈ���� �������ֱ�
                {
                    RootNode rootNode = this.chatContainer.nodes[0] as RootNode;
                    rootNode.child = chatNode;
                }
                else        // ������ �������ֱ�
                {
                    ChatNode chatParentNode = this.chatContainer.nodes[i] as ChatNode;
                    chatParentNode.child.Add(chatNode);
                }
            }

            // ���� ��� ������ֱ�


        }

        public void SaveChatSystem()
        {
            Debug.Log("����");

            this.chatContainer.nodes.ForEach(n =>
            {
                var children = this.chatContainer.GetChildren(n);
                NodeView parent = FindNodeView(n);
                children.ForEach(c =>
                {

                });
            });

        }

        public void PopulateView()
        {
            Debug.Log("���ΰ�ħ");

            graphViewChanged -= OnGraphViewChanged;

            DeleteElements(graphElements);       // ������ �׷����� �ֵ� ��� ����

            graphViewChanged += OnGraphViewChanged;

            // ���� �׷��ֱ�
            this.chatContainer.nodes.ForEach(n => CreateNodeView(n));

            // ���� �׷��ֱ�
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

            // ���� ������ֱ�
            this.chatContainer.SortIndex();
        }

        private NodeView FindNodeView(Node node)
        {
            return GetNodeByGuid(node.guid) as NodeView;
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.elementsToRemove != null)       // ������ �ֵ��� ������
            {
                graphViewChange.elementsToRemove.ForEach(elem =>
                {
                    var nodeView = elem as NodeView;
                    if (nodeView != null)
                    {
                        chatContainer.DeleteNode(nodeView.node);        // �����ֱ�
                    }
                    
                    var edge = elem as Edge;        // ���ἱ
                    if (edge != null)
                    {
                        NodeView parent = edge.output.node as NodeView;
                        NodeView child = edge.input.node as NodeView;

                        chatContainer.RemoveChild(parent.node, child.node);
                    }
                });
            }    

            if (graphViewChange.edgesToCreate != null)      // �� �������ֱ�
            {
                graphViewChange.edgesToCreate.ForEach(edge =>
                {
                    NodeView parent = edge.output.node as NodeView;
                    NodeView child = edge.input.node as NodeView;

                    chatContainer.AddChild(parent.node, child.node);
                });
            }

            if (graphViewChange.movedElements != null)      // ������ �ֵ��� ������ �������ֱ�
            {
                nodes.ForEach(n =>
                {
                    var nodeView = n as NodeView;
                    nodeView?.SortChildren();
                });
            }

            return graphViewChange;
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
            return ports.ToList().Where(x => x.direction != startPort.direction).ToList();      // input output�� �ٸ��� ���� ����
        }
    }
}