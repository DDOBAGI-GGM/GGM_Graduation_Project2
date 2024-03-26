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
    [SerializeField] RectTransform chatBoxParent;       // 쳇팅이 들어갈 박스의 뿌리. 그룹이랑 싸이즈 필터 컴포넌트 들어가 있음.

    [Header("Prefabs")]
    [SerializeField] Transform currentSpeech;       // 가장 최근의 대화
    [SerializeField] GameObject speechBalloon;      // 말하는 말풍선
    [SerializeField] GameObject choiceBalloon;          // 고르는 말풍선 (버튼달린)
    [SerializeField] GameObject myChatBox;          // 내 챗팅박스
    [SerializeField] GameObject otherChatBox;           // 조수의 쳇팅박스

    [Header("isBool")]
    [SerializeField] bool isCurrentUser;

    int myChatCount = 0;

    private void Awake()
    {
        Instance = this;
    }

    public void InputText(bool user, string msg, bool ask = true)        // user가 true 일면 플레이어가 말하는 것임.
    {
        // 텍스트 내려주기 기능 만들기
        // 공백으로 나눠주고 잘리는 부분의 인덱스와 가장 가까운 것을 잡아서 거기서 줄내림을 추가해준다.
        // 그런데 인덱스보다 큰데 한... 5이상이 넘는 줄이면 그 뒤에 것에서 줄내림을 해준다.

        if (msg.Length > cutTextSize)
        {
            if (msg[cutTextSize] == ' ')     // 자르려는 곳에 공백이 있으면
            {
                msg = $"{msg.Substring(0, cutTextSize)}\n{msg.Substring(cutTextSize + 1, (msg.Length - cutTextSize) - 1)}";
            }
            else
            {
                int space = msg.IndexOf(" ", cutTextSize);       // 20 뒤에 첫번째로 있는 공백을 찾아준다.
                if (space == -1) space = 50;        // 공백이 안 찾아진다면
                int space2 = msg.Substring(0, cutTextSize).LastIndexOf(" ", cutTextSize);     // 0 부터 20까지 있는 문자열에서 가장 마지막에 있는 공백을 찾아준다.
                int endIndex = space > space2 ? space2 : space;     // 둘 중 작은 것 넣어주기
                msg = $"{msg.Substring(0, endIndex)}\n{msg.Substring(endIndex + 1, (msg.Length - endIndex) - 1)}";
            }
        }
        

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
                currentSelectedButton.GetComponent<Image>().color = Color.white;
                Destroy(currentSpeech.GetChild(i).gameObject);
            }       // 나머지 친구들 다 지워주기
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
