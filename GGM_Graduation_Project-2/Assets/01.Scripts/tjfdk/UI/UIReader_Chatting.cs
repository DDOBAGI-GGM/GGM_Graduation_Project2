using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;

[Serializable]
public struct Chatting
{
    public bool isBool;
    public string msg;
}

[Serializable]
public class MemberChat
{
    public string name;
    public Sprite face;
    public bool isOpen;
    public List<Chatting> chattings = new List<Chatting>();
    public List<Chatting> quetions = new List<Chatting>();
    public MemberChat(string name)
    {
        this.name = name;
    }
}

public class UIReader_Chatting : UI_Reader
{
    [SerializeField]
    private List<MemberChat> memberChats = new List<MemberChat>();
    [SerializeField]
    private Texture2D changeMemberBtnOn, changeMemberBtnOff;

    // UXLM
    VisualElement chatGround;
    VisualElement questionGround;
    VisualElement chattingFace;
    Button changeMemberButton;
    Label memberName;
    VisualElement memberList;

    // template
    VisualTreeAsset ux_chat;
    VisualTreeAsset ux_askChat;
    VisualTreeAsset ux_hiddenAskChat;
    VisualTreeAsset ux_memberList;

    private void Awake()
    {
        base.Awake();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            AddMember("������");
        if (Input.GetKeyDown(KeyCode.A))
            AddMember("��ä��");
        if (Input.GetKeyDown(KeyCode.W))
            InputChat(true, true, FindMember("������"), "������");
        if (Input.GetKeyDown(KeyCode.E))
            InputChat(true, false, FindMember("������"), "�Ⱦ�");
        if (Input.GetKeyDown(KeyCode.S))
            InputChat(true, true, FindMember("��ä��"), "ä�ξ�");
        if (Input.GetKeyDown(KeyCode.D))
            InputChat(true, false, FindMember("��ä��"), "�� �̸� �θ��� ��");

        //EndToScroll();
    }

    public MemberChat FindMember(string name)
    {
        foreach(MemberChat member in memberChats)
        {
            if (member.name == name)
                return member;
        }

        return null;
    }

    private void OnEnable()
    {
        base.OnEnable();

        Template_Load();
        UXML_Load();
        Event_Load();

        chatGround.Q<ScrollView>("ChatGround").scrollDecelerationRate = 0.01f;
    }

    private void UXML_Load()
    {
        chatGround = root.Q<VisualElement>("ChatGround");
        questionGround = root.Q<VisualElement>("QuestionGround");
        chattingFace = root.Q<VisualElement>("FaceGround").Q<VisualElement>("OtherFace");
        changeMemberButton = root.Q<Button>("ChangeTarget");
        memberName = root.Q<Label>("TargetName");
        memberList = root.Q<VisualElement>("ChatMemberList");
    }

    private void Template_Load()
    {
        ux_chat = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\Chat\\Chat.uxml");
        ux_askChat = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\Chat\\AskChat.uxml");
        ux_hiddenAskChat = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\Chat\\HiddenAskChat.uxml");
        ux_memberList = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\Chat\\ChatMember.uxml");
    }

    private void Event_Load()
    {
        // ��� ���� ��ư
        ChangeMember();
        changeMemberButton.clicked += ChangeMember;
        // ä�� ��ũ�Ѻ� �ӵ� ����
        //Debug.Log(chatGround.Q<ScrollView>(chatGround.name));
        //chatGround.Q<ScrollView>(chatGround.name).scrollDecelerationRate = 5f;
    }

    private void RemoveChatting()
    {
        for (int i = chatGround.childCount - 1; i >= 0; i--)
            chatGround.RemoveAt(i);
    }

    private void RecallChatting(MemberChat otherName)
    {
        memberName.text = otherName.name;

        foreach (Chatting chat in otherName.chattings)
            InputChat(false, chat.isBool, otherName, chat.msg);

        foreach(Chatting chat in otherName.quetions)
            InputQuestion(false, chat.isBool, otherName, chat.msg, () => { });

        // ��ũ�ѹ� �� �Ʒ��� ������
        //chatGround.Q<ScrollView>(chatGround.name).verticalScroller.value 
        //    = chatGround.Q<ScrollView>(chatGround.name).verticalScroller.highValue;
    }

    // Function

    public enum ChatType
    {
        String,
        Question,
        Image,
        CutScene
    }

