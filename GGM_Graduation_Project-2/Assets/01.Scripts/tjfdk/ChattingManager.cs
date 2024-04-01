using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct Chapters
{
    public ChatSO chatSO;
    public AskAndReplySO[] askAndReplySO;
    public WhoSO whoSO;     // ������?
}

public class ChattingManager : MonoBehaviour
{
    public static ChattingManager Instance;

    [Header("ChattinggRoom")]
    public GameObject chatContainer;        // ���õ� ��� ����.
    public TMP_Text chattingHumanName;
    [HideInInspector]
    public List<GameObject> assistantChatList = new List<GameObject>();    // ������ ���� ��ȭ�� �������ֱ�

    [Space(25)]

    public Chapters[] chats;      // ���� SO���� �־���.

    private int nowChatIndex = 0;            // ���õ�
    [HideInInspector]
    public int nowLevel = 0;            // ���� ������ ����
    private bool is_choosing;       // �������� �־ �������� ��
    private bool is_SelectCriminalTiming = false;

    private int studentChatCount = 0;       // ó�� �л����� ��ȭ���� ����ī��Ʈ

    public float delayTime = 0.75f;
    private WaitForSeconds delay;       // ��ȭ ������ �ð�
    private WaitForSeconds delay2;       // ��ȭ ������ �ð�

    private void Start()
    {
        Instance = this;

        delay = new WaitForSeconds(delayTime);
        delay2 = new WaitForSeconds(delayTime * 3);

        chattingHumanName.text = chats[0].whoSO.humanName;
    }

    private void OnDisable()        // SO �ʱ�ȭ
    {
        foreach (var chats in chats)
        {
            foreach (var ask in chats.askAndReplySO)
            {
                ask.ask.is_used = false;        // ������� �ʾ���.
            }
        }
        chats[0].chatSO.is_Ask = true;      // ù��°���� ������ ����. �ϵ��ڵ�.
    }

    public void StartChatting(int index)
    {
        nowChatIndex = 0;
        nowLevel = index;
        if (index != 0) StopCoroutine(StartChattingCoroutine(index - 1));       // ������ ���ֱ�
        StartCoroutine(StartChattingCoroutine(index));          // ���ݲ� ����
    }

    private IEnumerator StartChattingCoroutine(int index)
    {
        // ����â ���� �������ֱ�
        //Debug.Log($"{chattingHumanName.text}, {chats[index].whoSO.humanName}");
        if (chattingHumanName.text != chats[index].whoSO.humanName)     // �ٸ� ����� ��ȭ�� �ϴ� ���̶��
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

            chattingHumanName.text = chats[index].whoSO.humanName;      // �̸� �־��ֱ�
        }

