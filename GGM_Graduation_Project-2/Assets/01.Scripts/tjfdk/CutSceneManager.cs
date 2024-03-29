using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Reflection;
using Unity.VisualScripting;

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

        currentCutNum = 0;
        currentTextNum = 0;

        imageSEt();
        //currentCutScene = cutScenes[currentChapterNum];
        //textNum = 0;
        //textMaxNum = currentCutScene.cutScenes[currentCutSceneNum].t
    }

    // �׸� ����
    private void imageSEt()
    {
        //screen.sprite = currentCutScene.cutScenes[currentCutNum].cut.sprite;
        //screen.sprite = currentCutScene.cutScenes[currentCutNum].cut.GetComponent<SpriteRenderer>().sprite;
        TTT(currentCutScene.cutScenes[currentCutNum].texts[currentTextNum]);
    }

    // �������� �ѱ�� �� ��
    public void NextCut()
    {
        if (currentCutScene != null)
        {

            // ���� �������� �ƾ��� ��簡 �� �ۼ����� �ʾҴٸ�
            test2 temp = currentCutScene.cutScenes[currentCutNum].texts[currentTextNum];
            if (temp.isEnd == false)
            {
                // �ڵ� �ۼ�
                text.DOKill();
                text.text = temp.text;
                temp.isEnd = true;
            }
            else
            {
                currentTextNum++;
                if (currentTextNum >= currentCutScene.cutScenes[currentCutNum].texts[currentTextNum].text.Length)
                {
                    Debug.Log("��!!!");
                    return;
                }
                temp = currentCutScene.cutScenes[currentCutNum].texts[currentTextNum];
                TTT(temp);
            }
        }
    }

    private void TTT(test2 temp)
    {
        text.text = "";
        //��Ʈ������ �ؽ�Ʈ �ۼ��ϱ�...
        text.DOText(temp.text, 0.15f * temp.text.Length).OnComplete(() =>
        {
            temp.isEnd = true;
        });
    }

    //public void ChangeCut()
    //{
    //    screen.sprite = currentCutScene.cutScenes[currentCutSceneNum].cut.sprite;
    //    text.text = currentCutScene.cutScenes[currentCutSceneNum].texts.;
    //}


    //private void StartCutScene()
    //{
    //    // ���� é���� �ƾ� ��Ͽ� �����Ͽ� ��� �ƾ��� ����Ѵ�
    //    foreach (CutSceneSO chapter in cutScenes[currentCutSceneNum].cutScenes)
    //    {

    //    }
    //}

    //private void EndCutScene()
    //{

    //}
}
