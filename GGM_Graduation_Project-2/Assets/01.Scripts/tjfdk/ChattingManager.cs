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
    private int replyCount = 0;     // first �� �� 2�� �ϰ� �ٷ� ������ ���ִ� ��.
    private bool is_SelectCriminalTiming = false;

    private string selectCriminal;

    private WaitForSeconds delay = new WaitForSeconds(0.75f);       // ��ȭ ������ �ð�

    private void Start()
    {
        Instance = this;
        selectCriminal = chats[chats.Length - 1].askAndReplySO[0].ask.GetReplys()[0];
        chattingHumanName.text = chats[0].whoSO.humanName;
        StartChatting(0);           // ���� ó���� 0���� �صα�
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
        if (chattingHumanName.text != chats[index].whoSO.humanName)     // �̸��� �ٸ���
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
                    assistantChatList[i].gameObject.SetActive(true);
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
            if (nowLevel == 1 && nowChatIndex == 2) InvisibleFileManager.Instance.ShowRoundFile("1-1");     // �б� ���� �����ֱ�
            bool state = chats[nowLevel].chatSO.chat[nowChatIndex].state == ChatState.Assistant ? false : true;       // �������� �÷��̾�(����) ���� ����ȯ. 1�� �÷��̾���.
            TextBox.Instance.InputText(state, chats[nowLevel].chatSO.chat[nowChatIndex].text, false);
            nowChatIndex++;
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
    }

    public void answer(string str)     // ��ư�� Ŭ������ ��
    {
        if (is_SelectCriminalTiming)
        {
            string name = str.Substring(4, 3);      // 3����
            TextBox.Instance.InputText(false, $"��. �׷� {name}���� �����ϰڽ��ϴ�.");
            StartCoroutine(End(name));
            return;     // ����
        }

        foreach (var replySO in chats[nowLevel].askAndReplySO)
        {
            if (replySO.ask.ask == str)
            {
                replySO.ask.is_used = true;
                StartCoroutine(ReplyPrint(replySO.ask.GetReplys()));
            }
        }
    }

    private IEnumerator ReplyPrint(string[] replys)     // first �������� �� ����ϵ���
    {
        if (chats[nowLevel].askAndReplySO[0].askName == "First")
        {
            string name = replys[0].Substring(0, replys[0].IndexOf(' '));
            UpLoadFile(null, name);       // ������ ���ε� �ϱ� ���ؼ�, ����, ������, �����ڰ� ����

            replyCount++;

            if (replyCount == 2)
            {
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

                name = remainder.Substring(0, remainder.IndexOf(' '));
                UpLoadFile(null, name);

                replyCount = 0;
                is_choosing = false;
                chats[nowLevel].chatSO.is_Ask = false;

                StartChatting(1);
                yield break;
            }
        }

        yield return delay;
        foreach (var text in replys)
        {
            TextBox.Instance.CurrentSpeechColorChange();
            TextBox.Instance.InputText(false, text);
            yield return delay;     // ������ ��ġ �Ǵ��ϱ�!
            if (text == selectCriminal)
            {
                Debug.Log("����ã��!");     // ������ ã�� �� �����ֱ�!
                is_SelectCriminalTiming = true;     // ������ ������ ã�� ��.
                TextBox.Instance.InputText(true, $"������ �̼�������");
                TextBox.Instance.InputText(true, $"������ Ȳ�ؿ�����");
                TextBox.Instance.InputText(true, $"������ ����������.");
                TextBox.Instance.InputText(true, $"������ ���±�����.");
                yield break;
            }
        }

        is_choosing = false;
        Chapter();
    }

    private void UpLoadFile(string round, string name = null)
    {
        if (name != null)
        {
            switch (name)
            {
                case "����":
                    InvisibleFileManager.Instance.ShowRoundFile("1-2");
                    InvisibleFileManager.Instance.ShowRoundFile("1-2-1");
                    break;
                case "������":
                    InvisibleFileManager.Instance.ShowRoundFile("1-3");
                    break;
                case "������":     // ��ǰ
                    InvisibleFileManager.Instance.ShowRoundFile("1-2");
                    InvisibleFileManager.Instance.ShowRoundFile("1-2-2");
                    break;
                default:
                    Debug.LogError("���� �̸��Դϴ�.");
                    break;
            }
            return;
        }
        InvisibleFileManager.Instance.ShowRoundFile(round);
    }

    private IEnumerator End(string answer)
    {
        yield return delay;
        Debug.Log(answer + "�� ����");
        SelectSuspectManager.Instance.Select(answer);
    }
}
