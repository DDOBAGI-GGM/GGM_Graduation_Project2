using ChatVisual;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

public class ChatEditor : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset treeAsset = null;

    private HierarchyView hierarchyView;

    private IMGUIContainer blackBoardView;      // �ڵ��� GUI, �Ʒ���

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

        hierarchyView = root.Q<HierarchyView>("hierarchy-view");        // ���̾��Űâ �̸����� ��������.
        blackBoardView = root.Q<IMGUIContainer>("inspector");       // �Ʒ��� �������� ���� blackboard ��.

    }
}
