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
    private List<MemberChat> memberChats;

    // UXLM
    VisualElement chatGround;
    VisualElement chattingFace;
    Button changeMemberButton;
    Label memberName;
    VisualElement memberList;

    // template
    VisualTreeAsset ux_myChat;
    VisualTreeAsset ux_otherChat;
    VisualTreeAsset ux_askChat;
    VisualTreeAsset ux_hiddenAskChat;
    VisualTreeAsset ux_memberList;

    private void Awake()
    {
        base.Awake();
        memberChats = new List<MemberChat>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            AddMember("������");
        if (Input.GetKeyDown(KeyCode.A))
            AddMember("��ä��");
        if (Input.GetKeyDown(KeyCode.W))
            InputChat(true, true, FindMember("������"), "Test");
        if (Input.GetKeyDown(KeyCode.E))
            InputChat(true, false, FindMember("������"), "Test1");
        if (Input.GetKeyDown(KeyCode.S))
            InputChat(true, true, FindMember("��ä��"), "Test2");
        if (Input.GetKeyDown(KeyCode.D))
            InputChat(true, false, FindMember("��ä��"), "Test3");
    }

    private MemberChat FindMember(string name)
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
    }

    private void UXML_Load()
    {
        chatGround = root.Q<VisualElement>("ChatGround");
        chattingFace = root.Q<VisualElement>("ChatFace");
        changeMemberButton = root.Q<Button>("ChangeTarget");
        memberName = root.Q<Label>("TargetName");
        memberList = root.Q<VisualElement>("ChatMemberList");
    }

    private void Template_Load()
    {
        ux_myChat = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\Chat\\MyChat.uxml");
        ux_otherChat = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\Chat\\OtherChat.uxml");
        ux_askChat = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\Chat\\AskChat.uxml");
        ux_hiddenAskChat = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\Chat\\HiddenAskChat.uxml");
        ux_memberList = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\Chat\\ChatMember.uxml");
    }

    private void Event_Load()
    {
        // ��� ���� ��ư
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
    public void InputChat(bool isRecord, bool isUser, MemberChat other, string msg, Sprite face = null)
    {
        // ����
        VisualElement chat = null;

        // ������ �����
        if (isUser)
            chat = RemoveContainer(ux_myChat.Instantiate());
        else
            chat = RemoveContainer(ux_otherChat.Instantiate());

        // ���� ǥ������ �ٲ��ֱ�
        if (face != null)
            chattingFace.style.backgroundImage = new StyleBackground(face);
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
        }
        else
            chat = RemoveContainer(ux_hiddenAskChat.Instantiate());

        // ��ȭ�� �߰�
        chatGround.Add(chat);

        if (isRecord)
        {
            Chatting chatting = new Chatting();
            chatting.isBool = isOpen;
            chatting.msg = msg;
            other.chattings.Add(chatting);
        }
    }

    public void AddMember(string memberName)
    {
        if (FindMember(memberName) == null)
        {
            VisualElement newMember = RemoveContainer(ux_memberList.Instantiate());
            newMember.Q<Button>("ChatMember").text = memberName;
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
            memberList.style.display = DisplayStyle.None;
        else
            memberList.style.display = DisplayStyle.Flex;
    }

    public void ChoiceMember(Button button)
    {
        Debug.Log("�巷��");
        //foreach (MemberChat member in memberChats)
        //{
        //    if (member.name == button.text)
        //    {
        //        ChangeMember(); // �̸� ��� �ݰ�
        //        RemoveChatting(); // ä�� ������
        //        RecallChatting(member); // ���� ����
        //    }
        //}
        MemberChat member = FindMember(button.text);
        if (member != null)
        {
            ChangeMember(); // �̸� ��� �ݰ�
            RemoveChatting(); // ä�� ������
            RecallChatting(member); // ���� ����
        }
    }
}
