using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ChatVisual;       // ���߿� �����

public class ChattingManager : MonoBehaviour
{
    public static ChattingManager Instance;

    [Header("ChattingContainer")]
    public GameObject chatContainer;        // ���õ� ��� ����.
    public TMP_Text chattingHumanName;
    [HideInInspector]
    public List<GameObject> assistantChatList = new List<GameObject>();    // ������ ���� ��ȭ�� �������ֱ�
    [SerializeField] private Chapter[] chapters;      // ���� SO���� �־���.
    public Chapter[] Chapters {  get { return chapters; } set { chapters = value; } }

    [Header("ChatDelay")]       // ���� ������ ����
    public float delayTime = 0.75f;
    private float currentTime = 0.0f;
    private bool is_Chatting = false;       // ê���� �ϴ� ���̶��

    [Header("ChatCount")]       // ���� ������ �󸶳� ����Ǿ�����
    [HideInInspector]
    public int nowLevel = 0;            // ���� ������ ����
    private int nowChatIndex = 0;            // ���� ���� �ε���

    [Header("Ask")]     // ����� �� ����
    private int askLenght = 0;
    private bool is_Choosing;       // �������� �־ �������� ��. �����ִ� �ð��� ���ϴ� ��.
    private bool is_AskChat;
    private int nowAskLevel = 0;        // ���� ������ ����   
    private int nowAskChatIndex = 0;        // ���� ����� �ε���
    private List<string> notUseAskList = new List<string>();

    private void Start()
    {
        Instance = this;
    }

    private void Update()       // ���� �ý���
    {
        if (is_Chatting && !is_Choosing)
        {
            currentTime += Time.deltaTime;
            if (currentTime > delayTime || Input.GetMouseButtonDown(0))     // ���� ��ư�� �����ٸ�
            {
                if (is_AskChat) AskChapter();       // ������ ���� ���� �����.
                else Chapter();

                currentTime = 0.0f;
            }
        }
    }

    public void StartChatting(int index)
    {
        Debug.Log("ü���� ���۵�" + index);
        // ���� �� ������ ���������� ���� �������� ��� ��������??
        // ê���� ������ �׼����� �ٽ� �� �Լ��� �θ��� �Ѵ�?
        if (UIManager.Instance.panels[0].activeSelf == false)
        {
            UIManager.Instance.alarmIcon.SetActive(true);
            UIManager.Instance.chatIndex = index;
            UIManager.Instance.startChatEvent += (chatIndex) => StartChatting(chatIndex);
            return; 
        }

        nowChatIndex = 0;
        nowAskChatIndex = 0;
        nowLevel = index;

        // ����â ���� �������ֱ�
       /* if (chattingHumanName.text != chapters[index].who)     // �ٸ� ����� ��ȭ�� �ϴ� ���̶��
        {
            // ���ݱ��� �ִ� ��ȭ �� �����ֱ�
            for (int i = 0; i < chatContainer.transform.childCount; i++)
            {
                chatContainer.transform.GetChild(i).gameObject.SetActive(false);
            }

            if (chattingHumanName.text != "����")      // �����ڶ� ��ȭ�� �����̿��ٸ� ��ȭ ������ ���Ͽ� png �� �����ϰ� ��ȭ ������ ��� �����. �׸��� ������ ��ȭ�� ���ֱ�
            {
                Debug.Log("������ ��ȭ ���� ���������� �������ֱ�!");
                for (int i = 0; i < assistantChatList.Count; i++)
                {
                    if (assistantChatList[i].gameObject != null)
                    {
                        assistantChatList[i].gameObject.SetActive(true);
                    }
                }
            }

            //chattingHumanName.text = chapters[index].who;      // �̸� �־��ֱ�
        }*/

        int chatLenght = chapters[index].chat.Count;       // ���õ��� ����
        askLenght = chapters[index].askAndReply.Count;      // �������� ����

        is_Chatting = true;
    }

    private void Chapter()
    {
        if (is_Choosing == false && nowChatIndex < chapters[nowLevel].chat.Count)        // �������� �ƴ϶��
        {
            bool state = false;       // �������� �÷��̾�(����) ���� ����ȯ. 1�� �÷��̾���.
            switch (chapters[nowLevel].chat[nowChatIndex].state)
            {
                /*case E_ChatState.Other:
                    state = false;
                    break;
                case E_ChatState.Me:
                    state = true;
                    break;
                case E_ChatState.Ask:
                    notUseAskList.Clear();      // ���� �ִ� �� ��� �����ֱ�
                    for (int i = 0; i < askLenght; i++)
                    {
                        //TextBox.Instance.InputText(true, chapters[nowLevel].chat[nowChatIndex].text, true);
                        notUseAskList.Add(chapters[nowLevel].chat[nowChatIndex].text);            // ������ �߰�
                        nowChatIndex++;
                    }
                    is_Choosing = true;
                    return;
                case E_ChatState.LoadNext:
                    Debug.LogError("���� ������ �ʴ� LoadNext ����.");
                    return;     // �ƿ� ����*/
                default:
                    Debug.LogError($"{chapters[nowLevel].chat[nowChatIndex].state} ��(��) ���� �����̿���!");
                    break;
            }
            //TextBox.Instance.InputText(state, chapters[nowLevel].chat[nowChatIndex].text, false);
            nowChatIndex++;
        }
        else
        {
            is_Chatting = false;
            //Debug.LogError($"�̰� �� ����, ������? : {is_Choosing}, ���� ����? : {nowLevel}, ���� ä����? : {nowChatIndex}");
        }
    }

    public void answer(string str)     // ��ư�� Ŭ������ ��
    {
        //Debug.Log(str);
        //TextBox.Instance.CurrentSpeechColorChange();
        //for (int i = 0; i < notUseAskList.Count; i++)
        //{
        //    if (notUseAskList[i] == str)        // �̰��� ���ڶ����� �Է¿��� �ϳ��� ��丸�� ������ ����.
        //    {
        //        nowAskLevel = i;
        //        nowAskChatIndex = 0;
        //        notUseAskList[i] = "";

        //        is_AskChat = true;
        //        is_Choosing = false;
        //    }
        //}
    }

    private void AskChapter()
    {
        if (nowAskChatIndex < chapters[nowLevel].askAndReply[nowAskLevel].reply.Count)
        {
            //TextBox.Instance.InputText(false, chapters[nowLevel].askAndReply[nowAskLevel].reply[nowAskChatIndex]);
            nowAskChatIndex++;
        }
        else
        {
            is_AskChat = false;

            if (notUseAskList.Count == 0)       // �� ������ ���� ������
            {
                is_Choosing = false;
                return;
            }

            //for (int i = 0; i < notUseAskList.Count; i++)
            //{
            //    TextBox.Instance.InputText(true, notUseAskList[i], true);
            //}
            is_Choosing = true;
        }

    }

    public void ChangeDelaySpeed(float _value)
    {
        delayTime = _value;
    }       // ���� ������ �ð� ����
}
