using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ChatVisual
{
    public class ChatContainer : MonoBehaviour
    {
        public Hierarchy hierarchy = new Hierarchy();        // 노드가 생성되는 곳.
        public List<Node> nodes = new List<Node>();         // 노드 리스트

        public int nowChaptersIndex;        // 챕터 인덱스
        public int nowChatIndex;            // 쳇팅 인덱스

        [SerializeField]
        private Chapter nowChapter;
        public Chapter NowChapter { get { return nowChapter; } set { nowChapter = value; } }

        [SerializeField]
        private List<Chapter> mainChapters = new List<Chapter>();     // 챕터들
        public List<Chapter> MainChapters { get { return mainChapters; } set { mainChapters = value; } }

        public void ChangeNowChapter(int index)         // 깊은 복사
        {
            if (mainChapters.Count <= index)
            {
                mainChapters.Add(new Chapter());
            }
            nowChaptersIndex = index;
            nowChapter.showName = mainChapters[index].showName;
            nowChapter.saveLocation = mainChapters[index].saveLocation;
            nowChapter.chat = new List<Chat>(mainChapters[index].chat);
            nowChapter.askAndReply = new List<AskAndReply>(mainChapters[index].askAndReply);
            nowChapter.lockAskAndReply = new List<LockAskAndReply>(mainChapters[index].lockAskAndReply);
            nowChapter.round = new List<string>(mainChapters[index].round);
        }

        public void ChangeNewChpater()     // 깊은 복사
        {
            mainChapters[nowChaptersIndex].showName = nowChapter.showName;
            mainChapters[nowChaptersIndex].saveLocation = nowChapter.saveLocation;
            mainChapters[nowChaptersIndex].chat = new List<Chat>(nowChapter.chat);
            mainChapters[nowChaptersIndex].askAndReply = new List<AskAndReply>(nowChapter.askAndReply);
            mainChapters[nowChaptersIndex].lockAskAndReply = new List<LockAskAndReply>(nowChapter.lockAskAndReply);
            mainChapters[nowChaptersIndex].round = new List<string>(nowChapter.round);
        }

#if UNITY_EDITOR
        public Node CreateNode(Type type)
        {
            var node = Activator.CreateInstance(type) as Node;
            node.guid = GUID.Generate().ToString();

            nodes.Add(node);        // 리스트에 추가

            AssetDatabase.SaveAssets();

            return node;
        }

        public void DeleteNode(Node node)
        {
            nodes.Remove(node);
            AssetDatabase.SaveAssets();
        }
#endif

        public void AddChild(Node parent, Node child)
        {
            Debug.Log($"선 연결, parent : {parent}, child : {child}");
            var rootNode = parent as RootNode;      //부모가 루트이면
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
            var rootNode = parent as RootNode;      //부모가 루트이면
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

        public void SortIndex()     // 인덱스를 정렬한다.
        {
            nowChatIndex = 1;
            nodes.ForEach(n =>
            {
                var children = GetChildren(n);
                children.ForEach(c =>
                {
                    c.index = nowChatIndex;
                    c.indexLabel.text = nowChatIndex.ToString();
                    nowChatIndex++;
                });
            });
        }


    }
}
