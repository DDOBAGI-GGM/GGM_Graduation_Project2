using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TextFile : MonoBehaviour, IPointerClickHandler
{
    public string text;        // ������ �ؽ�Ʈ
    private string fileName;

    private void Start()
    {
        fileName = GetComponentInChildren<TMP_Text>().text;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2)
        {
            FileManager.instance.OpenTextFile(text, fileName);
        }
    }
}
