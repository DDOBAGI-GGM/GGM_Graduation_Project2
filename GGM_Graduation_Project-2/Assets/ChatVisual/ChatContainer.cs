using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ChatVisual
{
    public class ChatContainer : MonoBehaviour
    {
        public List<Node> nodes = new List<Node>();         // ��� ����Ʈ

        public int nowChaptersIndex;        // é�� �ε���
        public int nowChatIndex;            // ���� �ε���

        [Space(30)]

        [SerializeField]        // �׳� ���� � é������ ������ �ִ� ��. ���� ����� �־�����.
        private Chapter nowChapter;
        public Chapter NowChapter { get { return nowChapter; } set { nowChapter = value; } }

        [SerializeField]
        private List<Chapter> mainChapter = new List<Chapter>();     // é�͵�
        public List<Chapter> MainChapter { get { return mainChapter; } set { mainChapter = value; } }

        public void ChangeNowChapter(int index)
        {
            if (mainChapter.Count <= index)
            {
                Debug.Log("���� ������ֱ�");
                mainChapter.Add(new Chapter());
            }
            nowChaptersIndex = index;
            nowChapter = mainChapter[index];        // ���� ����
        }

#if UNITY_EDITOR
        public Node CreateNode(Type type)
        {
            var node = Activator.CreateInstance(type) as Node;
            node.guid = GUID.Generate().ToString();

            nodes.Add(node);        // ����Ʈ�� �߰�

            AssetDatabase.SaveAssets();

            return node;
        }

        public void DeleteNode(Node node)
        {
            nodes.Remove(node);
            AssetDatabase.SaveAssets();
            SortIndex();
        }
#endif

        public void AddChild(Node parent, Node child)
        {
            Debug.Log($"�� ����, parent : {parent}, child : {child}");
            var rootNode = parent as RootNode;      //�θ� ��Ʈ�̸�
            if (rootNode != null)
            {
                rootNode.child = child;
                SortIndex();
                return;
            }

            var chatNode = parent as ChatNode;
            if (chatNode != null)
            {
                chatNode.child.Add(child);
                SortIndex();
                return;
            }

            var askNode = parent as AskNode;
            if (askNode != null)
            {
                askNode.child = child;
                SortIndex();
                return;
            }

            var lockAskNode = parent as LockAskNode;
            if (lockAskNode != null)
            {
                lockAskNode.child = child;
                SortIndex();
            }
        }

        public void RemoveChild(Node parent, Node child)
        {
            var rootNode = parent as RootNode;      //�θ� ��Ʈ�̸�
            if (rootNode != null)
            {
                rootNode.child = null;
                return;
            }

            var chatNode = parent as ChatNode;
            if (chatNode != null)
            {
                chatNode.child.Remove(child);
                return;
            }

            var askNode = parent as AskNode;
            if (askNode != null)
            {
                askNode.child = null;
                return;
            }

            var lockAskNode = parent as LockAskNode;
            if (lockAskNode != null)
            {
                lockAskNode.child = null;
            }
        }

        public List<Node> GetChildren(Node parent)
        {
            List<Node> children = new List<Node>();

            var rootNode = parent as RootNode;
            if (rootNode != null && rootNode.child != null)
            {
                children.Add(rootNode.child);
                return children;
            }

            var askNode = parent as AskNode;
            if (askNode != null && askNode.child != null)
            {
                children.Add(askNode.child);
                return children;
            }

            var lockAskNode = parent as LockAskNode;
            if (lockAskNode != null && lockAskNode.child != null)
            {
                children.Add(lockAskNode.child);
                return children;
            }

            var chatNode = parent as ChatNode;
            if (chatNode != null && chatNode.child.Count != 0)
            {
                children = chatNode.child;
            }

            return children;
        }

        public void SortIndex()     // �ε����� �����Ѵ�.
        {
            nowChatIndex = 1;

            bool is_firstChatEnd = false;
            List<Node> askNodes = new List<Node>();     // ���� ���� ��
            int askIndex = 0;

            nodes.ForEach(n =>
            {
                var children = GetChildren(n);
                if (children.Count == 0) askIndex++;
                children.ForEach(c =>
                {
                    if (c is ChatNode == false)     // ê�ó�尡 �ƴϸ� ù��°ê���� ����.
                    {
                        is_firstChatEnd = true;
                        askNodes.Add(c);

                        AskNode askNode = c as AskNode;
                        if (askNode != null)
                        {
                            askNode.reply.Clear();
                        }

                        LockAskNode lockAskNode = c as LockAskNode;
                        if (lockAskNode != null)
                        {
                            lockAskNode.reply.Clear();
                        }
                    }
                    else if (c is ChatNode && is_firstChatEnd)
                    {
                        // ���� ��� ���õ� ����.
                        AskNode askNode = askNodes[askIndex] as AskNode;
                        if (askNode != null)
                        {
                            ChatNode chatNode = c as ChatNode;
                            if (chatNode != null)
                            {
                                Chat chat = new Chat();
                                chat.text = chatNode.text;
                                chat.state = chatNode.state;
                                chat.face = chatNode.face;
                                chat.textEvent = chatNode.textEvent;
                                askNode.reply.Add(chat);
                            }
                        }

                        LockAskNode lockAskNode = askNodes[askIndex] as LockAskNode;
                        if (lockAskNode != null)
                        {
                            ChatNode chatNode = c as ChatNode;
                            if (chatNode != null)
                            {
                                Chat chat = new Chat();
                                chat.text = chatNode.text;
                                chat.state = chatNode.state;
                                chat.face = chatNode.face;
                                chat.textEvent = chatNode.textEvent;
                                lockAskNode.reply.Add(chat);
                            }
                        }
                    }

                    c.index = nowChatIndex;
                    c.indexLabel.text = nowChatIndex.ToString();
                    nowChatIndex++;
                });
            });
        }

        private void SortChild(Node node)
        {
            // DFS �� �� ���ٰ� BFS �� ������ ��� ���� ������ ������ ���鿡 ���ؼ� DFS �� �� �ؼ� �ε��� �о��ֱ�
        }
    }
}
