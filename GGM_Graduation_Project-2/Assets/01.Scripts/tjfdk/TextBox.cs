using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class TextBox : MonoBehaviour
{
    public static TextBox Instance;

    [Header("Object")]
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] RectTransform chatBoxParent;       // ������ �� �ڽ��� �Ѹ�. �׷��̶� ������ ���� ������Ʈ �� ����.

    [Header("Prefabs")]
    [SerializeField] Transform currentSpeech;       // ���� �ֱ��� ��ȭ
    [SerializeField] GameObject speechBalloon;      // ���ϴ� ��ǳ��
    [SerializeField] GameObject choiceBalloon;          // ���� ��ǳ�� (��ư�޸�)
    [SerializeField] GameObject myChatBox;          // �� ê�ùڽ�
    [SerializeField] GameObject otherChatBox;           // ������ ���ùڽ�

    [Header("isBool")]
    [SerializeField] bool isCurrentUser;

    int myChatCount = 0;

    private void Awake()
    {
        Instance = this;
    }

    public void InputText(bool user, string msg)        // user�� true �ϸ� �÷��̾ ���ϴ� ����.
    {
        // �ؽ�Ʈ �����ֱ� ��� �����
        // �������� �����ְ� �߸��� �κ��� �ε����� ���� ����� ���� ��Ƽ� �ű⼭ �ٳ����� �߰����ش�.
        // �׷��� �ε������� ū�� ��... 5�̻��� �Ѵ� ���̸� �� �ڿ� �Ϳ��� �ٳ����� ���ش�.
        Debug.Log(msg);
        string[] 


        if (currentSpeech == null || isCurrentUser != user)
        {
            if (user)
            {
                GameObject temp = Instantiate(myChatBox);
                temp.transform.SetParent(chatBoxParent);
                currentSpeech = temp.transform;
                isCurrentUser = true;
            }
            else
            {
                GameObject temp = Instantiate(otherChatBox);
                temp.transform.SetParent(chatBoxParent);
                currentSpeech = temp.transform;
                isCurrentUser = false;
            }
        }

        GameObject speech = null;
        if (user)
        {
            speech = Instantiate(choiceBalloon);
            speech.name += "-" + myChatCount;
            myChatCount++;
            speech.GetComponent<Button>().onClick.AddListener(() => ChoiceQuestion());
            speech.GetComponentInChildren<TextMeshProUGUI>().text = msg;
        }
        else
        {
            speech = Instantiate(speechBalloon);
            speech.GetComponentInChildren<TextMeshProUGUI>().text = msg;
        }
        speech.transform.SetParent(currentSpeech);
        LineAlignment();
    }

    public void ChoiceQuestion()
    {
        GameObject currentSelectedButton = EventSystem.current.currentSelectedGameObject;

        for (int i = 0; i < currentSpeech.childCount; ++i)
        {
            if (currentSpeech.GetChild(i).name != currentSelectedButton.name)
            {
                currentSelectedButton.GetComponent<Button>().interactable = false;
                Destroy(currentSpeech.GetChild(i).gameObject);
            }
        }

        StartCoroutine(LineRefresh());

        ChattingManager.Instance.answer(currentSelectedButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text);
    }

    private IEnumerator LineRefresh()
    {
        yield return null;
        LayoutRebuilder.ForceRebuildLayoutImmediate(chatBoxParent);
    }

    private void LineAlignment()
    {
        StartCoroutine(LineRefresh());
        StartCoroutine(ScrollRectDown());
    }

    private IEnumerator ScrollRectDown()
    {
        yield return null;
        scrollRect.verticalNormalizedPosition = 0;
    }
}
