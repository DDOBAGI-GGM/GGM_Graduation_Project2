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

        private int firstChatEndIndex = 0;      // ó������ �����ϴ� ������ ������. ������ ���۵Ǵ� ���� ��彺���� ã�� �� �ִ�  �ε���
        private int askAndReplysCount = 0;        // ������ ���� ����� ����
        private int lockAskAndReplysCount = 0;

        public ChatView()
        {
            firstChatEndIndex = 0;
            askAndReplysCount = 0;
            lockAskAndReplysCount = 0;

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
            if (this.chatContainer.nodes.Count == 0)
            {
                RootNode rootNode = chatContainer.CreateNode(typeof(RootNode)) as RootNode;
                EditorUtility.SetDirty(this.chatContainer);
                AssetDatabase.SaveAssets();
            }

            //Debug.Log($"ä�ó�� ���� : {this.chatContainer.NowChapter.chat.Count}");
            // ä�� ��� ������ֱ�
            for (int i = 0; i < this.chatContainer.NowChapter.chat.Count; ++i)
            {
                ++firstChatEndIndex;
                if (this.chatContainer.nodes.Count() - 1 > i) continue;     // ī��Ʈ�� i���� ũ�� ��尡 �ִ� ����. - 1�ϴ� ������ ��Ʈ��尡 �����ϴϱ�.

                ChatNode chatNode = chatContainer.CreateNode(typeof(ChatNode)) as ChatNode;
                chatNode.state = this.chatContainer.NowChapter.chat[i].state;
                chatNode.text = this.chatContainer.NowChapter.chat[i].text;
                chatNode.face = this.chatContainer.NowChapter.chat[i].face;
                chatNode.textEvent = new List<EChatEvent>(this.chatContainer.NowChapter.chat[i].textEvent);

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
            {
                ChatNode firstChatEndNode = this.chatContainer.nodes[firstChatEndIndex] as ChatNode;
                if (firstChatEndNode != null)
                {
                    //Debug.Log($"������ ���� : {this.chatContainer.NowChapter.askAndReply.Count}, ê���� �ε��� {firstChatEndIndex}");
                    // �׳� ���� ������ֱ�
                    for (int i = 0; i < this.chatContainer.NowChapter.askAndReply.Count; ++i)
                    {
                        if (this.chatContainer.nodes.Count() - 1 > i + firstChatEndIndex + askAndReplysCount) continue;
                        ++askAndReplysCount;

                        // ���� ��� �߰����ֱ�
                        AskNode askNode = chatContainer.CreateNode(typeof(AskNode)) as AskNode;
                        askNode.ask = this.chatContainer.NowChapter.askAndReply[i].ask;
                        askNode.reply = new List<Chat>(this.chatContainer.NowChapter.askAndReply[i].reply);        // �� ����, ���� ����
                        askNode.is_UseThis = this.chatContainer.NowChapter.askAndReply[i].is_UseThis;

                        // ���� ���� �������ֱ�
                        firstChatEndNode.child.Add(askNode);

                        // ���(����)��� �߰����ֱ�
                        for (int j = 0; j < this.chatContainer.NowChapter.askAndReply[i].reply.Count; ++j)
                        {
                            ChatNode replyNode = chatContainer.CreateNode(typeof(ChatNode)) as ChatNode;
                            replyNode.state = this.chatContainer.NowChapter.askAndReply[i].reply[j].state;
                            replyNode.text = this.chatContainer.NowChapter.askAndReply[i].reply[j].text;
                            replyNode.face = this.chatContainer.NowChapter.askAndReply[i].reply[j].face;
                            replyNode.textEvent = new List<EChatEvent>(this.chatContainer.NowChapter.askAndReply[i].reply[j].textEvent);

                            // ���� ���ֱ�
                            if (j == 0)     // �������� �����ؾ� �ϴ� ���
                            {
                                askNode.child = replyNode;
                            }
                            else
                            {
                                ChatNode chatParentNode = this.chatContainer.nodes[(j + firstChatEndIndex + askAndReplysCount) - 1] as ChatNode;        // ��Ʈ��� ���� �־��ֱ�
                                if (chatParentNode != null)
                                {
                                    chatParentNode.child.Add(replyNode);
                                }
                            }
                            ++askAndReplysCount;
                        }
                    }

                    // ��� ���� ������ֱ�
                    for (int i = 0; i < this.chatContainer.NowChapter.lockAskAndReply.Count; i++)
                    {
                        if (this.chatContainer.nodes.Count() - 1 > i + firstChatEndIndex + askAndReplysCount + askAndReplysCount) continue;
                        ++lockAskAndReplysCount;

                        // ������ ��� �߰����ֱ�
                        LockAskNode lockAskNode = chatContainer.CreateNode(typeof(LockAskNode)) as LockAskNode;
                        lockAskNode.evidence = new List<string>(this.chatContainer.NowChapter.lockAskAndReply[i].evidence);
                        lockAskNode.ask = this.chatContainer.NowChapter.lockAskAndReply[i].ask;
                        lockAskNode.reply = new List<Chat>(this.chatContainer.NowChapter.lockAskAndReply[i].reply);        // �� ����, ���� ����
                        lockAskNode.is_UseThis = this.chatContainer.NowChapter.lockAskAndReply[i].is_UseThis;

                        // ���� ���� �������ֱ�
                        firstChatEndNode.child.Add(lockAskNode);

                        // ���(����)��� �߰����ֱ�
                        for (int j = 0; j < this.chatContainer.NowChapter.lockAskAndReply[i].reply.Count; ++j)
                        {
                            ChatNode replyNode = chatContainer.CreateNode(typeof(ChatNode)) as ChatNode;
                            replyNode.state = this.chatContainer.NowChapter.lockAskAndReply[i].reply[j].state;
                            replyNode.text = this.chatContainer.NowChapter.lockAskAndReply[i].reply[j].text;
                            replyNode.face = this.chatContainer.NowChapter.lockAskAndReply[i].reply[j].face;
                            replyNode.textEvent = new List<EChatEvent>(this.chatContainer.NowChapter.lockAskAndReply[i].reply[j].textEvent);

                            // ���� ���ֱ�
                            if (j == 0)     // �������� �����ؾ� �ϴ� ���
                            {
                                lockAskNode.child = replyNode;
                            }
                            else
                            {
                                ChatNode chatParentNode = this.chatContainer.nodes[(j + firstChatEndIndex + askAndReplysCount + lockAskAndReplysCount) - 1] as ChatNode;        // ��Ʈ��� ���� �־��ֱ�
                                if (chatParentNode != null)
                                {
                                    chatParentNode.child.Add(replyNode);
                                }
                            }
                            ++lockAskAndReplysCount;
                        }
                    }
                }
            }
        }

        public void SaveChatSystem()
        {
            chatContainer.NowChapter.chat.Clear();
            chatContainer.NowChapter.askAndReply.Clear();
            chatContainer.NowChapter.lockAskAndReply.Clear();

            int nowChatIndex = 0;       // ó������ �����ϰ�
            int askIndex = 0;
            int lockAskIndex = 0;

            this.chatContainer.nodes.ForEach(n =>
            {
                var children = this.chatContainer.GetChildren(n);       // �ڽĵ� ��������
                children.ForEach(c =>
                {
                    ChatNode chatNode = c as ChatNode;      // ���� ����̸�
                    if (chatNode != null && firstChatEndIndex > nowChatIndex)
                    {
                        // ���ο� Ŭ���� �������.
                        Chat chapter = new Chat();
                        chatContainer.NowChapter.chat.Add(chapter);
                        chatContainer.NowChapter.chat[nowChatIndex].state = chatNode.state;
                        chatContainer.NowChapter.chat[nowChatIndex].text = chatNode.text;
                        chatContainer.NowChapter.chat[nowChatIndex].face = chatNode.face;
                        chatContainer.NowChapter.chat[nowChatIndex].textEvent = chatNode.textEvent;
                        ++nowChatIndex;
                    }

/*                    AskNode askNode = c as AskNode;     // ���� ����̸�
                    if (askNode != null)
                    {
                        AskAndReply askAndReply = new AskAndReply();
                        chatContainer.NowChapter.askAndReply.Add(askAndReply);
                        chatContainer.NowChapter.askAndReply[askIndex].ask = askNode.ask;
                        chatContainer.NowChapter.askAndReply[askIndex].reply = askNode.reply;
                        chatContainer.NowChapter.askAndReply[askIndex].is_UseThis = askNode.is_UseThis;
                        ++askIndex;
                    }

                    LockAskNode lockAskNode = c as LockAskNode;*/


                });
            });
            chatContainer.ChangeNewChpater();
        }

        public void PopulateView()
        {
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