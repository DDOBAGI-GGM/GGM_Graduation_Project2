using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChattingManager : Singleton<ChattingManager>
{
    [SerializeField]
    private List<DialogueSO> chapterSO = new List<DialogueSO>();

    public ChatSO[] chats;      // ���� SO���� �־���.
    public int nowChatIndex = 0;
    private bool is_choosing;       // �������� �־ �������� ��
    private bool is_Player;      // �÷��̾ ���ϴ� ���ΰ�

    public int currentChapter = 0;      // ���� é��
    public int currentStep = 0;         // ���� é���� ��ȭ��

    public bool isChoice = false;       // �������� ���� �ִ� ���� ��
    public bool isFunc;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
            Chapter();

        if (Input.GetKeyDown(KeyCode.C))
        {
            Chapterr();
        }
    }

    public void Chapter()
    {
        isFunc = false;

        if (isChoice == false)      // �� ���� �ִ� ���¶��
        {
            TextBox.Instance.InputText(false, chapterSO[currentChapter].temp[currentStep].text);
            Debug.Log(chapterSO[currentChapter].temp[currentStep].text);

            if (chapterSO[currentChapter].temp[currentStep].next.Count == 0)
            {
                currentStep++;
                Debug.Log("���� ����");
            }
            else
            {
                foreach (test ttt in chapterSO[currentChapter].temp[currentStep].next)      // �������� ���� �� ��� ������ֱ�
                {
                    if (ttt.isDone == false)
                    {
                        isChoice = true;
                        TextBox.Instance.InputText(true, ttt.text);
                        isFunc = true;
                    }
                }

                if (isFunc == false)
                    currentStep++;
            }
        }
    }

    public void Chapterr()
    {
        if (is_choosing == false)        // �������� �ƴ϶��
        {
            bool state = chats[0].chat[nowChatIndex].state == ChatState.Assistant ? false : true;       // �������� �÷��̾�(����) ���� ����ȯ. 1�� �÷��̾���.
            TextBox.Instance.InputText(state, chats[0].chat[nowChatIndex].text);
            nowChatIndex++;
        }
    }

    public void answer(string str)      // ���������� ���� ��.
    {
        foreach (test ttt in chapterSO[currentChapter].temp[currentStep].next)
        {
            if (ttt.text == str)
            {
                ttt.isDone = true;
                foreach (test ttttt in ttt.next)
                {
                    {
                        TextBox.Instance.InputText(false, ttttt.text);
                    }
                }
                    // ���� ���� �� ���� (so����?...)��
                    // �� bool�� Ȯ���ߴ��� Ȯ���ϰ� ���� �� Ȯ���ߴٸ� step++
            }
        }
        isChoice = false;

        // if ���� ������ �� ttttt�� ���� �� �ۿ� ���ٸ�
    }

    public void ChapterReset()
    {
        currentStep = 0;
    }
}
