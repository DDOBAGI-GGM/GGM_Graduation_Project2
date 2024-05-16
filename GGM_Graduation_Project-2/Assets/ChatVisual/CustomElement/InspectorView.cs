using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Collections.Generic;
using UnityEditorInternal;
using Codice.Client.Common;

namespace ChatVisual
{
    public class InspectorView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits { }

        private int enumValue;
        private int enumValue2;

        private List<EChatEvent> chatEventList = new List<EChatEvent>();     // ���õ�
        private List<string> evidenceList = new List<string>();     // ���ŵ�

        private bool is_Expand = false;

        //public InspectorView()
        //{

        //}

        public void UpdateSelection(NodeView node)      // ���� ��尡 �ٸ��Ÿ�
        {
            Clear();        // ������Ʈ ��� ���ְ�

            is_Expand = false;

            var container = new IMGUIContainer();
            container.onGUIHandler = () =>
            {
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.fontSize = 20;
                GUILayout.Label($"{node.node.GetType().Name}", style);
                GUILayout.Label("Description");
                node.node.description = EditorGUILayout.TextArea(node.node.description, EditorStyles.textArea);
                GUILayout.Space(15);

                bool is_ChildExist = false;
                switch (node.node)
                {
                    case RootNode:
                        RootNode rootNode = node.node as RootNode;
                        if (rootNode.child != null) is_ChildExist = true;
                        break;
                    case ChatNode:
                        {
                            ChatNode chatNode = node.node as ChatNode;
                            if (chatNode.child.Count != 0) is_ChildExist = true;        // �ڽ��� �ϳ��� ������ 

                            enumValue = (int)chatNode.state;
                            enumValue = GUILayout.Toolbar(enumValue, System.Enum.GetNames(typeof(EChatState)));     // ���ϴ� �� Ÿ��
                            chatNode.state = (EChatState)enumValue;
                            
                            GUILayout.Label("Chat");
                            chatNode.text = EditorGUILayout.TextArea(chatNode.text, EditorStyles.textArea);     // �ؽ�Ʈ
                            
                            enumValue = (int)chatNode.face;
                            enumValue2 = GUILayout.Toolbar(enumValue2, System.Enum.GetNames(typeof(EFace)));        // ���� ���� ǥ��
                            chatNode.face = (EFace)enumValue2;

                            // ���� �̺�Ʈ �߰�
                            chatEventList = chatNode.textEvent;
                            GUIStyle boxStyle = EditorStyles.helpBox;
                            GUILayout.BeginVertical(boxStyle);
                            is_Expand = EditorGUILayout.BeginFoldoutHeaderGroup(is_Expand, "Chat Event List", menuAction: ShowHeaderContextMenu);
                            if (is_Expand)
                            {
                                for (int i = 0; i < chatEventList.Count; ++i)
                                {
                                    chatEventList[i] = (EChatEvent)EditorGUILayout.EnumPopup(chatEventList[i]);
                                }
                                if (GUILayout.Button("Add Evidence"))
                                {
                                    chatEventList.Add(EChatEvent.None);
                                }
                            }
                            EditorGUILayout.EndFoldoutHeaderGroup();
                            GUILayout.EndVertical();

                        }
                        break;
                    case AskNode:
                        {
                            AskNode askNode = node.node as AskNode;
                            if (askNode.child != null) is_ChildExist = true;

                            GUILayout.Label("Ask");
                            askNode.ask = EditorGUILayout.TextArea(askNode.ask, EditorStyles.textArea);     // ����
                            EditorGUI.BeginDisabledGroup(true);
                            EditorGUILayout.Toggle("is_UseThie", askNode.is_UseThis);
                        }
                        break;
                    case LockAskNode:
                        {
                            LockAskNode lockAskNode = node.node as LockAskNode;
                            if (lockAskNode.child != null) is_ChildExist = true;

                            // ����
                            evidenceList = lockAskNode.evidence;
                            GUIStyle boxStyle = EditorStyles.helpBox;
                            GUILayout.BeginVertical(boxStyle);
                            is_Expand = EditorGUILayout.BeginFoldoutHeaderGroup(is_Expand, "Evidence List", menuAction: ShowHeaderContextMenu);
                            if (is_Expand)
                            {
                                for (int i = 0; i < evidenceList.Count; ++i)
                                {
                                    evidenceList[i] = GUILayout.TextField(evidenceList[i]);
                                }
                                if (GUILayout.Button("Add Evidence"))
                                {
                                    evidenceList.Add(null);
                                }
                            }
                            EditorGUILayout.EndFoldoutHeaderGroup();
                            GUILayout.EndVertical();

                            GUILayout.Label("Ask");
                            lockAskNode.ask = EditorGUILayout.TextArea(lockAskNode.ask, EditorStyles.textArea);     // ����
                        }
                        break;
                }

                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.Toggle("child", is_ChildExist);
                EditorGUI.EndDisabledGroup();
            };

            Add(container);     // UI �����̳ʿ� �־���, ������ ���̰� ����.
        }

        private void ShowHeaderContextMenu(Rect position)       // �޴� ���� Ŭ���� ������ֱ�
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Clear"), false, () =>
            {
                chatEventList.Clear();
                evidenceList.Clear();
            });
            menu.DropDown(position);
        }
    }
}
