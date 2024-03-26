using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ImageFile : MonoBehaviour, IPointerClickHandler
{
    public Sprite image;        // ������ �̹��� ��������Ʈ
    public Vector2 showScale = new Vector2(500, 500);       // ������ ũ��
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
            FileManager.instance.OpenImageFile(image, showScale, fileName);
            if (assistantUse && !used)
            {
                ChattingManager.Instance.StartChatting(index);
                used = true;
            }
        }
    }
}
