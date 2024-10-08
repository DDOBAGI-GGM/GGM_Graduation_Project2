using ChatVisual;
using DG.Tweening;
using DG.Tweening.Core.Easing;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum FileType
{
    FOLDER,
    IMAGE,
    TEXT
}

[Serializable]
public class FolderFile
{
    public string folderName;
    public string parentFolderName;
    public List<VisualElement> folderFiles;
    public List<VisualElement> textFiles;
    public List<VisualElement> imageFiles;

    public FolderFile(string name, string parent)
    {
        folderName = name;
        parentFolderName = parent;
        folderFiles = new List<VisualElement>();
        textFiles = new List<VisualElement>();
        imageFiles = new List<VisualElement>();
    }
}

public class FileSystem : MonoBehaviour
{
    static public FileSystem Instance;

    public bool isPathClick = false;

    [SerializeField]
    private float fileAreaSizeOn, fileAreaSizeOff;
    [SerializeField]
    private Texture2D changeSizeBtnOn, changeSizeBtnOff;
    public bool isFileSystemOpen;

    Tween changeFileSystemSizeDOT;

    // root
    VisualElement root;

    // UXML
    VisualElement ui_fileSystemArea;
    public VisualElement ui_fileGround;
    VisualElement ui_filePathGround;
    VisualElement ui_mainFilePath;
    [HideInInspector] public Button ui_changeSizeButton;
    [HideInInspector] public VisualElement ui_hpGround;

    // Template
    [Header("Template")]
    [SerializeField] VisualTreeAsset ux_filePath;
    [SerializeField] VisualTreeAsset ux_fileGround;
    [SerializeField] VisualTreeAsset ux_folderFile;
    [SerializeField] VisualTreeAsset ux_imageFile;
    [SerializeField] VisualTreeAsset ux_textFile;

    // folder
    public string currentFolderName;
    [SerializeField] List<FolderFile> fileFolders; // this is test, fileFolders -> fileFolderList X!!

    // path
    private Stack<string> filePathLisk;
    public FolderFile currentFileFolder;

    // file drag and drop
    private VisualElement fileDefaultArea;
    private List<VisualElement> lockQuestions;

    [Header("Use Relationship")]
    public Sprite textFileSprite;

    private void Awake()
    {
        Instance = this;

        fileFolders = new List<FolderFile>();
        filePathLisk = new Stack<string>();
        lockQuestions = new List<VisualElement>();
    }

    private void OnEnable()
    {
        UXML_Load();
        Event_Load();

        fileFolders.Add(new FolderFile("Main", "Main"));
        AddFilePath("Main");
    }

    private void UXML_Load()
    {
        root = GameObject.Find("Game").GetComponent<UIDocument>().rootVisualElement;

        ui_fileSystemArea = root.Q<VisualElement>("FileSystem");
        ui_fileGround = UIReader_Main.Instance.root.Q<VisualElement>("FileGround");
        ui_filePathGround = UIReader_Main.Instance.root.Q<VisualElement>("FilePathGround");
        ui_changeSizeButton = UIReader_Main.Instance.root.Q<Button>("ChangeSize");
        ui_hpGround = UIReader_Main.Instance.root.Q<VisualElement>("HPbar");
    }

    private void Event_Load()
    {
        ui_changeSizeButton.clicked += () =>
        {
            OnOffFileSystem(0.25f);
        };
    }

