using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public enum FileType
{
    FOLDER,
    IMAGE,
    TEXT
}

public class UIReader_FileSystem : MonoBehaviour
{
    // other system
    TestUI mainSystem;
    UIReader_Chatting chatSystem;
    UIReader_Connection connectionSystem;

    // main
    private UIDocument document;
    private VisualElement root;
    //private VisualElement fileRoot;

    // UXML
    VisualElement fileArea;
    VisualElement filePathGround;
    VisualElement mainFilePath;

    // Template
    VisualTreeAsset ux_folderFile;
    VisualTreeAsset ux_imageFile;
    VisualTreeAsset ux_textFile;
    VisualTreeAsset ux_filePath;
    VisualTreeAsset ux_fileGround;

    // test path
    public Stack<string> filePathLisk = new Stack<string>();

    private void Awake()
    {
        mainSystem = GetComponent<TestUI>();
        chatSystem = GetComponent<UIReader_Chatting>();
        connectionSystem = GetComponent<UIReader_Connection>();

        //currentFileGround = fileDefaultGround;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            AddFile(FileType.FOLDER, "Main", "�б�");
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            AddFile(FileType.IMAGE, "�б�", "����");
        }
    }

    private void OnEnable()
    {
        document = GetComponent<UIDocument>();
        root = document.rootVisualElement;
        //fileRoot = root.Q<VisualElement>("");

        UXML_Load();
        Template_Load();
        Event_Load();
    }

    private void UXML_Load()
    {
        fileArea = root.Q<VisualElement>("FileArea");
        filePathGround = root.Q<VisualElement>("FilePath");

        //mainFilePath = filePathGround.Q<VisualElement>("");
    }

    private void Template_Load()
    {
        ux_folderFile = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\File\\FolderFile.uxml");
        ux_imageFile = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\File\\ImageFile.uxml");
        ux_textFile = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\File\\TextFile.uxml");
        ux_fileGround = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\File\\FileGround.uxml");
        ux_filePath = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\File\\FilePathText.uxml");
    }

    private void Event_Load()
    {
        //filePathLisk.Push(mainFilePath.Q<Button>().text);
        //mainFilePath.Q<Button>().clicked += () =>
        //{
        //    FolderPathEvent(mainFilePath.Q<Button>().text);
        //};

        AddFilePath("Main", () => FolderPathEvent("Main"));
    }

    // Function
    public void AddFile(FileType fieType, string fileGroundName, string fileName, bool isRock = false)
    {
        // ����
        VisualElement file = null;

        // ���� Ÿ�� ������
        switch (fieType)
        {
            case FileType.FOLDER:
            {
                // ���� ���� �� �̺�Ʈ ����
                file = ux_folderFile.Instantiate();
                file.Q<Button>().clicked += () => FolderEvent(file);

                // ������ ������ ���� �� ���
                VisualElement newFileGround = ux_fileGround.Instantiate();
                newFileGround.name += "FileGround_" + fileName;
                fileArea.Add(newFileGround); // FileGround_�б�
                break;
            }
            case FileType.IMAGE:
                // ���� ���� �� �̺�Ʈ ����
                file = ux_imageFile.Instantiate();
                file.Q<Button>().clicked += () => ImageEvent(file);
                break;
            case FileType.TEXT:
                // ���� ���� �� �̺�Ʈ ����
                file = ux_textFile.Instantiate();
                file.Q<Button>().clicked += () => NoteEvent(file);
                break;
        }

        // ���� �̸� ����
        file.Q<Label>().text = fileName;

        // ���� �����ϴ� �����̶�� �ش� ������ �߰�
        foreach (VisualElement fileGround in fileArea.Children())
        {
            int index = fileGround.name.IndexOf('_');
            if (index != -1)
            {
                if (fileGround.name.Substring(index + 1) == fileGroundName)
                {
                    fileGround.Add(file);
                    return;
                }
            }
        }

        // �ƴ϶�� �ش� ������ ���� �� �߰�
        {
            VisualElement newFileGround = ux_fileGround.Instantiate();
            newFileGround.name += "FileGround_" + fileGroundName;
            newFileGround.style.display = DisplayStyle.None;
            fileArea.Add(newFileGround);
            newFileGround.Add(file);
        }
    }

    private void FolderEvent(VisualElement folder)
    {
        // ���� �̸� ����
        string folderName = folder.Q<Label>("FileName").text;

        // �ش� ���� ������ Ȱ��ȭ
        OpenFileGround(folderName);
        // ���� ��� �߰�
        AddFilePath(folderName, () => FolderPathEvent(folderName));
    }

    private void ImageEvent(VisualElement image)
    {
        Debug.Log("�̹��� ���� Ȱ��ȭ");
    }

    private void NoteEvent(VisualElement note)
    {
        Debug.Log("�ؽ�Ʈ ���� Ȱ��ȭ");
    }

    private void AddFilePath(string pathName, Action action)
    {
        // �̹� �����Ѵٸ� Ȱ��ȭ
        foreach(VisualElement filePath in filePathGround.Children())
        {
            if (StringSplit(filePath.name, '_') == pathName)
            {
                filePath.style.display = DisplayStyle.Flex;
                filePathLisk.Push(filePath.name);
                return;
            }
        }

        {
            Debug.Log("��� ����");
            // ����
            VisualElement filePath = null;
            filePath = ux_filePath.Instantiate();

            // ��� �̸� ���� (UI �̸�)
            filePath.name += "FilePathText_" + pathName;
            // ��� �̸� ����
            filePath.Q<Button>().text = pathName + " > ";
            // ��� �̺�Ʈ ����
            filePath.Q<Button>().clicked += action;
            // ��� �߰�
            filePathGround.Add(filePath);

            // �߰��� �� ���� �迭 ���� ���� ���������� ���� �� ������ �� �� �ε����� ����, FIleManager GoFile ����
            filePathLisk.Push(filePath.name);
        }
    }

    private void FolderPathEvent(string fileName)
    {
        // fileName���� ���� �ִ� ���� ��Ȱ��ȭ
        while (true)
        {
            if (StringSplit(filePathLisk.Peek(), '_') == fileName)
                break;
            Debug.Log(StringSplit(filePathLisk.Peek(), '_') + " " + " ��� false");
            filePathGround.Q<VisualElement>(filePathLisk.Peek()).style.display = DisplayStyle.None;
            //filePathGround.Remove(filePathGround.Q<VisualElement>("FilePathText" + '_' + filePathLisk.Peek()));
            // �߰��� ���� Add�� ���� ���� active false�� �������� ��, �� �� ���� ������ ���ߵ� Ȱ��ȭ ��Ȱ��ȭ�� ���ߵ� ���Ͻ��Ѷ�
            filePathLisk.Pop();
        }

        OpenFileGround(StringSplit(filePathLisk.Peek(), '_'));
    }

    private void OpenFileGround(string fileName)
    {
        // FileArea�� ��� Ground ��
        foreach (VisualElement fileGround in fileArea.Children())
        {
            // �ش� ������ ������ 
            //Debug.Log(StringSplit(fileGround.name, '_') + " " + fileName + " open");
            if (StringSplit(fileGround.name, '_') == fileName)
            {
                fileGround.style.display = DisplayStyle.Flex;
                Debug.Log(StringSplit(fileGround.name, '_') + " ���� true");
            }
            // �ƴ϶�� ����
            else
            {
                fileGround.style.display = DisplayStyle.None;
                Debug.Log(StringSplit(fileGround.name, '_') + " ���� false");
            }
        }
    }

    private string StringSplit(string str, char t)
    {
        int index = str.IndexOf(t);
        if (index != -1)
            return str.Substring(index + 1);
        else
        {
            Debug.LogError("String Split ����");
            return null;
        }
    }
}
