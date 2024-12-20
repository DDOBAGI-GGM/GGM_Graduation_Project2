using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ChapterManager : MonoBehaviour
{
    private float scaleNormal = 1f;
    private float scaleReduced = 0.9f;
    private float chapterSpacing = 600f;
    
    [SerializeField] private List<RectTransform> chapterList;

    [SerializeField] private List<Button> chapterButtons;

    [SerializeField] private List<TextMeshProUGUI> chapterTexts;

    [SerializeField] private RectTransform chapter;

    private int currentChapterIndex = 0; 

    private void Start()
    {
        UpdateChapterScales();
        UpdateUIState();
    }

    public void MoveLeft()
    {
        if (currentChapterIndex > 0)
        {
            currentChapterIndex--;
            UpdateChapterPosition();
        }
    }

    public void MoveRight()
    {
        if (currentChapterIndex < chapterList.Count - 1)
        {
            currentChapterIndex++;
            UpdateChapterPosition();
        }
    }

    private void UpdateChapterPosition()
    {
        chapter.DOAnchorPosX(currentChapterIndex * -chapterSpacing, 0.5f).SetEase(Ease.OutCubic);

        UpdateChapterScales();
        UpdateUIState();
    }

    private void UpdateChapterScales()
    {
        for (int i = 0; i < chapterList.Count; i++)
        {
            float distanceToCenter = Mathf.Abs(i - currentChapterIndex);

            chapterList[i].DOScale(distanceToCenter == 0 ? scaleNormal : scaleReduced, 0.5f).SetEase(Ease.OutCubic);
        }
    }

    private void UpdateUIState()
    {
        for (int i = 0; i < chapterList.Count; i++)
        {
            bool isCenter = (i == currentChapterIndex);

            chapterButtons[i].interactable = isCenter;

            SetAlpha(chapterButtons[i].GetComponent<Image>(), isCenter ? 1f : 0.5f);
            SetAlpha(chapterTexts[i], isCenter ? 1f : 0.5f);
        }
    }

    private void SetAlpha(Graphic graphic, float alpha)
    {
        if (graphic != null)
        {
            Color color = graphic.color;
            color.a = alpha;
            graphic.color = color;
        }
    }
}
