using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ChatVisual
{
    public class ChatContainer : MonoBehaviour
    {
        public Node rootNode;
        public Hierarchy hierarchy = new Hierarchy();        // ��尡 �����Ǵ� ��.
        public List<Node> nodes = new List<Node>();         // ��� ����Ʈ

        [SerializeField]
        private Chapters[] chapters;     // é�͵�
        public Chapters[] Chapters { get { return chapters; } set { chapters = value; } }

#if UNITY_EDITOR
        //public Node CreateNode(Type type)
        //{
        //    var node = Activator.CreateInstance(type) as Node;
        //    node.guid = GUID.Generate().ToString();

        //    nodes.Add(node);

        //    AssetDatabase.SaveAssets();

        //    return node;
        //}
#endif
    }
}
