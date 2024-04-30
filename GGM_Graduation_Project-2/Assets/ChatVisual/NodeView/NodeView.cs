using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ChatVisual
{
    public class NodeView : UnityEditor.Experimental.GraphView.Node
    {
        public Node node;

        public Port input;      // �Է�
        public Port output;     // ���

        public Action<NodeView> OnNodeSelected;

        public NodeView(Node node) : base("Assets/ChatVisual/Editor/NodeView/NodeView.uxml")
        {
            this.node = node;
            this.title = node.name;

            //this.viewDataKey = node.guid;

            style.left = node.position.x;
            style.top = node.position.y;

            CreateInputPorts();
            CreateOutputPorts();
            SetUpClasses();

            Label descLabel = this.Q<Label>("description");
            descLabel.bindingPath = "description";
            descLabel.Bind(new SerializedObject(node));      // node ��� Ŭ�������� description ������ ã�Ƽ� �־���.
        }

        private void CreateInputPorts()
        {
            
        }



    }
}
