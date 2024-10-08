using UnityEngine;
using UnityEngine.UIElements;

public class Investigation : MonoBehaviour
{
    // root
    VisualElement root;

    // UXML
    VisualElement ui_imageGround;
    public VisualElement ui_panelGround;
    VisualElement ui_description;



    // Template
    [SerializeField] VisualTreeAsset ux_imageGround;
    [SerializeField] VisualTreeAsset ux_imageEvidence;
    [SerializeField] VisualTreeAsset ux_evidenceExplanation;
    [SerializeField] VisualTreeAsset ux_ImagePanel;
    [SerializeField] VisualTreeAsset ux_TextPanel;



    bool isImageOpen = true;

    private void Awake()
    {
    }

    private void OnEnable()
    {
        UXML_Load();
    }

    private void UXML_Load()
    {
        root = GameObject.Find("Game").GetComponent<UIDocument>().rootVisualElement;

        ui_imageGround = root.Q<VisualElement>("ImageFinding");
        ui_panelGround = root.Q<VisualElement>("PanelGround");      // 이걸 껴저 있을 때에도 해야 켜짐
    }

    public void OpenImage(VisualElement fileIcon, string fileName)
    {
        // find image
        ImageSO image = GameManager.Instance.imageManager.FindImage(fileName);

        // When image isn't null
        if (image != null)
        {
            if (isImageOpen)
            {
                isImageOpen = false;

                // fileSystem size change button off
                GameManager.Instance.fileSystem.ui_changeSizeButton.style.display = DisplayStyle.None;
                // fileSystem off bool value
                GameManager.Instance.fileSystem.isFileSystemOpen = false;
                // fileSystem size function
                GameManager.Instance.fileSystem.isFileSystemOpen = true;
                GameManager.Instance.fileSystem.OnOffFileSystem(0f);

                GameManager.Instance.selectHumanSystem.is_Open = true;
                GameManager.Instance.selectHumanSystem.OnOffThisSystem(0f);

                // png panel ground on
                ui_imageGround.style.display = DisplayStyle.Flex;

                GameManager.Instance.fileManager.FindFile(fileName).isRead = true;
                if (GameManager.Instance.fileManager.FindFile(fileName) == true)
                    fileIcon.Q<VisualElement>("NewIcon").style.display = DisplayStyle.None;

                // create uxml
                VisualElement imagePanel = UIReader_Main.Instance.RemoveContainer(ux_imageGround.Instantiate());
                imagePanel.Q<VisualElement>("ImageGround").style.backgroundImage = new StyleBackground(image.image);
                imagePanel.Q<Label>("Count").text = image.pngName.Count.ToString();

                imagePanel.Q<Button>("ImageGround").clicked += () =>
                { if (ui_description != null) ui_description.parent.Remove(ui_description); ui_description = null; };

                imagePanel.Q<Button>("ImageExitBtn").clicked += (() =>
                {
                    // fileSystem size change button on
                    GameManager.Instance.fileSystem.ui_changeSizeButton.style.display = DisplayStyle.Flex;
                    // image panel off
                    ui_imageGround.style.display = DisplayStyle.None;

                    // image panel clear
                    for (int i = ui_imageGround.childCount - 1; i >= 0; i--)
                        ui_imageGround.RemoveAt(i);

                    // image check action
                    FileSO fileT = GameManager.Instance.fileManager.FindFile(fileName);
                    if (fileT != null)
                        GameManager.Instance.fileManager.UnlockChat(fileT.name);

                    GameManager.Instance.chatHumanManager.IsChat(true);
                    isImageOpen = true;
                });

                foreach (string evid in image.pngName)
                {
                    PngSO png = GameManager.Instance.imageManager.FindPng(evid);
                    if (png != null)
                    {
                        if (evid == png.name)
                        {
                            VisualElement evidence = null;
                            if (png.importance)
                            {
                                evidence = UIReader_Main.Instance.RemoveContainer(ux_imageEvidence.Instantiate());
                                evidence.Q<Button>("EvidenceImage").style.backgroundImage = new StyleBackground(png.image);
                                evidence.Q<VisualElement>("Descripte").Q<Label>("EvidenceName").text = png.name;
                                evidence.Q<VisualElement>("Descripte").Q<Label>("Memo").text = png.memo;
                                evidence.Q<Button>("EvidenceImage").clicked += (() =>
                                {
                                    evidence.Q<VisualElement>("Descripte").style.display = DisplayStyle.Flex;
                                    evidence.Q<VisualElement>("Descripte").style.left = png.memoPos.x;
                                    evidence.Q<VisualElement>("Descripte").style.top = png.memoPos.y;

                                    TextSO text = GameManager.Instance.imageManager.FindText(evid);
                                    if (text != null)
                                    {
                                        GameManager.Instance.fileSystem.AddFile(FileType.TEXT, text.name,
                                            GameManager.Instance.fileManager.FindFile(text.name).fileParentName);
                                    }
                                    else
                                    {
                                        if (png.saveName != "" && png.saveName != null)
                                            GameManager.Instance.fileSystem.AddFile(FileType.TEXT, png.saveName,
                                            GameManager.Instance.fileManager.FindFile(png.saveName).fileParentName);
                                        else
                                            GameManager.Instance.fileSystem.AddFile(FileType.IMAGE, png.name,
                                                GameManager.Instance.fileManager.FindFile(png.name).fileParentName);
                                    }

                                    evidence.Q<Button>("EvidenceImage").pickingMode = PickingMode.Ignore;
                                });
                            }
                            else
                            {
                                evidence = UIReader_Main.Instance.RemoveContainer(ux_imageEvidence.Instantiate());

                                evidence.Q<Button>("EvidenceImage").style.backgroundImage = new StyleBackground(png.image);
                                evidence.Q<Button>("EvidenceImage").clicked += (() =>
                                {
                                    // remove description
                                    //for (int i = imagePanel.Q<VisualElement>("ImageGround").childCount - 1; i >= 0; i--)
                                    //{
                                    //    if (imagePanel.Q<Button>("ImageGround").Children().ElementAt(i).name == "descriptionLabel")
                                    //        imagePanel.Q<Button>("ImageGround").RemoveAt(i);
                                    //}
                                    // when description is not null
                                    if (ui_description != null)
                                    {
                                        ui_description.parent.Remove(ui_description);
                                        ui_description = null;
                                    }

                                    // add description
                                    // create description
                                    VisualElement description
                                        = UIReader_Main.Instance.RemoveContainer(ux_evidenceExplanation.Instantiate());
                                    // save description
                                    ui_description = description;
                                    // attach to imageGround
                                    imagePanel.Q<Button>("ImageGround").Add(description);
                                    // input description text
                                    UIReader_Main.Instance.DoText(description.Q<Label>("Text"), png.memo, 2f, false,
                                        () => { }, "");
                                    // check condition
                                    GameManager.Instance.fileManager.UnlockChat(png.name);
                                });
                            }

                            evidence.style.position = Position.Absolute;
                            evidence.Q<Button>("EvidenceImage").style.left = png.pos.x;
                            evidence.Q<Button>("EvidenceImage").style.top = png.pos.y;
                            evidence.Q<Button>("EvidenceImage").style.width = png.size.x;
                            evidence.Q<Button>("EvidenceImage").style.height = png.size.y;
                            imagePanel.Q<VisualElement>("ImageGround").Add(evidence);
                        }
                    }
                    else
                        Debug.Log("Evidence not found in pngList");
                }

                ui_imageGround.Add(imagePanel);
                GameManager.Instance.chatHumanManager.IsChat(false);
            }

            //isImageOpen = !isImageOpen;
        }
        // png ????
        else
        {
            // find png
            PngSO png = GameManager.Instance.imageManager.FindPng(fileName);

            // When png isn't null
            if (png != null)
            {
                // png panel clear
                for (int i = ui_panelGround.childCount - 1; i >= 0; i--)
                    ui_panelGround.RemoveAt(i);

                //create uxml
                VisualElement panel = UIReader_Main.Instance.RemoveContainer(ux_ImagePanel.Instantiate());
                panel.name = "panel";
                // change png panel name
                panel.Q<Label>("Name").text = png.name + ".png";
                // change png image
                panel.Q<VisualElement>("Image").style.backgroundImage = new StyleBackground(png.saveSprite);
                // change png size
                UIReader_Main.Instance.ReSizeImage(panel.Q<VisualElement>("Image"), png.saveSprite);
                // connection exit click event
                panel.Q<Button>("CloseBtn").clicked += () =>
                {
                    GameManager.Instance.chatHumanManager.IsChat(true);
                    // remove this panel 
                    ui_panelGround.Remove(panel);
                };

                // png check action
                FileSO file = GameManager.Instance.fileManager.FindFile(png.name);
                if (file != null)
                    GameManager.Instance.fileManager.UnlockChat(file.name);

                GameManager.Instance.fileManager.FindFile(png.name).isRead = true;
                if (GameManager.Instance.fileManager.FindFile(png.name).isRead == true)
                    fileIcon.Q<VisualElement>("NewIcon").style.display = DisplayStyle.None;

                ui_panelGround.Add(panel);

                GameManager.Instance.chatHumanManager.IsChat(false);
            }
            else
                Debug.Log("it's neither an image nor a png");
        }
    }

