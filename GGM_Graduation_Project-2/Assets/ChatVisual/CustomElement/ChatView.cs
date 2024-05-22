using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System;
using System.Linq;
using Codice.Client.Common;

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

        public void LoadChatSystem(ChatContainer _chatContainer)     // �ε����ֱ�
        {
            chatContainer = _chatContainer;

            chatContainer.nodes.Clear();

            int firstChatEndIndex = 0;      // ó������ �����ϴ� ������ ������. ������ ���۵Ǵ� ���� ��彺���� ã�� �� �ִ�  �ε���
            int askAndReplysCount = 0;        // ������ ���� ����� ����          // �̰� 2���� �ǰ� �־���. �ٲ��ֱ�
            int lockAskAndReplysCount = 0;

            // ��Ʈ ������ �ٽ� �׷��ֱ�
            {
                RootNode rootNode = chatContainer.CreateNode(typeof(RootNode)) as RootNode;
                rootNode.showName = chatContainer.NowChapter.showName;
                rootNode.saveLocation = chatContainer.NowChapter.saveLocation;
                rootNode.round = new List<string>(chatContainer.NowChapter.round);
                EditorUtility.SetDirty(chatContainer);
                AssetDatabase.SaveAssets();
            }

            // ä�� ��� ������ֱ�
            for (int i = 0; i < chatContainer.NowChapter.chat.Count; ++i)
            {
                ++firstChatEndIndex;

                // ������ �������ֱ�
                ChatNode chatNode = chatContainer.CreateNode(typeof(ChatNode)) as ChatNode;
                chatNode.state = chatContainer.NowChapter.chat[i].state;
                chatNode.text = chatContainer.NowChapter.chat[i].text;
                chatNode.face = chatContainer.NowChapter.chat[i].face;
                chatNode.textEvent = new List<EChatEvent>(chatContainer.NowChapter.chat[i].textEvent);

                // ��ġ ����ֱ�
                if (chatNode.state == EChatState.Other)
                {
                    chatNode.position = new Vector2(-125, 150 * (i + 1));
                }
                else
                {
                    chatNode.position = new Vector2(125, 150 * (i + 1));
                }

                if (i == 0)     // ��Ʈ���� �������ֱ�
                {
                    RootNode rootNode = chatContainer.nodes[0] as RootNode;
                    rootNode.child = chatNode;
                }
                else        // ������ �������ֱ�
                {
                    ChatNode chatParentNode = chatContainer.nodes[i] as ChatNode;
                    chatParentNode.child.Add(chatNode);
                }
            }

            // ����, ��� ���� ��� ������ֱ�
            {
                ChatNode firstChatEndNode = chatContainer.nodes[firstChatEndIndex] as ChatNode;
                if (firstChatEndNode != null)
                {
                    Vector2 endChatNodePosition = firstChatEndNode.position;
                    //Debug.Log($"������ ���� : {this.chatContainer.NowChapter.askAndReply.Count}, ê���� �ε��� {firstChatEndIndex}");

                    // �׳� ���� ������ֱ�
                    for (int i = 0; i < chatContainer.NowChapter.askAndReply.Count; ++i)
                    {
                        ++askAndReplysCount;

                        // ���� ��� �߰��ؼ� ������ �߰�
                        AskNode askNode = chatContainer.CreateNode(typeof(AskNode)) as AskNode;
                        askNode.ask = this.chatContainer.NowChapter.askAndReply[i].ask;
                        askNode.reply = new List<Chat>(this.chatContainer.NowChapter.askAndReply[i].reply);        // �� ����, ���� ����
                        askNode.is_UseThis = this.chatContainer.NowChapter.askAndReply[i].is_UseThis;

                        // ���� ���� �������ֱ�
                        firstChatEndNode.child.Add(askNode);

                        // ��ġ �������ֱ�
                        askNode.position = new Vector2(endChatNodePosition.x + i * 400, endChatNodePosition.y + 200);

                        // ���(����)��� �߰����ֱ�
                        for (int j = 0; j < chatContainer.NowChapter.askAndReply[i].reply.Count; ++j)
                        {
                            // ���� ��� �߰��ؼ� ������ �߰�
                            ChatNode replyNode = chatContainer.CreateNode(typeof(ChatNode)) as ChatNode;
                            replyNode.state = this.chatContainer.NowChapter.askAndReply[i].reply[j].state;
                            replyNode.text = this.chatContainer.NowChapter.askAndReply[i].reply[j].text;
                            replyNode.face = this.chatContainer.NowChapter.askAndReply[i].reply[j].face;
                            replyNode.textEvent = new List<EChatEvent>(this.chatContainer.NowChapter.askAndReply[i].reply[j].textEvent);

                            // ��ġ ����ֱ�
                            if (replyNode.state == EChatState.Other)
                            {
                                replyNode.position = new Vector2(askNode.position.x - 100, askNode.position.y + 140 * (j + 1));
                            }
                            else
                            {
                                replyNode.position = new Vector2(askNode.position.x + 100, askNode.position.y + 140 * (j + 1));
                            }

                            // ���� ���ֱ�
                            if (j == 0)     // �������� �����ؾ� �ϴ� ���
                            {
                                askNode.child = replyNode;
                            }
                            else
                            {
                                ChatNode chatParentNode = this.chatContainer.nodes[j + firstChatEndIndex + askAndReplysCount] as ChatNode;        // ��Ʈ��� ���� �־��ֱ�
                                if (chatParentNode != null)
                                {
                                    chatParentNode.child.Add(replyNode);
                                }
                                if (j == chatContainer.NowChapter.askAndReply[i].reply.Count - 1)      // �̰� ������ �ݺ��̸�
                                {
                                    askAndReplysCount += j + 1;
                                }
                            }
                        }
                    }

                    // ��� ���� ������ֱ�
                    for (int i = 0; i < chatContainer.NowChapter.lockAskAndReply.Count; i++)
                    {
                        ++lockAskAndReplysCount;

                        // ������ ��� �߰����ֱ�
                        LockAskNode lockAskNode = chatContainer.CreateNode(typeof(LockAskNode)) as LockAskNode;
                        lockAskNode.evidence = new List<string>(this.chatContainer.NowChapter.lockAskAndReply[i].evidence);
                        lockAskNode.ask = this.chatContainer.NowChapter.lockAskAndReply[i].ask;
                        lockAskNode.reply = new List<Chat>(this.chatContainer.NowChapter.lockAskAndReply[i].reply);        // �� ����, ���� ����
                        lockAskNode.is_UseThis = this.chatContainer.NowChapter.lockAskAndReply[i].is_UseThis;

                        // ���� ���� �������ֱ�
                        firstChatEndNode.child.Add(lockAskNode);

                        // ��ġ �������ֱ�
                        lockAskNode.position = new Vector2(endChatNodePosition.x + (chatContainer.NowChapter.askAndReply.Count * 400) + (i * 400), endChatNodePosition.y + 200);

                        // ���(����)��� �߰����ֱ�
                        for (int j = 0; j < this.chatContainer.NowChapter.lockAskAndReply[i].reply.Count; ++j)
                        {
                            ChatNode replyNode = chatContainer.CreateNode(typeof(ChatNode)) as ChatNode;
                            replyNode.state = this.chatContainer.NowChapter.lockAskAndReply[i].reply[j].state;
                            replyNode.text = this.chatContainer.NowChapter.lockAskAndReply[i].reply[j].text;
                            replyNode.face = this.chatContainer.NowChapter.lockAskAndReply[i].reply[j].face;
                            replyNode.textEvent = new List<EChatEvent>(this.chatContainer.NowChapter.lockAskAndReply[i].reply[j].textEvent);

                            // ��ġ ����ֱ�
                            if (replyNode.state == EChatState.Other)
                            {
                                replyNode.position = new Vector2(lockAskNode.position.x - 100, lockAskNode.position.y + 140 * (j + 1));
                            }
                            else
                            {
                                replyNode.position = new Vector2(lockAskNode.position.x + 100, lockAskNode.position.y + 140 * (j + 1));
                            }

                            // ���� ���ֱ�
                            if (j == 0)     // �������� �����ؾ� �ϴ� ���
                            {
                                lockAskNode.child = replyNode;
                            }
                            else
                            {
                                ChatNode chatParentNode = this.chatContainer.nodes[j + firstChatEndIndex + askAndReplysCount + lockAskAndReplysCount] as ChatNode;        // ��Ʈ��� ���� �־��ֱ�
                                if (chatParentNode != null)
                                {
                                    chatParentNode.child.Add(replyNode);
                                }
                                if (j == chatContainer.NowChapter.lockAskAndReply[i].reply.Count - 1)      // �̰� ������ �ݺ��̸�
                                {
                                    lockAskAndReplysCount += j + 1;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void SaveChatSystem()        // ������ ��������.
        {
            chatContainer.NowChapter.chat.Clear();
            chatContainer.NowChapter.askAndReply.Clear();
            chatContainer.NowChapter.lockAskAndReply.Clear();

            // nodes ����Ʈ���� ���� �ε����� ǥ������ ��.
            int chatIndex = 0;       // ����ó���� �ִ� ���õ��� �ε���
            int askIndex = 0;       // ���� ���� �������� �ε���
            int lockAskIndex = 0;       // ���� ������ ���� ��� ������
            bool firstChatEnd = false;

            int nowAskIndex = 0;        // ���� ������ �ε���
            int nowReplyIndex = 0;      // ���� ������ �� ��� �ε���
            int nowReplysCountIndex = 0;      // ���� ������ ��� ���� �ε���, �Ʒ� ����Ʈ�� �ε��� ��
            bool lockAskStart = false;      // ��� ������ ���۵Ǿ��ٸ�

            // ������ ��� ������ �������ִ� ����Ʈ
            List<int> replysCount = new List<int>();            // -1�� 

            if (chatContainer.nodes[0] != null)     // ��Ʈ��尡 �ִٸ�
            {
                RootNode rootNode = chatContainer.nodes[0] as RootNode;
                if (rootNode != null)
                {
                    chatContainer.NowChapter.showName = rootNode.showName;
                    chatContainer.NowChapter.saveLocation = rootNode.saveLocation;
                    chatContainer.NowChapter.round = new List<string>(rootNode.round);
                }
            }

            chatContainer.nodes.ForEach(n =>
            {
                var children = chatContainer.GetChildren(n);       // �ڽĵ� ��������
                children.ForEach(c =>
                {
                    ChatNode chatNode = c as ChatNode;      // ���� ����̸�
                    if (chatNode != null)
                    {
                        if (firstChatEnd == false)
                        {
                            // ���ο� Ŭ���� �������.
                            Debug.Log("�׳�����");
                            Chat chat = new Chat();
                            chatContainer.NowChapter.chat.Add(chat);
                            chatContainer.NowChapter.chat[chatIndex].state = chatNode.state;
                            chatContainer.NowChapter.chat[chatIndex].text = chatNode.text;
                            chatContainer.NowChapter.chat[chatIndex].face = chatNode.face;
                            chatContainer.NowChapter.chat[chatIndex].textEvent = chatNode.textEvent;
                            ++chatIndex;
                        }
                        else
                        {
                            // ���� �������ֱ�
                            if (lockAskStart == false)
                            {
                                if (replysCount[nowReplysCountIndex] == -1)
                                {
                                    Debug.Log("��� ���� ����");
                                    nowReplysCountIndex++;

                                    nowAskIndex = 0;
                                    nowReplyIndex = 0;

                                    Chat lockReply = new Chat();
                                    chatContainer.NowChapter.lockAskAndReply[nowAskIndex].reply.Add(lockReply);
                                    chatContainer.NowChapter.lockAskAndReply[nowAskIndex].reply[nowReplyIndex].state = chatNode.state;
                                    chatContainer.NowChapter.lockAskAndReply[nowAskIndex].reply[nowReplyIndex].text = chatNode.text;
                                    chatContainer.NowChapter.lockAskAndReply[nowAskIndex].reply[nowReplyIndex].face = chatNode.face;
                                    chatContainer.NowChapter.lockAskAndReply[nowAskIndex].reply[nowReplyIndex].textEvent = chatNode.textEvent;
                                    
                                    lockAskStart = true;
                                }

                                if (lockAskStart == false)
                                {
                                    Debug.Log("�׳����������������");
                                    Chat reply = new Chat();
                                    chatContainer.NowChapter.askAndReply[nowAskIndex].reply.Add(reply);
                                    chatContainer.NowChapter.askAndReply[nowAskIndex].reply[nowReplyIndex].state = chatNode.state;
                                    chatContainer.NowChapter.askAndReply[nowAskIndex].reply[nowReplyIndex].text = chatNode.text;
                                    chatContainer.NowChapter.askAndReply[nowAskIndex].reply[nowReplyIndex].face = chatNode.face;
                                    chatContainer.NowChapter.askAndReply[nowAskIndex].reply[nowReplyIndex].textEvent = chatNode.textEvent;
                                    nowReplyIndex++;

                                    if (nowReplyIndex + 1 > replysCount[nowReplysCountIndex])      // ��� ������ �Ѿ��ٸ�
                                    {
                                        nowAskIndex++;
                                        nowReplysCountIndex++;
                                        nowReplyIndex = 0;
                                    }
                                }
                            }
                            else
                            {
                                Debug.Log("��� ���� 2��° ���");

                                nowReplyIndex++;
                                Chat chat = new Chat();
                                chatContainer.NowChapter.lockAskAndReply[nowAskIndex].reply.Add(chat);
                                chatContainer.NowChapter.lockAskAndReply[nowAskIndex].reply[nowReplyIndex].state = chatNode.state;
                                chatContainer.NowChapter.lockAskAndReply[nowAskIndex].reply[nowReplyIndex].text = chatNode.text;
                                chatContainer.NowChapter.lockAskAndReply[nowAskIndex].reply[nowReplyIndex].face = chatNode.face;
                                chatContainer.NowChapter.lockAskAndReply[nowAskIndex].reply[nowReplyIndex].textEvent = chatNode.textEvent;

                                if (nowReplyIndex + 1 > replysCount[nowReplysCountIndex])      // ��� ������ �Ѿ��ٸ�
                                {
                                    nowAskIndex++;
                                    nowReplysCountIndex++;
                                    nowReplyIndex = 0;
                                }
                            }
                        }
                    }

                    AskNode askNode = c as AskNode;     // ���� ����̸�
                    if (askNode != null)
                    {
                        AskAndReply askAndReply = new AskAndReply();
                        chatContainer.NowChapter.askAndReply.Add(askAndReply);
                        chatContainer.NowChapter.askAndReply[askIndex].ask = askNode.ask;
                        replysCount.Add(askNode.reply.Count);
                        Debug.Log($"�� �ڽ� ���� : {askNode.reply.Count}");
                        chatContainer.NowChapter.askAndReply[askIndex].is_UseThis = askNode.is_UseThis;
                        ++askIndex;
                        firstChatEnd = true;
                    }

                    LockAskNode lockAskNode = c as LockAskNode;
                    if (lockAskNode != null)
                    {
                        if (replysCount.Find(n => n == -1) == 0)
                        {
                            replysCount.Add(-1);        // ���ɸ� ������ �����ߴٰ�
                        }

                        LockAskAndReply lockAskAndReply = new LockAskAndReply();
                        chatContainer.NowChapter.lockAskAndReply.Add(lockAskAndReply);
                        chatContainer.NowChapter.lockAskAndReply[lockAskIndex].evidence = lockAskNode.evidence;
                        chatContainer.NowChapter.lockAskAndReply[lockAskIndex].ask = lockAskNode.ask;
                        replysCount.Add(lockAskNode.reply.Count);
                        chatContainer.NowChapter.lockAskAndReply[lockAskIndex].is_UseThis = lockAskNode.is_UseThis;
                        ++lockAskIndex;
                        firstChatEnd = true;
                    }
                });
            });
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