using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public struct Chapters
{
    public ChatSO chatSO;
    public AskAndReplySO[] askAndReplySO;
}

public class ChattingManager : MonoBehaviour
{
    public static ChattingManager Instance;

    public Chapters[] chats;      // ���� SO���� �־���.
    public int nowChatIndex = 0;            // ���õ�
    public int nowLevel = 0;            // ���� ������ ����
    private bool is_choosing;       // �������� �־ �������� ��
    private bool is_Player;      // �÷��̾ ���ϴ� ���ΰ�
    private int replyCount = 0;     // first �� �� 2�� �ϰ� �ٷ� ������ ���ִ� ��.

    private string selectCriminal;

    private WaitForSeconds delay = new WaitForSeconds(0.75f);       // ��ȭ ������ �ð�

    private void Start()
    {
        Instance = this;
        selectCriminal = chats[chats.Length - 1].askAndReplySO[0].ask.reply;
        StartChatting(5);           // ���� ó���� 0���� �صα�
    }

    private void OnDisable()        // SO �ʱ�ȭ
    {
        foreach (var chats in chats)
        {
            foreach(var ask in chats.askAndReplySO)
            {
                ask.ask.is_used = false;        // ������� �ʾ���.
            }
        }
        chats[0].chatSO.is_Ask = true;      // ù��°���� ������ ����. �ϵ��ڵ�.
    }

    public void StartChatting(int index)
    {
        if (index != 0) StopCoroutine(StartChattingCoroutine(index - 1));       // ������ ���ֱ�
        StartCoroutine(StartChattingCoroutine(index));          // ���ݲ� ����
    }

    private IEnumerator StartChattingCoroutine(int index)
    {
        int chatLenght = chats[index].chatSO.chat.Length;       // ���õ��� ����
        Debug.Log(chatLenght);
        int askLenght = chats[index].askAndReplySO.Length == 0 ? 0 : 1;      // �������� ����
        nowLevel = index;
        for (int i = 0; i < chatLenght + askLenght; i++)
        {
            Chapter();
            yield return delay;
        }
    }

    public void Chapter()
    {
        if (is_choosing == false && nowChatIndex < chats[nowLevel].chatSO.chat.Length)        // �������� �ƴ϶��
        {
            bool state = chats[nowLevel].chatSO.chat[nowChatIndex].state == ChatState.Assistant ? false : true;       // �������� �÷��̾�(����) ���� ����ȯ. 1�� �÷��̾���.
            TextBox.Instance.InputText(state, chats[nowLevel].chatSO.chat[nowChatIndex].text);
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
            else
            {
                nowLevel++;
                nowChatIndex = 0;
                Chapter();
            }
        }
    }

    public void answerr(string str)     // ��ư�� Ŭ������ ��
    {
        Debug.Log(str);
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
            replyCount++;
            Debug.Log(replyCount);
            if (replyCount == 2)
            {
                yield return delay;
                TextBox.Instance.InputText(false, replys[0]);       // "~~~�� �Űܵ�Ⱦ��"
                yield return delay;
                string remainder = null;
                foreach (var noUse in chats[nowLevel].askAndReplySO)
                {
                    if (noUse.ask.is_used == false)
                    {
                        remainder = noUse.ask.ask;          // "~~~���� ��"
                        remainder = remainder.Substring(0, remainder.IndexOf("����"));
                    }
                }
                TextBox.Instance.InputText(false, $"�׸��� ������ {remainder}�� �Űܵ�Ⱦ��.");
                replyCount = 0;
                is_choosing = false;
                chats[nowLevel].chatSO.is_Ask = false;
                StartChatting(1);
                yield break;
            }
        }

        yield return delay;
        foreach(var text in replys)
        {
            TextBox.Instance.InputText(false, text);
            yield return delay;     // ������ ��ġ �Ǵ��ϱ�!
            if (text == selectCriminal)
            {
                Debug.Log("����ã��!");     // ������ ã�� �� �����ֱ�!
                TextBox.Instance.InputText(false, $"��. �׷� {000}���� �����ϰڽ��ϴ�.");
                yield break;
            }
        }

        is_choosing = false;
        Chapter();
    }
}
