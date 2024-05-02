using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditorInternal.VR;

namespace ChatVisual
{
    public class InspectorView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits { }

        private Editor editor;

        public InspectorView()
        {

        }

        public void UpdateSelection(NodeView node)      // ���� ��尡 �ٸ��Ÿ�
        {
            Clear();        // ������Ʈ ��� ���ְ�
            Object.DestroyImmediate(editor);        // �����͸� �����ְ�

            //editor = Editor.CreateEditor(node.node);     // ����Ƽ �⺻ �ν����ͺ並 ������ش�.

            var container = new IMGUIContainer(() => {
                if (editor.target)      // ���� ���� ��������, �������� �갡 �ִٸ�
                {
                    editor.OnInspectorGUI();     // �����̳ʷ� �־���
                }
            });

            Add(container);     // UI �����̳ʿ� �־���, ������ ���̰� ����.
        }
    }
}
