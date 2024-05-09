using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class UIReader_Chatting : MonoBehaviour
{
    // other system
    TestUI mainSystem;
    UIReader_FileSystem fileSystem;
    UIReader_Connection connectionSystem;

    // main
    private UIDocument document;
    private VisualElement root;
    private VisualElement chatRoot;

    // UXLM
    VisualElement chatGround;
    VisualElement chattingFace;

    // template
    VisualTreeAsset ux_myChat;
    VisualTreeAsset ux_otherChat;
    VisualTreeAsset ux_askChat;
    VisualTreeAsset ux_hiddenAskChat;

    private void Awake()
    {
        mainSystem = GetComponent<TestUI>();
        fileSystem = GetComponent<UIReader_FileSystem>();
        connectionSystem = GetComponent<UIReader_Connection>();
    }

    private void OnEnable()
    {
        document = GetComponent<UIDocument>();
        root = document.rootVisualElement;
        chatRoot = root.Q<VisualElement>("");

        UXML_Load();
        Template_Load();
        Event_Load();
    }

    private void UXML_Load()
    {
        chatGround = root.Q<VisualElement>("ChatGround");
        chattingFace = root.Q<VisualElement>("ChatFace");
    }

    private void Template_Load()
    {
        ux_myChat = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\MyChat.uxml");
        ux_otherChat = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\OtherChat.uxml");
        ux_askChat = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\AskChat.uxml");
        ux_hiddenAskChat = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\HiddenAskChat.uxml");
    }

    private void Event_Load()
    {
    }

    // Function
    public void InputChat(bool isUser, string msg, Sprite face = null)
    {
        // ����
        VisualElement chat = null;

        // ������ �����
        if (isUser)
            chat = ux_myChat.Instantiate();
        else
            chat = ux_otherChat.Instantiate();

        // ���� ǥ������ �ٲ��ֱ�
        if (face != null)
            chattingFace.style.backgroundImage = new StyleBackground(face);
        // ��� ����
        chat.Q<Label>().text = msg;
        // ��ȭ�� �߰�
        chatGround.Add(chat);
    }

    public void InputQuestion(bool isOpen, string msg, Action action)
    {
        // ����
        VisualElement chat;

        // ����� Ǯ�� �����
        if (isOpen)
        {
            chat = ux_askChat.Instantiate();
            // ��� ����
            chat.Q<Label>().text = msg;
            // ��� �̺�Ʈ ����
            chat.Q<Button>().clicked += action;
        }
        else
            chat = ux_hiddenAskChat.Instantiate();

        // ��ȭ�� �߰�
        chatGround.Add(chat);
    }
}
