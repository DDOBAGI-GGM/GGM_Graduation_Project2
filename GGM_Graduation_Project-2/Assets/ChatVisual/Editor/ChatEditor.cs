using ChatVisual;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System;

public class ChatEditor : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset treeAsset = null;           // UI ���� �־��ֱ�

    private ChatView chatView;        // ê�õ� �� �ִ� ��.
    private InspectorView inspectorView;        // �ν����� �� ����.
    private IMGUIContainer hierarchyView;      // �ڵ��� GUI, ������ ���ϴ� ��. ���̾��Ű.
    private Button arrayAddBtn;

    private SerializedObject chatObject;        // �����Ϳ��� ����ϱ� ���� ����ȭ
    private SerializedProperty chatProperty;        // �������� �Ӽ��� ����.

    private ChatContainer chatContainer;

    [MenuItem("ChatSystem/ChatEditor")]
    public static void OpenWindow()
    {
        GetWindow<ChatEditor>("ChatEditor");
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;     // ��Ʈ�� ��������.

        // UXML ������ֱ�
        VisualElement template = treeAsset.Instantiate();
        template.style.flexGrow = 1;        // ��� �ȿ� �־��� �� �󸶳� Ŀ�� �� �ΰ�
        root.Add(template);

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/ChatVisual/Editor/ChatEditor.uss");
        root.styleSheets.Add(styleSheet);       // �� ��Ÿ���� ��������.

        chatView = root.Q<ChatView>("chat-view");
        inspectorView = root.Q<InspectorView>("inspector-view");        // �ν����� �̸����� ��������.
        hierarchyView = root.Q<IMGUIContainer>("hierarchy-view");       // �Ʒ��� �������� hierarchy ��.
        hierarchyView.onGUIHandler = () =>         // hierarchyView �� ����. 
        {
            if (chatObject != null && chatObject.targetObject != null)
            {
                chatObject.Update();        // �������ֱ�
                EditorGUILayout.PropertyField(chatProperty);
                chatObject.ApplyModifiedProperties();
            }
        };
        arrayAddBtn = root.Q<Button>("AddBtn");     // ��ư ��������
        arrayAddBtn.clickable.clicked += OnArrayAddBtn;

        chatView.OnNodeSelected += OnSelectionNodeChanged;      // ��带 ���� ���� �޶����� �� �̺�Ʈ ȣ��

        OnSelectionChange();
    }

    private void OnArrayAddBtn()
    {
        if (chatContainer != null)
        {
            Debug.Log("�迭 �߰����ֱ�");
        }
    }

    private void OnSelectionNodeChanged(NodeView nodeView)
    {
        Debug.Log("����");
        inspectorView.UpdateSelection(nodeView);        // �� ��带 �����ٰ� �ν����Ϳ� �˷���.
    }

    private void OnSelectionChange()        // �����͸� Ų ���¿��� ���𰡸� �������� ��
    {
        if (Selection.activeGameObject != null)
        {
            if (Selection.activeGameObject.TryGetComponent<ChatContainer>(out chatContainer))      // ���̾��Ű â���� ��������
            {
                chatContainer.ChangeNowChapter(0);      // �ϴ� 0���� �����ϰ�
                chatView.PopulateView(chatContainer);           // ä�����

                Debug.Log($"{chatContainer.Chapters.Length}��ŭ ����Ʈ�� �����Ǿ�� ��.");

                chatObject = new SerializedObject(chatContainer);       // ����ȭ ���ֱ�
                chatProperty = chatObject.FindProperty("hierarchy");       // �Ӽ� ã�Ƽ� �־��ֱ�
                //chatProperty = chatObject.FindProperty("chapters");       // �Ӽ� ã�Ƽ� �־��ֱ�
            }
        }
    }
}
