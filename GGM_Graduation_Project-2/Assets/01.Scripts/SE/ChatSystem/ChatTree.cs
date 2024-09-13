using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChatVisual
{
    [CreateAssetMenu(menuName = "SO/ChatTree")]
    public class ChatTree : ScriptableObject
    {
        public RootNode rootNode;

        public MemberProfile humanInfo;
        public List<Node> nodeList = new List<Node>();

        public void AddChild(Node parent, Node child)
        {
            AddParent(parent, child);

            var rootNode = parent as RootNode;
            if (rootNode != null)
            {
                rootNode.child = child;
                return;
            }

            var chatNode = parent as ChatNode;
            if (chatNode != null)
            {
                chatNode.childList.Add(child);
                return;
            }

            var askNode = parent as AskNode;
            if (askNode != null)
            {
                askNode.child = child;
                return;
            }

            var conditionNode = parent as ConditionNode;
            if (conditionNode != null)
            {
                conditionNode.childList.Add(child);
            }
        }

        public void AddParent(Node parent, Node child)
        {
            var chatNode = child as ChatNode;
            if (chatNode != null)
            {
                chatNode.parent = parent;
                return;
            }

            var askNode = child as AskNode;
            if (askNode != null)
            {
                askNode.parent = parent;
                return;
            }

            var conditionNode = child as ConditionNode;
            if (conditionNode != null)
            {
                conditionNode.parentList.Add(parent);
            }
        }

        public List<Node> GetChild(Node parent)
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

            var chatNode = parent as ChatNode;
            if (chatNode != null && chatNode.childList.Count != 0)
            {
                children = chatNode.childList;
                return children;
            }

            var conditionNode = parent as ConditionNode;
            if (conditionNode != null && conditionNode.childList.Count != 0)
            {
                children = conditionNode.childList;
            }

            return children;
        }
    }
}