    private void LoadDragAndDrop(VisualElement file, Action action)
    {
        // drl!
        file.AddManipulator(new Dragger((evt, target, beforeSlot) =>
        {
            // get questionGround
            VisualElement questionGround = GameManager.Instance.chatSystem.ui_questionGround;
            VisualElement relationshipGround = GameManager.Instance.relationshipSystem.ui_RelationshipGround;

            // 만약 questionGround와 부딪혔다면
            if (questionGround.worldBound.Contains(evt.mousePosition))
            {
                // currnet member 가져오고
                MemberProfile member = GameManager.Instance.chatHumanManager.currentMember;

                bool isCorrect = false;

                // current member의 question 다 돌고?
                for (int i = 0; i < member.questions.Count; ++i)
                {
                    // ask의 condition 가져오기
                    if (member.questions[i].parent is ConditionNode condition)
                    {
                        // file name 가져오고
                        string fileName = file.Q<Label>("FileName").text;

                        // condition names 가져와서
                        string[] names = condition.fileName.Split('/');

                        // 둘이 비교해
                        foreach (string name in names)
                        {
                            // when file is not null
                            if (GameManager.Instance.fileManager.FindFile(name) != null)
                            {
                                // name(condition) == fileName(file)
                                if (GameManager.Instance.fileManager.FindFile(name).fileName.Trim() == fileName.Trim())
                                {
                                    if (condition.is_Unlock == false)
                                    {
                                        // unlock 
                                        condition.is_Unlock = true;

                                        // remove visualElement
                                        for (int j = 0; j < questionGround.childCount; ++j)
                                        {
                                            if (questionGround[j].name == "HiddenAskChat")
                                            {
                                                questionGround.RemoveAt(j);
                                                break;
                                            }
                                        }

                                        //change from lockQustion to question - 질문으로 만드는 거
                                        GameManager.Instance.chatSystem.InputQuestion(member.name, false, condition.childList[0] as AskNode);

                                        UIReader_Main.Instance.PlusHP();

                                        // add question
                                        // 이거 다시 켜야될지도?
                                        //member.questions.Add(condition.childList[0] as AskNode);

                                        isCorrect = true;
                                        beforeSlot.Add(target);
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }

                // minus hp
                if (isCorrect == false)
                    UIReader_Main.Instance.MinusHP();
            }
            else if (UIReader_Main.Instance.isRelationshipOpen)      // 관계도 시스템이 켜져 있었다면
            {
                Debug.Log("관계도 시스템 켜져있음");
                if (relationshipGround.worldBound.Contains(evt.mousePosition))
                {
                    GameManager.Instance.relationshipSystem.AddEvidence(file.Q<Label>("FileName").text);
                }
            }

            beforeSlot.Add(target);
        },
        () => { action(); }
        ));
    }

    private FolderFile FindFolder(string name)
    {
        foreach (FolderFile folder in fileFolders)
        {
            if (folder.folderName == name)
                return folder;
        }

        return null;
    }

    public void AddFile(FileType fileType, string fileName, string fileParentName)
    {
        // find parentFolder
        FolderFile parentFolder = FindFolder(fileParentName);

        // if exist parenteFolder
        if (parentFolder != null)
        {
            VisualElement file;

            // register folder to parentFolder
            switch (fileType)
            {
                case FileType.FOLDER:
                    {
                        FileSO folder = GameManager.Instance.fileManager.FindFile(fileName);

                        // create uxml
                        file = UIReader_Main.Instance.RemoveContainer(ux_folderFile.Instantiate());

                        // change file name
                        file.name = folder.fileName;
                        file.Q<Label>("FileName").text = folder.fileName;

                        // connection click event
                        LoadDragAndDrop(file, () =>
                        {
                            GameManager.Instance.fileManager.FindFile(fileName).isRead = true;

                            // image check action
                            if (folder != null)
                                GameManager.Instance.fileManager.UnlockChat(folder.name);
                            if (GameManager.Instance.fileManager.FindFile(fileName).isRead == true)
                                file.Q<VisualElement>("NewIcon").style.display = DisplayStyle.None;

                            // draw current foluder
                            DrawFile(folder.fileName);

                            // add current folder path
                            AddFilePath(folder.fileName);
                        });

                        fileFolders.Add(new FolderFile(fileName, fileParentName));

                        // add file
                        bool overlapping = false;
                        for (int i = 0; i < parentFolder.imageFiles.Count; ++i)
                        {
                            if (parentFolder.imageFiles[i].name == file.name)
                                overlapping = true;
                        }
                        if (overlapping == false)
                            parentFolder.imageFiles.Add(file);
                    }
                    break;
                case FileType.IMAGE:
                    {
                        FileSO image = GameManager.Instance.fileManager.FindFile(fileName);

                        // create uxml
                        file = UIReader_Main.Instance.RemoveContainer(ux_imageFile.Instantiate());

                        // change file name
                        file.name = image.fileName;
                        file.Q<Label>("FileName").text = image.fileName;

                        // connection drag and drop & button click event
                        LoadDragAndDrop(file, () => { GameManager.Instance.imageSystem.OpenImage(file, image.fileName); });

                        // add file
                        bool overlapping = false;
                        for (int i = 0; i < parentFolder.imageFiles.Count; ++i)
                        {
                            if (parentFolder.imageFiles[i].name == file.name)
                                overlapping = true;
                        }
                        if (overlapping == false)
                            parentFolder.imageFiles.Add(file);
                    }
                    break;
                case FileType.TEXT:
                    {
                        FileSO text = GameManager.Instance.fileManager.FindFile(fileName);

                        // create uxml
                        file = UIReader_Main.Instance.RemoveContainer(ux_textFile.Instantiate());

                        // change file name
                        file.name = text.fileName;
                        file.Q<Label>("FileName").text = text.fileName;

                        // connection drag and drop & button click event
                        LoadDragAndDrop(file, () => { GameManager.Instance.imageSystem.OpenText(file, text.fileName); });

                        // add file
                        bool overlapping = false;
                        for (int i = 0; i < parentFolder.imageFiles.Count; ++i)
                        {
                            if (parentFolder.imageFiles[i].name == file.name)
                                overlapping = true;
                        }
                        if (overlapping == false)
                            parentFolder.imageFiles.Add(file);
                    }
                    break;
            }
        }
        // if not exist parenteFolder
        else
        {
            Debug.Log(fileParentName + " this is null");
        }

        if (currentFolderName == "")
            currentFolderName = fileParentName;
        if (currentFolderName == fileParentName)
            DrawFile(currentFolderName);
    }
    public void DrawFile(string folderName)
    {
        // change current folder
        currentFolderName = folderName;
        currentFileFolder = FindFolder(folderName);

        // all file remove of fileGround
        RemoveFile();

        // folder isn't null
        if (currentFileFolder != null)
        {
            // create current folder's childen
            foreach (VisualElement folder in currentFileFolder.folderFiles)
                ui_fileGround.Add(folder);

            foreach (VisualElement image in currentFileFolder.imageFiles)
                ui_fileGround.Add(image);

            foreach (VisualElement text in currentFileFolder.textFiles)
                ui_fileGround.Add(text);
        }
        else
            Debug.Log("current folder file null");
    }

    private void RemoveFile()
    {
        for (int i = ui_fileGround.childCount - 1; i >= 0; i--)
            ui_fileGround.RemoveAt(i);
    }

    private void AddFilePath(string pathName)
    {
        VisualElement filePath = UIReader_Main.Instance.RemoveContainer(ux_filePath.Instantiate());

        filePath.Q<Button>().text = pathName + "> ";
        filePath.Q<Button>().clicked += () => { PathEvent(pathName); isPathClick = true; };

        ui_filePathGround.Add(filePath);
        filePathLisk.Push(pathName);
    }

    private void PathEvent(string folderName)
    {
        while (true)
        {
            if (filePathLisk.Peek() == folderName)
                break;
            ui_filePathGround.RemoveAt(ui_filePathGround.childCount - 1);
            filePathLisk.Pop();
        }

        DrawFile(filePathLisk.Peek());
    }

    public void HyperLinkEvent(string folderName)
    {
        isPathClick = true;

        if (GameManager.Instance.fileSystem.FindFolder(folderName) != null)
        {
            Stack<string> pathName = new Stack<string>();
            string top = GameManager.Instance.fileSystem.FindFolder(folderName).parentFolderName;

            // all remove paths
            for (int i = ui_filePathGround.childCount - 1; i >= 0; i--)
                ui_filePathGround.RemoveAt(i);

            while (top != "Main")
            {
                pathName.Push(top);
                top = GameManager.Instance.fileSystem.FindFolder(top).parentFolderName;
            }

            AddFilePath("Main");
            while (pathName.Count > 0)
            {
                AddFilePath(pathName.Peek());
                pathName.Pop();
            }

            DrawFile(GameManager.Instance.fileSystem.FindFolder(folderName).parentFolderName);
        }
        else
            Debug.LogError("this folder is not exist");
    }

    public void OnOffFileSystem(float during)
    {
        isFileSystemOpen = !isFileSystemOpen;

        if (isFileSystemOpen)
        {
            ui_fileSystemArea.RemoveFromClassList("OffFileSystem");
            ui_fileSystemArea.AddToClassList("OnFileSystem");
            ui_changeSizeButton.style.backgroundImage = new StyleBackground(changeSizeBtnOn);
        }
        else
        {
            ui_fileSystemArea.RemoveFromClassList("OnFileSystem");
            ui_fileSystemArea.AddToClassList("OffFileSystem");
            ui_changeSizeButton.style.backgroundImage = new StyleBackground(changeSizeBtnOff);
        }
    }
}