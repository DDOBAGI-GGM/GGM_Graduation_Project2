using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace ChatVisual
{
    public class ChatView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<ChatView, UxmlTraits> { }
        public class UmalTraits : UxmlTraits { }

        public ChatView()
        {
            Insert(0, new GridBackground());        // �׸��� �ֱ�

            this.AddManipulator(new ContentZoomer());       // �ܱ�� ���� �߰�
            this.AddManipulator(new ContentDragger());  // ������ �巡�� ����
            this.AddManipulator(new SelectionDragger());    // �������ذ� �����̱�
            //this.AddManipulator(new RectangleSelector());   // �׸� ������ֱ�  ���۵� �߰�
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)       // ��Ŭ�� ���� �� ���� �޴���
        {
            /*if (??? == null)
            {
                evt.StopPropagation();      // �̺�Ʈ ���� ����
                return;
            }*/
            Debug.Log("��Ŭ����");
        }

    }
}