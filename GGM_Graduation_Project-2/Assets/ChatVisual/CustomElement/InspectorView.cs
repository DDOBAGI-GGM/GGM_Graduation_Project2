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
        private List<string> roundList = new List<string>();        // ������ ����(���ϸ����̸�)

        private bool is_Expand = false;

        public void UpdateSelection(NodeView node)      // ���� ��尡 �ٸ��Ÿ�
        {
            Clear();        // ������Ʈ ��� ���ְ�

            is_Expand = false;

            var container = new IMGUIContainer();
            Debug.Log(container);
            container.onGUIHandler = () =>
            {
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.fontSize = 20;
                GUILayout.Label($"{node.node.GetType().Name}", style);

                bool is_ChildExist = false;
                switch (node.node)
                {
                    case RootNode:
                        {
                            RootNode rootNode = node.node as RootNode;
                            if (rootNode.child != null) is_ChildExist = true;

                            GUILayout.Label("Description");
                            rootNode.description = EditorGUILayout.TextArea(rootNode.description, EditorStyles.textArea);
                            GUILayout.Space(15);

                            GUILayout.Label("ShowName");
                            rootNode.showName = EditorGUILayout.TextArea(rootNode.showName, EditorStyles.textArea);     // ������
                            GUILayout.Space(5);

                            GUILayout.Label("SaveLocation");
                            rootNode.saveLocation = (ESaveLocation)EditorGUILayout.EnumPopup(rootNode.saveLocation);        // �̳Ѱ� �ٲٱ�
                            GUILayout.Space(10);

                            // round �߰�
                            roundList = new List<string>(rootNode.round);
                            GUIStyle boxStyle = EditorStyles.helpBox;
                            GUILayout.BeginVertical(boxStyle);
                            is_Expand = EditorGUILayout.BeginFoldoutHeaderGroup(is_Expand, "Round List", menuAction: ShowHeaderContextMenu);
                            if (is_Expand)
                            {
                                for (int i = 0; i < roundList.Count; ++i)
                                {
                                    roundList[i] = EditorGUILayout.TextArea(roundList[i]);
                                }
                                if (GUILayout.Button("Add Round"))
                                {
                                    roundList.Add("");
                                }
                            }
                            rootNode.round = new List<string>(roundList);
                            EditorGUILayout.EndFoldoutHeaderGroup();
                            GUILayout.EndVertical();
                        }
                        break;
                    case ChatNode:
                        {
                            GUILayout.Space(15);
                            ChatNode chatNode = node.node as ChatNode;
                            if (chatNode.child.Count != 0) is_ChildExist = true;        // �ڽ��� �ϳ��� ������ 

                            enumValue = (int)chatNode.state;
                            enumValue = GUILayout.Toolbar(enumValue, System.Enum.GetNames(typeof(EChatState)));     // ���ϴ� �� Ÿ��
                            chatNode.state = (EChatState)enumValue;
                            GUILayout.Space(5);

                            GUILayout.Label("Chat");
                            chatNode.text = EditorGUILayout.TextArea(chatNode.text, EditorStyles.textArea);     // �ؽ�Ʈ
                            GUILayout.Space(10);

                            enumValue2 = (int)chatNode.face;
                            enumValue2 = GUILayout.Toolbar(enumValue2, System.Enum.GetNames(typeof(EFace)));        // ���� ���� ǥ��
                            chatNode.face = (EFace)enumValue2;
                            GUILayout.Space(10);

                            // ���� �̺�Ʈ �߰�
                            chatEventList = new List<EChatEvent>(chatNode.textEvent);
                            GUIStyle boxStyle = EditorStyles.helpBox;
                            GUILayout.BeginVertical(boxStyle);
                            is_Expand = EditorGUILayout.BeginFoldoutHeaderGroup(is_Expand, "Chat Event List", menuAction: ShowHeaderContextMenu);
                            if (is_Expand)
                            {
                                Debug.Log($"�̺�Ʈ {chatEventList.Count}");
                                for (int i = 0; i < chatEventList.Count; ++i)
                                {
                                    chatEventList[i] = (EChatEvent)EditorGUILayout.EnumPopup(chatEventList[i]);
                                }
                                if (GUILayout.Button("Add Chat Event"))
                                {
                                    chatEventList.Add(EChatEvent.Vibration);
                                }
                            }
                            chatNode.textEvent = new List<EChatEvent>(chatEventList);
                            EditorGUILayout.EndFoldoutHeaderGroup();
                            GUILayout.EndVertical();
                        }
                        break;
                    case AskNode:
                        {
                            GUILayout.Space(15);
                            AskNode askNode = node.node as AskNode;
                            if (askNode.child != null) is_ChildExist = true;
                            GUILayout.Space(10);

                            GUILayout.Label("Ask");
                            askNode.ask = EditorGUILayout.TextArea(askNode.ask, EditorStyles.textArea);     // ����
                            EditorGUI.BeginDisabledGroup(true);
                            EditorGUILayout.Toggle("is_UseThie", askNode.is_UseThis);
                        }
                        break;
                    case LockAskNode:
                        {
                            GUILayout.Space(15);
                            LockAskNode lockAskNode = node.node as LockAskNode;
                            if (lockAskNode.child != null) is_ChildExist = true;

                            GUILayout.Label("Ask");
                            lockAskNode.ask = EditorGUILayout.TextArea(lockAskNode.ask, EditorStyles.textArea);     // ����
                            GUILayout.Space(10);

                            // ����
                            evidenceList = new List<string>(lockAskNode.evidence);
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
                            lockAskNode.evidence = new List<string>(evidenceList);
                            EditorGUILayout.EndFoldoutHeaderGroup();
                            GUILayout.EndVertical();
                        }
                        break;
                }

                GUILayout.Space(5);
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
                Debug.Log("dl");
                chatEventList.Clear();
                evidenceList.Clear();
                roundList.Clear();
            });
            menu.DropDown(position);
        }
    }
}