    public void OpenText(VisualElement fileIcon, string name)
    {
        // create uxml
        VisualElement panel = UIReader_Main.Instance.RemoveContainer(ux_TextPanel.Instantiate());
        UIReader_Main.Instance.RemoveSlider(panel);

        // find text
        TextSO text = GameManager.Instance.imageManager.FindText(name);

        // When png isn't null
        if (text != null)
        {
            // text panel clear
            for (int i = ui_panelGround.childCount - 1; i >= 0; i--)
                ui_panelGround.RemoveAt(i);

            panel.name = "panel";
            // change name
            panel.Q<Label>("Name").text = name + ".text";
            // change memo
            panel.Q<Label>("Text").text = text.memo;
            // connection exit click event
            panel.Q<Button>("CloseBtn").clicked += () =>
            {
                GameManager.Instance.chatHumanManager.IsChat(true);
                // remove this panel 
                ui_panelGround.Remove(panel);

                // text check action
                FileSO file = GameManager.Instance.fileManager.FindFile(name);
                if (file != null)
                    GameManager.Instance.fileManager.UnlockChat(file.name);
            };

            if (fileIcon != null)
            {
                GameManager.Instance.fileManager.FindFile(name).isRead = true;
                if (GameManager.Instance.fileManager.FindFile(name).isRead == true)
                    fileIcon.Q<VisualElement>("NewIcon").style.display = DisplayStyle.None;
            }

            ui_panelGround.Add(panel);

            GameManager.Instance.chatHumanManager.IsChat(false);
        }
    }
}