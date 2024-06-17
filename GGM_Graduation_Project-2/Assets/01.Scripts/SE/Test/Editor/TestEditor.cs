using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Test))]
public class TestEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // �⺻ �ν����͸� ǥ���մϴ�.
        DrawDefaultInspector();

        Test testScript = (Test)target;

        // ����Ʈ�� �׸���� ��ȸ�ϸ鼭 �ڽ� Ŭ������ �����鵵 ǥ���մϴ�.
        for (int i = 0; i < testScript.parent.Count; i++)
        {
            EditorGUILayout.LabelField($"Element {i}");

            Parent parent = testScript.parent[i];
            EditorGUI.indentLevel++;

            // �θ� Ŭ������ �ʵ� ǥ��
            parent.text = EditorGUILayout.TextField("Text", parent.text);

            // �� �ڽ� Ŭ������ Ÿ�Կ� ���� �ٸ��� ǥ��
            if (parent is Child child)
            {
                child.id = EditorGUILayout.IntField("ID", child.id);
            }
            else if (parent is Chidl2 child2)
            {
                child2.pi = EditorGUILayout.FloatField("Pi", child2.pi);
            }

            EditorGUI.indentLevel--;
        }
    }
}
