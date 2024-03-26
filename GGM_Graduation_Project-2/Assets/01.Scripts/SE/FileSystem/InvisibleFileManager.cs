using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct RoundFile
{
    public string round;        // == Chapter
    public GameObject[] Files;
}

public class InvisibleFileManager : MonoBehaviour
{
    public static InvisibleFileManager Instance;

    public RoundFile[] roundFile;        // ���� ���� ������ ��
    HashSet<string> ShowRoundSet = new HashSet<string>();

    private void Awake()
    {
        Instance = this;
        ShowRoundSet.Clear();
    }

    public void Start()
    {
        DontShowRound();
    }

    public void  ShowRoundFile(string round)
    {
        foreach (var file in roundFile)
        {
            if (file.round == round)
            {
                foreach(var rountFile in file.Files)
                {
                    rountFile.SetActive(true);
                    FileManager.instance.NowFilePathIncludeFileCheck(rountFile);     // Ű���� ���� ���� ��ο� �־�߸���.
                }
            }
        }
        ShowRoundSet.Add(round);
    }

    public void DontShowRound()
    {
        foreach (var file in roundFile)
        {
            if (ShowRoundSet.Contains(file.round) == false)      // ���ԵǾ����� ������ ���ְ� ���� ���� ��ο� ������ ���������.
            {
                foreach (var obj in file.Files)
                {
                    obj.gameObject.SetActive(false);
                }
            }
        }
    }
}
