using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EvidenceFile : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // ������� �̹���, �ؽ�Ʈ ���Ͽ� ������Ʈ�� �߰��� ����

    [Header("Drag And Drop")]
    [SerializeField] private Transform canvas;
    [SerializeField] private Transform parent;
    [SerializeField] private Vector3 previousPosition;
    [SerializeField] private RectTransform rect;

    //public GameObject temp;

    [Header("Evidence")]
    [SerializeField] private bool isUseable;
    [SerializeField] private Sprite sprite;
    [SerializeField] private string msg;
    [SerializeField] private string type = "Image";
    public bool IsUseable => isUseable;

    private void Awake()
    {
        canvas = FindObjectOfType<Canvas>().transform;
        rect = GetComponent<RectTransform>();
        previousPosition = rect.transform.position;
        parent = rect.parent;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
            this.gameObject.SetActive(true);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        rect.SetParent(canvas);
        //rect.SetAsFirstSibling();
        rect.SetSiblingIndex(2);
    }

    public void OnDrag(PointerEventData eventData)
    {
        rect.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GameObject temp = new GameObject();
        if (type == "Image")
            temp.AddComponent<Image>().sprite = sprite;
        else
            temp.AddComponent<TextMesh>().text = msg;

        TextBox.Instance.InputFile(true, temp, type);

        rect.SetParent(parent);
        rect.transform.position = previousPosition;
    }
}
