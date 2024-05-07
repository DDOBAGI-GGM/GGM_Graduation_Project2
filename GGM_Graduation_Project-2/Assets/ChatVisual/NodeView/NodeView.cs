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

        public NodeView(Node node) : base("Assets/ChatVisual/NodeView/NodeView.uxml")
        {
            this.node = node;
            this.title = node.GetType().Name;

            this.viewDataKey = node.guid;

            style.left = node.position.x;
            style.top = node.position.y;

            CreateInputPorts();
            CreateOutputPorts();
            SetUpClasses();

            Label descLabel = this.Q<Label>("description");
            descLabel.bindingPath = "description";
            //descLabel.Bind(new SerializedObject(node));      // node ��� Ŭ�������� description ������ ã�Ƽ� �־���.
        }

        private void CreateInputPorts()
        {
            switch (node)
            {
                case ChatNode:
                    input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
                    break;
                case AskNode:
                    input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
                    break;
                case LockAskNode:
                    input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
                    break;
            }

            if (input != null)
            {
                input.portName = "";
                inputContainer.Add(input);
            }
        }

        private void CreateOutputPorts()
        {
            switch (node)
            {
                case RootNode:
                    output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
                    break;
                case ChatNode:
                    output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
                    break;
                case AskNode:
                    output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
                    break;
                case LockAskNode:
                    output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
                    break;
            }

            if (output != null)
            {
                output.portName = "";
                output.style.marginLeft = new StyleLength(-15);
                outputContainer.Add(output);
            }
        }

        private void SetUpClasses()
        {
            switch(node)
            {
                case RootNode:
                    AddToClassList("root");     // ����
                    break;
                case ChatNode:
                    AddToClassList("chat");     // ����
                    break;
                case AskNode:
                    AddToClassList("ask");      // �ϴ�
                    break;
                case LockAskNode:
                    AddToClassList("lockAsk");      // ȸ��
                    break;
            }
        }

        public override void OnSelected()
        {
            base.OnSelected();
            OnNodeSelected.Invoke(this);
        }
    }
}
