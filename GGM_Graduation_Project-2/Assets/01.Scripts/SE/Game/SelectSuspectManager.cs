using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectSuspectManager : Singleton<SelectSuspectManager>
{
    public Image fadePanel;
    private string suspectName;
    private TMP_Text Text;

    private WaitForSeconds delay = new WaitForSeconds(1.5f);

    public void Select(string name)
    {
        suspectName = name;
        StopAllCoroutines();
        StartCoroutine(End());
    }

    private IEnumerator End()
    {
        fadePanel.gameObject.SetActive(true);
        fadePanel.DOFade(1, 0.5f);
        yield return delay;
        SceneManager.LoadScene("End");
        yield return delay;

        Text = FindObjectOfType<TMP_Text>();

        if (suspectName == "Ȳ�ؿ�")
        {
            Text.text = "����� �����Ͽ����ϴ�.\n������ Ȳ�ؿ� �̿����ϴ�.";
        }
        else
        {
            Text.text = "����� �ذ�Ǿ����ϴ�.\n������ Ȳ�ؿ� �̿����ϴ�.";
        }

        fadePanel.color = new Color(0, 0, 0, 0);
        Text.DOFade(1, 1.5f);
        yield return delay;
        fadePanel.DOFade(1, 0.5f);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // ���ø����̼� ����
#endif
    }
}
