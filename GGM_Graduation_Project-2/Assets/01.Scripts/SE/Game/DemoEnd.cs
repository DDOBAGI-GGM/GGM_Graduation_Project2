using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DemoEnd : MonoBehaviour
{
    public TMP_Text text;
    public Image image;

    public string endText = "���... ���� �б��� �ڻ� ����� �̹��� ó���� �ƴϿ���.";

    private IEnumerator Start()
    {
        for (int i = 0; i < 6; i++)
        {
            text.text += endText[i];
            yield return new WaitForSeconds(0.25f);
        }

        yield return new WaitForSeconds(1f);

        for (int i = 6; i < endText.Length; i++)
        {
            text.text += endText[i];
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(2.5f);
        image.DOFade(1, 1.5f);
        yield return new WaitForSeconds(2.5f);
        Application.Quit();
    }
}
