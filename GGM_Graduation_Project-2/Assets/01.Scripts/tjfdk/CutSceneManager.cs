using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CutSceneManager : MonoBehaviour
{
    [Header("Object")]
    [SerializeField] private Image screen;
    [SerializeField] private Text text;
    //[SerializeField] private TextMeshProUGUI text;

    [Header("Current Index")]
    [SerializeField] private CutSceneSO currentCutScene;
    [SerializeField] private int currentCutNum;
    [SerializeField] private int currentTextNum;

    [Header("Data")]
    [SerializeField] private List<CutSceneSO> cutSceneChapters = new List<CutSceneSO>();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            PlayChapter("test");
        }
    }

    public void PlayChapter(string _chapterName)
    {
        // ���� é�� ã���ֱ�
        foreach (CutSceneSO chapter in cutSceneChapters)
        {
            if (chapter.chapterName == _chapterName)
                currentCutScene = chapter;
        }

        // ���� é�� SO �ʱ�ȭ
        foreach (CutSceneDialoges chapter in currentCutScene.cutScenes)
        {
            foreach (CutSceneText text in chapter.texts)
                text.isEnd = false;
        }

        // é�� ������ �ε��� �ʱ�ȭ
        currentCutNum = 0;
        currentTextNum = 0;

        CutSetting();
    }

    // �׸� ����
    private void CutSetting()
    {
        // �̹��� ����
        //
        // ��� �Է� �Լ� ȣ��
        Texting(currentCutScene.cutScenes[currentCutNum].texts[currentTextNum]);
    }

    // �������� �ѱ�� �� �� (��ư�� �����ص�)
    public void NextCut()
    {
        if (currentCutScene != null)
        {
            CutSceneText currentText = currentCutScene.cutScenes[currentCutNum].texts[currentTextNum];

            // ���� �������� �ƾ��� ��簡 �� �ۼ����� �ʾҴٸ�
            if (currentText.isEnd == false)
            {
                // ��Ʈ�� ���� ����
                text.DOKill();
                // ��� �Է�
                text.text = currentText.text;
                // ��� �Է� �Ϸ�
                currentText.isEnd = true;
            }
            // ���� �������� �ƾ��� ��簡 �� �ۼ� �ƴٸ�
            else
            {
                // ��� �ε��� ����
                currentTextNum++;
                // ���� ���� ��� ��縦 �����ߴٸ�
                if (currentTextNum >= currentCutScene.cutScenes[currentCutNum].texts[currentTextNum].text.Length - 1)
                {
                    // �׽�Ʈ�� �����
                    Debug.Log("��!!!");
                }
                // ���� ���� ��簡 �����ִٸ�
                else
                {
                    // ��� ����
                    currentText = currentCutScene.cutScenes[currentCutNum].texts[currentTextNum];
                    // ��� �Է� �Լ� ȣ��
                    Texting(currentText);
                }
            }
        }
    }

    private void Texting(CutSceneText temp)
    {
        // ���� �ؽ�Ʈ ����
        text.text = "";
        //��Ʈ������ �ؽ�Ʈ �ۼ�
        text.DOText(temp.text, 3f).OnComplete(() =>
        {
            temp.isEnd = true;
        });
    }
}
