using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragAndDrop : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private Canvas canvas;
    private RectTransform rectTransform;
    private Outline outline;

    [SerializeField] private bool canDrag = false;

    private Vector2 dragStartPosition;
    private Vector2 dragEndPosition;

    [SerializeField] private GameObject linePrefab; // ���� �׸��� ���� ������
    [SerializeField] private GameObject currentLine; // ���� �׷����� �ִ� ��

    private void Awake()
    {
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        outline = GetComponent<Outline>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        dragEndPosition = Input.mousePosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canDrag)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
        else
        {
            Debug.Log(dragStartPosition +  " " + dragEndPosition);
            UpdateLine();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        canDrag = !canDrag;
        outline.enabled = !outline.IsActive();
        currentLine = null;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragStartPosition = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canDrag = false;
        outline.enabled = false;
    }

    private void UpdateLine()
    {
        if (currentLine == null)
            currentLine = Instantiate(linePrefab, canvas.transform);

        // ���� ������ �� ���� ���� �Ÿ� �� ���� ���
        float distance = Vector2.Distance(dragStartPosition, dragEndPosition);
        Vector2 direction = (dragEndPosition - dragStartPosition).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // ���� ��ġ�� ȸ���� ����
        currentLine.transform.position = (dragStartPosition + dragEndPosition) / 2f;
        currentLine.transform.rotation = Quaternion.Euler(0, 0, angle);
        currentLine.transform.localScale = new Vector3(distance, 1f, 1f);
    }
}