    public void InputChatting(bool isUser, bool isChat, ChatType chatType, string msg)
    {
        VisualElement chat = null;

        switch (chatType)
        {
            case ChatType.String:
                chat = ux_chat.Instantiate();
                chat.Q<Label>().text = msg;
                break;
            case ChatType.Image:
                chat = new VisualElement();
                chat.style.backgroundImage = new StyleBackground(imageManager.FindPNG(msg).image);
                break;
            case ChatType.CutScene:
                chat = new Button();
                chat.style.backgroundImage = new StyleBackground(imageManager.FindPNG(msg).image);
                break;
        }

        // ������ �����
        if (isUser)
            chat.AddToClassList("MyChat");
        else
            chat.AddToClassList("OtherChat");


    }

    public void InputChat(bool isRecord, bool isUser, MemberChat other, string msg, Sprite face = null)
    {
        // ����
        VisualElement chat = RemoveContainer(ux_chat.Instantiate());

        // ������ �����
        if (isUser)
            chat.AddToClassList("MyChat");
        else
            chat.AddToClassList("OtherChat");

        // ���� ǥ������ �ٲ��ֱ�
        if (face != null)
            chattingFace.Q<VisualElement>("Face").style.backgroundImage = new StyleBackground(face);
        // ��� ����
        chat.Q<Label>().text = msg;
        // ��ȭ�� �߰�
        chatGround.Add(chat);

        if (isRecord)
        {
            Debug.Log("�����");
            Chatting chatting = new Chatting();
            chatting.isBool = isUser;
            chatting.msg = msg;
            other.chattings.Add(chatting);
        }

        EndToScroll();
    }

    public void InputQuestion(bool isRecord, bool isOpen, MemberChat other, string msg, Action action)
    {
        // ����
        VisualElement chat;

        // ����� Ǯ�� �����
        if (isOpen)
        {
            chat = RemoveContainer(ux_askChat.Instantiate());
            // ��� ����
            chat.Q<Label>().text = msg;
            // ��� �̺�Ʈ ����
            chat.Q<Button>().clicked += action;
            chat.Q<Button>().clicked += (() => { chat.parent.Remove(chat); });
        }
        else
            chat = RemoveContainer(ux_hiddenAskChat.Instantiate());

        // ��ȭ�� �߰�
        questionGround.Add(chat);
        EndToScroll();

        if (isRecord)
        {
            Chatting chatting = new Chatting();
            chatting.isBool = isOpen;
            chatting.msg = msg;
            other.chattings.Add(chatting);
        }

    }

    private void EndToScroll()
    {
        ScrollView scrollView = chatGround.Q<ScrollView>("ChatGround");

        scrollView.schedule.Execute(() =>
        {
            float contentHeight = scrollView.contentContainer.layout.height;
            float viewportHeight = scrollView.contentViewport.layout.height;

            scrollView.scrollOffset = new Vector2(0, contentHeight - viewportHeight);
        });
    }

    public void AddMember(string memberName)
    {
        if (FindMember(memberName).isOpen == false)
        {
            FindMember(memberName).isOpen = true;

            VisualElement newMember = RemoveContainer(ux_memberList.Instantiate());
            newMember.Q<Label>("Name").text = memberName;
            newMember.Q<VisualElement>("Face").style.backgroundImage = new StyleBackground(FindMember(memberName).face);
            newMember.Q<Button>("ChatMember").clicked += () =>
            {
                ChoiceMember(newMember.Q<Button>("ChatMember"));
            };

            memberList.Add(newMember);
            memberChats.Add(new MemberChat(memberName));
        }
    }

    private void Test()
    {
        Debug.Log("����");
    }

    public void ChangeMember()
    {
        if (memberList.style.display.value == DisplayStyle.Flex)
        {
            changeMemberButton.style.backgroundImage = new StyleBackground(changeMemberBtnOn);
            memberList.style.display = DisplayStyle.None;
        }
        else
        {
            changeMemberButton.style.backgroundImage = new StyleBackground(changeMemberBtnOff);
            memberList.style.display = DisplayStyle.Flex;
        }
    }

    public void ChangeProfile(string name, Sprite face)
    {
        chattingFace.Q<Label>("Name").text = name;
        chattingFace.Q<VisualElement>("Face").style.backgroundImage = new StyleBackground(face);
    }

    public void ChoiceMember(Button button)
    {
        Debug.Log("dhktek");
        MemberChat member = FindMember(button.Q<Label>("Name").text);
        if (member != null)
        {
            ChangeProfile(member.name, member.face);
            ChangeMember(); // �̸� ��� �ݰ�
            RemoveChatting(); // ä�� ������
            RecallChatting(member); // ���� ����
        }
    }
}
