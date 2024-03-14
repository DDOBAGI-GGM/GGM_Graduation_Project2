using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ImageFile : MonoBehaviour, IPointerClickHandler
{
    public Sprite image;        // ������ �̹��� ��������Ʈ
    public Vector3 showScale;       // ������ ũ��

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2)
        {
            FileManager.instance.OpenImageFile(image, showScale);
        }
    }
}
