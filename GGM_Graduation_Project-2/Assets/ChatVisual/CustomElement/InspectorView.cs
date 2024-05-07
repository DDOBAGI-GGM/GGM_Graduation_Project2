using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace ChatVisual
{
    public class InspectorView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits { }

        private SerializedObject inspectorObject;
        private SerializedProperty inspectorProperty;

        public InspectorView()
        {

        }

        public void UpdateSelection(NodeView node)      // ���� ��尡 �ٸ��Ÿ�
        {
            Clear();        // ������Ʈ ��� ���ְ�

            var container = new IMGUIContainer();
            /*container.onGUIHandler = () =>
            {
                inspectorObject = new SerializedObject(node.node.nodeContainer.GetComponent<Node.NodeContainer>());
                inspectorProperty = inspectorObject.FindProperty("no");
                Debug.Log($"{inspectorObject}, {inspectorProperty}");

                if (inspectorObject != null && inspectorObject.targetObject != null)
                {
                    inspectorObject.Update();
                    EditorGUILayout.PropertyField(inspectorProperty);
                    inspectorObject.ApplyModifiedProperties();
                }
            };*/

            Add(container);     // UI �����̳ʿ� �־���, ������ ���̰� ����.
        }
    }
}
