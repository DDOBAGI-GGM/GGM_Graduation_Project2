using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using UnityEngine.SceneManagement;

public enum EPanel
{
    MAIN,
    CUTSCENE,
    RELATIONSHIP,
    SETTING,
    QUIT
}

public class UIReader_Main : MonoBehaviour
{
    static public UIReader_Main Instance;

    // main
    public UIDocument document;
    public VisualElement root;

    // UXML
    // Button
    public Button chattingButton;
    public Button connectionButton;
    public Button settingButton;
    public Button NextChatBtn;

    // Panel
    private VisualElement mainPanel;
    private VisualElement filePanel;
    private VisualElement cutScenePanel;
    private VisualElement gamePanel;
    public VisualElement RelationshipPanel;
    public GameObject relationshipCanvas;
    private VisualElement settingPanel;
    private VisualElement quitPanel;

    public bool isCutSceneOpen;
    public bool isRelationshipOpen;

    public Tween currentTextTween;
    public string currentText;

    private Vector2 originalPosition;
    public Label currentTextUi;

    public Tween currentUiTween;

    public float MinWidth;
    public float MinHeight;
    public float MaxWidth;
    public float MaxHeight;

    public int hp = 3;
    public Color sliderColor;



    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        relationshipCanvas.SetActive(false);
    }

    private void OnEnable()
    {
        root = document.rootVisualElement;
        GameLoad();
    }

    private void GameLoad()
    {
        // Panel
        mainPanel = root.Q<VisualElement>("MainGame");      // 컷 씬에서 사용함.
        filePanel = root.Q<VisualElement>("FileSystem");
        cutScenePanel = root.Q<VisualElement>("CutScene");
        gamePanel = root.Q<VisualElement>("MainSystem");
        RelationshipPanel = root.Q<VisualElement>("RelationshipSystem");
        settingPanel = root.Q<VisualElement>("Setting");
        quitPanel = root.Q<VisualElement>("Quit");

        chattingButton = root.Q<Button>("ChattingBtn");
        chattingButton.clicked += () => { OpenPanel(EPanel.MAIN); };

        connectionButton = root.Q<Button>("RelationshipBtn");
        connectionButton.clicked += () => { OpenPanel(EPanel.RELATIONSHIP); };

        settingButton = root.Q<Button>("SoundSettingBtn");
        settingButton.clicked += () => { OpenPanel(EPanel.SETTING); };

        NextChatBtn = root.Q<Button>("NextChatBtn");

        root.Q<Button>("QuitBtn").clicked += () => { OpenPanel(EPanel.QUIT); };
    }


    public void OpenPanel(EPanel panelType)
    {
        if (panelType == EPanel.SETTING)
        {
            settingPanel.style.display = DisplayStyle.Flex;
            return;
        }
        else if (panelType == EPanel.QUIT)
        {
            quitPanel.style.display = DisplayStyle.Flex;

            quitPanel.Q<Button>("QuitCloseBtn1").clicked += () => { quitPanel.style.display = DisplayStyle.None; };
            quitPanel.Q<Button>("QuitCloseBtn2").clicked += () => { quitPanel.style.display = DisplayStyle.None; };
            quitPanel.Q<Button>("RealQuitBtn").clicked += () => { SceneManager.LoadScene("Intro"); };

            return;
        }

        isRelationshipOpen = false;
        gamePanel.style.display = DisplayStyle.None;
        RelationshipPanel.style.display = DisplayStyle.None;
        settingPanel.style.display = DisplayStyle.None;

        switch (panelType)
        {
            case EPanel.MAIN:
                //filePanel.style.display = DisplayStyle.Flex;
                gamePanel.style.display = DisplayStyle.Flex;
                relationshipCanvas.SetActive(false);
                StartCoroutine(GameManager.Instance.chatSystem.EndToScroll(0.05f));
                break;
            case EPanel.RELATIONSHIP:
                isRelationshipOpen = true;
                //filePanel.style.display = DisplayStyle.None;
                RelationshipPanel.style.display = DisplayStyle.Flex;
                relationshipCanvas.SetActive(true);
                break;
        }
    }

    public VisualElement RemoveContainer(VisualElement visualElement)
    {
        return visualElement[0];
    }

    public void OpenCutScene()
    {
        isCutSceneOpen = !isCutSceneOpen;

        if (isCutSceneOpen)
        {
            cutScenePanel.style.display = DisplayStyle.Flex;
            mainPanel.style.display= DisplayStyle.None;

            GameManager.Instance.chatHumanManager.IsChat(false);
        }
        else
        {
            //GameManager.Instance.cutSceneSystem.BarAnim(130.5f, 0f, 1f);

            cutScenePanel.style.display = DisplayStyle.None;
            mainPanel.style.display = DisplayStyle.Flex;

            GameManager.Instance.chatHumanManager.IsChat(true);
        }
    }

    public void DoText(Label ui, string text, float during, bool isErase, Action action,
        string soundName)
    {
        int currentTextLength = 0;
        int previousTextLength = -1;

        currentTextUi = ui;
        originalPosition = currentTextUi.transform.position;

        currentText = text;

        ui.text = "";

        currentTextTween = DOTween.To(() => currentTextLength, x => currentTextLength = x, text.Length, during)
            .SetEase(Ease.Linear)
            .OnPlay(() => 
            { 
                if (soundName != "")
                    SoundManager.Instance.PlaySFX(soundName);
            })
            .OnUpdate(() =>
            {
                if (currentTextLength != previousTextLength)
                {
                    ui.text += text[currentTextLength];
                    previousTextLength = currentTextLength;
                }
            })
            .OnComplete(() =>
            {
                action();

                if (currentUiTween != null)
                    currentUiTween.Kill();

                SoundManager.Instance.StopSFX();

                if (isErase)
                    ui.text = "";

                currentTextLength = 0;
            });
    }

    public void EndText()
    {
        if (currentTextTween != null)
        {
            currentTextTween.Kill();
            currentUiTween.Kill();
            currentTextUi.text = currentText;
            currentTextUi.transform.position = originalPosition;
            GameManager.Instance.cutSceneManager.currentTextNum++;
        }
    }

    public void ReSizeImage(VisualElement visualElement, Sprite sprite)
    {
        float originalWidth = sprite.rect.width;
        float originalHeight = sprite.rect.height;

        Vector2 adjustedSize = ChangeSize(originalWidth, originalHeight);

        visualElement.style.width = adjustedSize.x;
        visualElement.style.height = adjustedSize.y;

        visualElement.style.backgroundImage = new StyleBackground(sprite);
    }

    private Vector2 ChangeSize(float originalWidth, float originalHeight)
    {
        float aspectRatio = originalWidth / originalHeight;
        float adjustedWidth = originalWidth;
        float adjustedHeight = originalHeight;

        if (originalWidth > MaxWidth || originalHeight > MaxHeight)     // 넓이, 높이 중 하나라도 맥스보다 크면
        {
            if (aspectRatio > 1)        // 넓이가 더 길면
            {
                adjustedWidth = MaxWidth;       // 넓이를 맥스로 바꾸고
                adjustedHeight = MaxWidth * (originalHeight / originalWidth);
            }
            else
            {
                adjustedHeight = MaxHeight;
                adjustedWidth = MaxHeight * aspectRatio;
            }
            return new Vector2(adjustedWidth, adjustedHeight);
        }

        if (adjustedWidth < MinWidth || adjustedHeight < MinHeight)
        {
            if (aspectRatio > 1)
            {
                adjustedWidth = MinWidth;
                adjustedHeight = MinWidth / aspectRatio;
            }
            else
            {
                adjustedHeight = MinHeight;
                adjustedWidth = MinHeight * aspectRatio;
            }
        }

        return new Vector2(adjustedWidth, adjustedHeight);
    }

    public VisualElement GetSelectHumanSystem()
    {
        return root.Q<VisualElement>("SelectHumanSystem");
    }

    public VisualElement GetTopBar()
    {
        return root.Q<VisualElement>("TopBar");
    }

    public void MinusHP()
    {
        GameManager.Instance.fileSystem.ui_hpGround
            .Q<VisualElement>("HP_" + hp).style.display = DisplayStyle.None;

        hp -= 1;

        SoundManager.Instance.PlaySFX("minusHP");

        if (hp <= 0)
        {
            GameManager.Instance.chatHumanManager.IsChat(false);
            GameManager.Instance.cutSceneSystem.PlayCutScene("BadEnd");
        }
    }

    public void PlusHP()
    {
        Debug.Log("hp+");
        if (hp < 10)
        {
            hp += 1;

            GameManager.Instance.fileSystem.ui_hpGround
                .Q<VisualElement>("HP_" + hp).style.display = DisplayStyle.Flex;
        }
    }

    public void RemoveSlider(VisualElement scrollView)
    {
        // hidding scrollview slider
        var verticalScroller = scrollView.Q<Scroller>(className: "unity-scroll-view__vertical-scroller");

        if (verticalScroller != null)
        {
            var lowButton = verticalScroller.Q<VisualElement>(className: "unity-scroller__low-button");
            var highButton = verticalScroller.Q<VisualElement>(className: "unity-scroller__high-button");

            var sliderBG = verticalScroller.Q<VisualElement>(className: "unity-base-slider__tracker");
            var sliderOL = verticalScroller.Q<VisualElement>(className: "unity-base-slider__dragger-border");
            var slider = verticalScroller.Q<VisualElement>(className: "unity-base-slider__dragger");

            if (lowButton != null)
                lowButton.style.display = DisplayStyle.None;

            if (highButton != null)
                highButton.style.display = DisplayStyle.None;

            if (sliderBG != null)
                sliderBG.style.display = DisplayStyle.None;

            if (sliderOL != null)
                sliderOL.style.display = DisplayStyle.None;

            if (slider != null)
                slider.style.backgroundColor = sliderColor;
        }
    }
}
