using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

//[Serializable]
//public enum FileType
//{
//    Image = 0,
//    Data
//}

public class TextBox : MonoBehaviour
{
    public static TextBox Instance;

    public int cutTextSize = 20;

    [Header("Object")]
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] RectTransform chatBoxParent;       // ������ �� �ڽ��� �Ѹ�. �׷��̶� ������ ���� ������Ʈ �� ����.

    [Header("Prefabs")]
    [SerializeField] Transform currentSpeech;       // ���� �ֱ��� ��ȭ
    [SerializeField] GameObject speechBalloon_left;      // ���ϴ� ��ǳ��
    [SerializeField] GameObject speechBalloon_right;      // ���ϴ� ��ǳ��

    [SerializeField] GameObject imageBackground; // �̹��� ������ �װ� sprite�� �����ϸ鼭 ���
    [SerializeField] GameObject dataBackground; // �굵 ���������� ����

    [SerializeField] GameObject choiceBalloon;          // ���� ��ǳ��(��ư�޸�)
    [SerializeField] GameObject myChatBox;          // �� ���ùڽ�
    [SerializeField] GameObject otherChatBox;           // ������ ���ùڽ�

    public Sprite sprite;
    public string msg;

    [Header("isBool")]
    [SerializeField] bool isCurrentUser;

    int myChatCount = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            InputFile(true, sprite, "Image");
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Text txt = gameObject.AddComponent<Text>();
            txt.text = msg;
            InputFile(true, txt, "Data");
        }
    }

    public void InputText(bool user, string msg, bool ask = true)       // user�� true �ϸ� �÷��̾ ���ϴ� ����.
    {
        CutText(ref msg);

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
                speech = Instantiate(speechBalloon_right);
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
            speech = Instantiate(speechBalloon_left);
            speech.GetComponentInChildren<TextMeshProUGUI>().text = msg;
        }
        HiddingText(speech);
        AssistantChatListAdd(speech);       // ������ ��ȭ�� ����Ʈ�� �߰�
        speech.transform.SetParent(currentSpeech);
        StartCoroutine(OpenText(speech, user, ask));
        LineAlignment();
    }

    public void InputFile(bool user, UnityEngine.Object file, string _type)       // user�� true �ϸ� �÷��̾ ���ϴ� ����.
    {
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

        GameObject data = null;

        if (_type == "Image")
        {
            data = Instantiate(imageBackground);
            SpriteRenderer sprite = file.GetComponent<SpriteRenderer>();
            data.GetComponent<Image>().sprite = sprite.sprite;
            Vector3 size = sprite.bounds.size;
            data.GetComponent<Transform>().localScale = size;
        }
        else if (_type == "Data")
        {
            data = Instantiate(dataBackground);
            data.GetComponent<TextMeshProUGUI>().text = file.GetComponentInChildren<Text>().text;
        }

        data.transform.SetParent(currentSpeech);
        LineAlignment();
    }

    private void CutText(ref string msg)
    {
        // �ؽ�Ʈ �����ֱ� ��� �����
        // �������� �����ְ� �߸��� �κ��� �ε����� ���� ����� ���� ��Ƽ� �ű⼭ �ٳ����� �߰����ش�.
        // �׷��� �ε������� ū�� ��... 5�̻��� �Ѵ� ���̸� �ڸ��⸦ �ڿ� �Ϳ��� �߶� �ٳ����� ���ش�.

        int cutIndex = cutTextSize;
        int endIndex = 0;

        while (msg.Length > cutIndex)
        {
            if (msg[cutIndex] == ' ')     // �ڸ����� ���� ������ ������
            {
                // 0 ���� �ڸ����� ������ �ڸ��� �ڸ����� ������ ������ �߶��ش�.
                msg = $"{msg.Substring(0, cutIndex)}\n{msg.Substring(cutIndex + 1)}";
                endIndex = cutIndex;
            }
            else
            {
                int space = msg.IndexOf(" ", cutIndex);       // �ڸ����� �� �ڿ� ù��°�� �ִ� ������ ã���ش�.
                if (space == -1) space = 300;         // ������ �� ã�����ٸ�

                int space2 = msg.LastIndexOf(" ", cutIndex);     // startIndex ���� 20���� �ִ� ���ڿ����� ���� �������� �ִ� ������ ã���ش�.
                
                if (space >= cutIndex + 5)
                {
                    endIndex = space2;
                }
                else
                {
                    if (space < space2)
                    {
                        endIndex = space;
                    }
                    else endIndex = space2;
                }

                msg = $"{msg.Substring(0, endIndex)}\n{msg.Substring(endIndex + 1)}";

            }
            cutIndex = endIndex + cutTextSize;
        }

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
        currentSelectedButton.GetComponent<Button>().interactable = false;

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

    public void CurrentSpeechColorChange()      // ���� ���� ���ϰ� �ִ� ģ���� ���� ��� ����
    {
        for (int i = 0; i < currentSpeech.childCount; ++i)
        {
            currentSpeech.GetChild(i).GetComponent<Image>().color = Color.white;
        }
    }

    private void LineAlignment()
    {
        StartCoroutine(LineRefresh());
        StartCoroutine(ScrollRectDown());
    }

    private IEnumerator LineRefresh()
    {
        yield return new WaitForSeconds(0.1f);
        LayoutRebuilder.ForceRebuildLayoutImmediate(chatBoxParent);
    }

    private IEnumerator ScrollRectDown()
    {
        yield return new WaitForSeconds(0.1f);
        scrollRect.normalizedPosition = new Vector2(0f, 0);
    }

    private void HiddingText(GameObject _temp)
    {
        _temp.GetComponent<Image>().color = Color.clear;
        _temp.GetComponentInChildren<TextMeshProUGUI>().color = Color.clear;
    }

    public IEnumerator OpenText(GameObject _temp, bool _isUser, bool _isAsk)
    {
        yield return new WaitForSeconds(0.25f);
        if (_isUser && _isAsk)
        {
            Color color;
            UnityEngine.ColorUtility.TryParseHtmlString("#CCFFB8", out color);
            _temp.GetComponent<Image>().color = color;
        }
        else
            _temp.GetComponent<Image>().color = Color.white;
        _temp.GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
    }
}
