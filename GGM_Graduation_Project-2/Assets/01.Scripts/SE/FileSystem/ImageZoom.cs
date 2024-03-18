using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ImageZoom : MonoBehaviour, IScrollHandler
{
    public TMP_Text ZoomPersent;
    public float zoomSpeed = 0.05f;
    public float minScale = 0.7f;      // �ּҷ� ������ ���� 70% ��.
    public float maxScale = 10.0f;         // �ִ�� ������ ���� 1000% ������.

    private Vector3 originScale;       // �������� ������, min ������.

    private void Start()
    {
        originScale = transform.localScale;
    }

    public void OnScroll(PointerEventData eventData)
    {
        Vector3 move = Vector3.one * (eventData.scrollDelta.y + zoomSpeed);     // ���͸� ���� �������.
        Vector3 newScale = transform.localScale + move;

        newScale = Vector3.Max(originScale * minScale, newScale);      // �� �� ū �͵��� ��ȯ
        newScale = Vector3.Min(originScale * maxScale, newScale);            // �� �� ���� �͵��� ��ȯ

        ScaleApply(ClampScale(newScale));
    }

    private Vector3 ClampScale(Vector3 newScale)
    {
        newScale = Vector3.Max(originScale * minScale, newScale);      // �� �� ū �͵��� ��ȯ
        newScale = Vector3.Min(originScale * maxScale, newScale);            // �� �� ���� �͵��� ��ȯ

        return newScale;
    }

    public void IncreaseBtn()
    {
        transform.localScale += Vector3.one * 0.1f;
        ScaleApply(ClampScale(transform.localScale));
    }

    public void DecreaseBtn()
    {
        transform.localScale -= Vector3.one * 0.1f;
        ScaleApply(ClampScale(transform.localScale));
    }

    private void ScaleApply(Vector3 newScale)
    {
        transform.localScale = newScale;
        ScalePersent();
    }

    private void ScalePersent()
    {
        float currentScale = transform.localScale.x / originScale.x;        // ���� �������� ���� �����Ϸ� ���� ����
        float persent = MathF.Round(currentScale * 100);
        ZoomPersent.text = $"{persent}%";
    }

}
