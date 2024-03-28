using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class TextBox : MonoBehaviour
{
    public static TextBox Instance;

    public int cutTextSize = 20;

    [Header("Object")]
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] RectTransform chatBoxParent;       // ������ �� �ڽ��� �Ѹ�. �׷��̶� ������ ���� ������Ʈ �� ����.

    [Header("Prefabs")]
    [SerializeField] Transform currentSpeech;       // ���� �ֱ��� ��ȭ
    [SerializeField] GameObject speechBalloon;      // ���ϴ� ��ǳ��
    [SerializeField] GameObject choiceBalloon;          // ���� ��ǳ��(��ư�޸�)
    [SerializeField] GameObject myChatBox;          // �� ���ùڽ�
    [SerializeField] GameObject otherChatBox;           // ������ ���ùڽ�

    [Header("isBool")]
    [SerializeField] bool isCurrentUser;

    int myChatCount = 0;

    private void Awake()
    {
        Instance = this;
    }

    public void InputText(bool user, string msg, bool ask = true)       // user�� true �ϸ� �÷��̾ ���ϴ� ����.
    {
        // �ؽ�Ʈ �����ֱ� ��� �����
        // �������� �����ְ� �߸��� �κ��� �ε����� ���� ����� ���� ��Ƽ� �ű⼭ �ٳ����� �߰����ش�.
        // �׷��� �ε������� ū�� ��... 5�̻��� �Ѵ� ���̸� �� �ڿ� �Ϳ��� �ٳ����� ���ش�.

        if (msg.Length > cutTextSize)
        {
            if (msg[cutTextSize] == ' ')     // �ڸ����� ���� ������ ������
            {
                msg = $"{msg.Substring(0, cutTextSize)}\n{msg.Substring(cutTextSize + 1, (msg.Length - cutTextSize) - 1)}";
            }
            else
            {
                int space = msg.IndexOf(" ", cutTextSize);       // 20 �ڿ� ù��°�� �ִ� ������ ã���ش�.
                if (space == -1) space = 50;         // ������ �� ã�����ٸ�
                int space2 = msg.Substring(0, cutTextSize).LastIndexOf(" ", cutTextSize);     // 0 ���� 20���� �ִ� ���ڿ����� ���� �������� �ִ� ������ ã���ش�.
                int endIndex = space > space2 ? space2 : space;    // �� �� ���� �� �־��ֱ�
                msg = $"{msg.Substring(0, endIndex)}\n{msg.Substring(endIndex + 1, (msg.Length - endIndex) - 1)}";
            }
        }
        LineAlignment();

        if (currentSpeech == null || isCurrentUser != user)
        {
            GameObject temp = null;
            if (user)
            {
                temp = Instantiate(myChatBox);
                temp.transform.SetParent(chatBoxParent);
                currentSpeech = temp.transform;
                isCurrentUser = true;
            }
            else
            {
                temp = Instantiate(otherChatBox);
                temp.transform.SetParent(chatBoxParent);
                currentSpeech = temp.transform;
                isCurrentUser = false;
            }
            AssistantChatListAdd(temp);     // ���� ���� ��ȭ�� ����Ʈ�� �߰��ض�
            LineAlignment();
        }

        GameObject speech = null;
        if (user)
        {
            if (ask == false)
            {
                speech = Instantiate(speechBalloon);
            }
            else
            {
                speech = Instantiate(choiceBalloon);
                speech.GetComponent<Button>().onClick.AddListener(() => ChoiceQuestion());
            }
            speech.name += "-" + myChatCount;
            myChatCount++;
            speech.GetComponentInChildren<TextMeshProUGUI>().text = msg;
        }
        else
        {
            speech = Instantiate(speechBalloon);
            speech.GetComponentInChildren<TextMeshProUGUI>().text = msg;
        }
        AssistantChatListAdd(speech);       // ������ ��ȭ�� ����Ʈ�� �߰�
        speech.transform.SetParent(currentSpeech);
        LineAlignment();
    }

    private void AssistantChatListAdd(GameObject obj)
    {
        if (ChattingManager.Instance.chats[ChattingManager.Instance.nowLevel].whoSO.humanName == "����")
        {
            ChattingManager.Instance.assistantChatList.Add(obj);
        }
    }

    public void ChoiceQuestion()
    {
        GameObject currentSelectedButton = EventSystem.current.currentSelectedGameObject;

        for (int i = 0; i < currentSpeech.childCount; ++i)
        {
            if (currentSpeech.GetChild(i).name != currentSelectedButton.name)
            {
                currentSelectedButton.GetComponent<Button>().interactable = false;
                currentSelectedButton.GetComponent<Image>().color = Color.white;
                Destroy(currentSpeech.GetChild(i).gameObject);
            }     // ������ ģ���� �� �����ֱ�
        }

        StartCoroutine(LineRefresh());

        ChattingManager.Instance.answer(currentSelectedButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text);
    }

    public void CurrentSpeechColorChange()
    {
        for (int i = 0; i < currentSpeech.childCount; ++i)
        {
            currentSpeech.GetChild(i).GetComponent<Image>().color = Color.white;
        }
    }

    private IEnumerator LineRefresh()
    {
        yield return new WaitForSeconds(0.1f);
        LayoutRebuilder.ForceRebuildLayoutImmediate(chatBoxParent);
    }

    private void LineAlignment()
    {
        StartCoroutine(LineRefresh());
        StartCoroutine(ScrollRectDown());
    }

    private IEnumerator ScrollRectDown()
    {
        yield return new WaitForSeconds(0.1f);
        scrollRect.normalizedPosition = new Vector2(0f, 0);
    }
}