        int chatLenght = chats[index].chatSO.chat.Length;       // ���õ��� ����
        int askLenght = chats[index].askAndReplySO.Length == 0 ? 0 : 1;      // �������� ����
        for (int i = 0; i < chatLenght + askLenght; i++)
        {
            Chapter();
            yield return delay;
        }
    }

    private void Chapter()
    {
        if (is_choosing == false && nowChatIndex < chats[nowLevel].chatSO.chat.Length)        // �������� �ƴ϶��
        {
            bool state = chats[nowLevel].chatSO.chat[nowChatIndex].state == ChatState.Other ? false : true;       // �������� �÷��̾�(����) ���� ����ȯ. 1�� �÷��̾���.
            TextBox.Instance.InputText(state, chats[nowLevel].chatSO.chat[nowChatIndex].text, false);
            nowChatIndex++;

            if (nowLevel == 4 && nowChatIndex >= chats[nowLevel].chatSO.chat.Length)      // ù �л����� ��ȭ�� ������ �ߴٸ�.
            {
                StartCoroutine(EndOtherChat(5));
            }

            if (nowLevel == 5)    // ������ ������ ��û�ߴٸ�
            {
                if (nowChatIndex == 1) UpLoadFile("ä������");
                if (nowChatIndex == 5) UpLoadFile("��������ǰ");
            }

            if (nowLevel == 7 && nowChatIndex >= chats[nowLevel].chatSO.chat.Length)
            {
                StartCoroutine(EndOtherChat(8));
            }

            if (nowLevel == 8 && nowChatIndex >= chats[nowLevel].chatSO.chat.Length - 1)
            {
                Debug.Log("���屳�繰ǰ");
                UpLoadFile("���屳�繰ǰ");
            }

            if (nowLevel == 10 && nowChatIndex >= chats[nowLevel].chatSO.chat.Length)
            {
                StartCoroutine(EndOtherChat(11));
            }       // ����� ���� ����

            if (nowLevel == 13 && nowChatIndex >= chats[nowLevel].chatSO.chat.Length)
            {
                StartCoroutine(EndOtherChat(14));
            }       // Ȳ�ؿ��� ��ȭ ��

            if (nowLevel == 15 && nowChatIndex >= chats[nowLevel].chatSO.chat.Length)
            {
                Debug.Log("ȭ�� ����");
                Debug.Log("���� �б��� �ڻ� ����� �̹��� ó���� �ƴϿ���...");
            }
        }
        else if (nowChatIndex >= chats[nowLevel].chatSO.chat.Length && is_choosing == false)       // ���� ���� ������ �Ѿ��� �������� ���°� �ƴ� ��
        {
            if (chats[nowLevel].chatSO.is_Ask)        // ���� ������ �ִٸ�
            {
                foreach (var askSO in chats[nowLevel].askAndReplySO)      // �������� ���� �� ��� ������ֱ�
                {
                    if (askSO.ask.is_used == false)       // ������ ���� �����̿��ٸ�
                    {
                        is_choosing = true;
                        TextBox.Instance.InputText(true, askSO.ask.ask);
                    }
                }
            }
        }
        else
        {
            Debug.LogError($"�̰� �� ����, ������? : {is_choosing}, ���� ����? : {nowLevel}, ���� ä����? : {nowChatIndex}");
        }
    }


    public void answer(string str)     // ��ư�� Ŭ������ ��
    {
        TextBox.Instance.CurrentSpeechColorChange();
        foreach (var replySO in chats[nowLevel].askAndReplySO)
        {
            if (replySO.ask.ask == str)
            {
                replySO.ask.is_used = true;
                StartCoroutine(ReplyPrint(replySO.ask.GetReplys()));
            }
        }
    }

    private IEnumerator ReplyPrint(string[] replys)     // �������� ���� �� ����ϵ���
    {
        if (chats[nowLevel].askAndReplySO[0].askName == "First")
        {
            string name = replys[0].Substring(0, replys[0].IndexOf(' '));
            UpLoadFile(name);       // ������ ���ε� �ϱ� ���ؼ�, ����, �б� �� 2���� ����.

            yield return delay;
            TextBox.Instance.InputText(false, replys[0]);       // "~~~�� �Űܵ�Ⱦ��"
            yield return delay;

            string remainder = null;
            foreach (var noUse in chats[nowLevel].askAndReplySO)
            {
                if (noUse.ask.is_used == false)     // ���°� �ϳ� ã��
                {
                    remainder = noUse.ask.ask;          // "~~~���� ��"
                    remainder = remainder.Substring(0, remainder.IndexOf("����"));
                }
            }
            TextBox.Instance.InputText(false, $"�׸��� ������ {remainder}�� �Űܵ�Ⱦ��.");
            yield return delay;

            name = remainder.Substring(0, remainder.IndexOf(' '));
            UpLoadFile(name);

            is_choosing = false;
            chats[nowLevel].chatSO.is_Ask = false;

            StartChatting(1);       // �̾ ����Ǵ� ���̱� ������
            yield break;

        }

        yield return delay;

        foreach (var text in replys)        // ���� �߰����ֱ�
        {
            TextBox.Instance.InputText(false, text);
            yield return delay;     // ������ ��ġ �Ǵ��ϱ�!
        }

        is_choosing = false;

        // �� �Ʒ��δ� �亯�� �� �� �̻��� ���� �����ִ� ����.
        if (chats[nowLevel].askAndReplySO[0].askName == "StudentMeet")
        {
            StartCoroutine(EndOtherChat(3));
            yield break;
        }

        if (chats[nowLevel].askAndReplySO[0].askName == "Student")
        {
            studentChatCount++;
            if (studentChatCount == 3)      // 3���� ������ �ߴٸ�
            {
                Debug.Log("3�� �̻�");
                StartCoroutine(EndOtherChat(4));
                yield break;
            }
        }

        if (chats[nowLevel].askAndReplySO[0].askName == "JihyeonMeet")
        {
            StartCoroutine(EndOtherChat(7));
            yield break;
        }

        if (chats[nowLevel].askAndReplySO[0].askName == "HyeonSeokMeet")
        {
            StartCoroutine(EndOtherChat(10));
            yield break;
        }

        if (chats[nowLevel].askAndReplySO[0].askName == "LastMeet")
        {
            StartCoroutine(EndOtherChat(15));
            yield break;
        }

        Debug.Log("������� �´ٰ�?");
        Chapter();
    }

    private IEnumerator EndOtherChat(int next)
    {
        yield return delay2;
        StartChatting(next);
    }

    private void UpLoadFile(string round)
    {
        Debug.Log(round);
        switch (round)
        {
            case "�ʵ�":
            case "����":
                InvisibleFileManager.Instance.ShowRoundFile("����");
                break;
            case "�б�":
            case "�б���":
                InvisibleFileManager.Instance.ShowRoundFile("�б�");
                break;
            case "ä������":
                InvisibleFileManager.Instance.ShowRoundFile("ä������");
                break;
            case "��������ǰ":
                InvisibleFileManager.Instance.ShowRoundFile("��������ǰ");
                break;
            case "���屳�繰ǰ":
                InvisibleFileManager.Instance.ShowRoundFile("���屳�繰ǰ");
                break;
            default:
                Debug.LogError($"{round}�� ���� �̸��Դϴ�.");
                break;
        }
    }
}
