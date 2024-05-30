using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ChatVisual
{
    public class HierarchyView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<HierarchyView, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits { }

        private ChatContainer chatContainer;
        private ChatView chatView;

        public void UpdateHierarchy(ChatContainer _chatContainer, ChatView _chatView)
        {
            chatContainer = _chatContainer;
            chatView = _chatView;

            // IMGUI ����ؼ� ���̾��Űâ ������ֱ�
            Clear();

            ScrollView scrollView = new ScrollView(ScrollViewMode.Vertical);
            scrollView.style.marginBottom = 5;
            scrollView.Add(new Label("Chapters :"));
            for (int i = 0; i < chatContainer.MainChapter.Count; ++i)
            {
                int index = i;
                string name = "";
                if (chatContainer.MainChapter[i].showName == null || chatContainer.MainChapter[i].showName == "")
                {
                    name = "???";
                }
                else
                {
                    name = chatContainer.MainChapter[i].showName;
                }
                var button = new Button(() => ChangeChapter(index))
                {
                    text = name + " - " + index.ToString()
                };
                button.style.flexGrow = 1;
                var deleteButton = new Button(() => DeleteChapter(index))
                {
                    text = "Delete"
                };
                var set = new VisualElement();
                set.style.flexDirection = FlexDirection.Row;
                set.Add(button);
                set.Add(deleteButton);
                scrollView.Add(set);
            }
            Add(scrollView);
        }

        private void ChangeChapter(int index)
        {
            chatView.SaveChatSystem();      // ���� é�� �������ֱ� 
            chatContainer.ChangeNowChapter(index);      // é�� �ѱ��
            chatView.LoadChatSystem(chatContainer);         // é�� �ε����ֱ�
            chatView.PopulateView();        // ���̴� �� �׷��ֱ�
        }

        private void DeleteChapter(int index)
        {
            chatContainer.MainChapter.RemoveAt(index);
            UpdateHierarchy(chatContainer, chatView);
        }
    }
}
