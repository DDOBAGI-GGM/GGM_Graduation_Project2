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
            scrollView.Add(new Label("Chapters :"));
            for (int i = 0; i < chatContainer.MainChapter.Count; ++i)
            {
                int index = i;
                string name = "";
                if (chatContainer.MainChapter[i].showName == null || chatContainer.MainChapter[i].showName == "")
                {
                    name = "��Ʈ ��忡�� �̸��� �߰����ּ���.";
                }
                else
                {
                    name = chatContainer.MainChapter[i].showName;
                }
                var button = new Button(() => ChangeChapter(index))
                {
                    text = name + " - " + (index + 1).ToString()
                };
                scrollView.Add(button);
            }
            Add(scrollView);
        }

        private void ChangeChapter(int index)
        {
            chatView.SaveChatSystem();
            chatContainer.ChangeNowChapter(index);
            chatView.LoadChatSystem(chatContainer); 
            chatView.PopulateView();
        }
    }
}
