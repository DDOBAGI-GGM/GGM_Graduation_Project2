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

        private Node askParentNode;

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
            SortChildAndIndex();
        }
#endif

        public void AddChild(Node parent, Node child)
        {
            Debug.Log($"�� ����, parent : {parent}, child : {child}");
            var rootNode = parent as RootNode;      //�θ� ��Ʈ�̸�
            if (rootNode != null)
            {
                rootNode.child = child;
                SortChildAndIndex();
                return;
            }

            var chatNode = parent as ChatNode;
            if (chatNode != null)
            {
                chatNode.child.Add(child);
                SortChildAndIndex();
                return;
            }

            var askNode = parent as AskNode;
            if (askNode != null)
            {
                askNode.child = child;
                SortChildAndIndex();
                return;
            }

            var lockAskNode = parent as LockAskNode;
            if (lockAskNode != null)
            {
                lockAskNode.child = child;
                SortChildAndIndex();
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

        public void SortChildAndIndex()        // ��带 �����ϰ� ���� ��� �Ʒ� ê���̶�� ���� ����� �ڽ����� �־���. 
        {
            nowChatIndex = 1;

            Queue<Node> askQueue = new Queue<Node>();    // ����, ������� ���� ��
            Node nowNode = nodes[0];

            // DFS �� �� ���ٰ� BFS �� ������ ��� ���� ������ ������ ���鿡 ���ؼ� DFS �� �� �ؼ� �ε��� ������ֱ�
            while (nowNode != null)
            {
                var children = GetChildren(nowNode);
                if (children.Count == 1)        // �׳� ���ó���̸�
                {
                    children[0].index = nowChatIndex;
                    children[0].indexLabel.text = nowChatIndex.ToString();
                    nowChatIndex++;
                    nowNode = children[0];
                }
                else
                {
                    for (int i = 0; i < children.Count; i++)
                    {
                        askQueue.Enqueue(children[i]);

                        if (children[i] is AskNode askNode)
                        {
                            askNode.reply.Clear();
                        }

                        if (children[i] is LockAskNode lockAskNode)
                        {
                            lockAskNode.reply.Clear();
                        }

                        children[i].index = nowChatIndex;
                        children[i].indexLabel.text = nowChatIndex.ToString();
                        nowChatIndex++;
                    }
                    break;
                }
            }

            while (askQueue.Count > 0)
            {
                Debug.Log(askQueue.Peek());
                askParentNode = askQueue.Peek();
                var children = GetChildren(askQueue.Peek());
                if (children.Count > 0) AskChatSort(children[0] as ChatNode);
                askQueue.Dequeue();
            }
        }

        private void AskChatSort(ChatNode chatNode)      // ������ �ְ� �� �� �ش� ������ �ڽĵ��� �����ϱ� �����ϸ鼭 �θ� �͵� �־������.
        {
            if (chatNode == null) return;

            if (askParentNode is  AskNode askNode)
            {
                Chat chat = new Chat();
                chat.text = chatNode.text;
                chat.state = chatNode.state;
                chat.face = chatNode.face;
                chat.textEvent = chatNode.textEvent;
                askNode.reply.Add(chat);
            }

            if (askParentNode is LockAskNode lockAskNode)
            {
                Chat chat = new Chat();
                chat.text = chatNode.text;
                chat.state = chatNode.state;
                chat.face = chatNode.face;
                chat.textEvent = chatNode.textEvent;
                lockAskNode.reply.Add(chat);
            }

            chatNode.index = nowChatIndex;
            chatNode.indexLabel.text = nowChatIndex.ToString();
            nowChatIndex++;

            Debug.Log(chatNode.child.Count);
            var next = GetChildren(chatNode);
            if (next.Count > 0) AskChatSort(next[0] as ChatNode);
        }
    }
}
