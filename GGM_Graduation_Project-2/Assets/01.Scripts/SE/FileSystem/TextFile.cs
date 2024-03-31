using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TextFile : MonoBehaviour, IPointerClickHandler
{
    [TextArea(3, 10)]
    public string text;        // ������ �ؽ�Ʈ
    private string fileName;

    public bool assistantUse = false;
    public int index;
    private bool used = false;

    private void Start()
    {
        fileName = GetComponentInChildren<TMP_Text>().text;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2)
        {
            FileManager.instance.OpenTextFile(text, fileName);
            if (assistantUse && !used)
            {
                ChattingManager.Instance.StartChatting(index);
                used = true;
            }
        }
    }
}
