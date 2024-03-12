using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/*
 ������ �¾�Ƽ�긦 ���ٰ� Ű�� ������ �̵��� �����Ѵ�.
  
��δ� ����/�б�/��¼�� ������ ��.
 */

[Serializable]
public struct FileTree
{
    public string path;
    public GameObject[] Files;
}

public class FileManager : MonoBehaviour
{
    public static FileManager instance;     // ��𼭳� ȣ���� �� �ֵ���

    public BackBtn backBtn;         // �ڷΰ���
    public GameObject[] upLinePathBtn;     // ���ٿ� ��� ǥ�ÿ�
    public FileTree[] fileTrees;        // ���� ��ü ����

    private string nowPath;     // ���� ���
    private TMP_Text[] upLinePathText;       // ���ٿ� ��� ǥ�� �ؽ�Ʈ


    private void Awake()
    {
        instance = this;
        nowPath = "����";

        upLinePathText = new TMP_Text[upLinePathBtn.Length];
        for (int i = 0; i < upLinePathBtn.Length; i++)
        {
            upLinePathText[i] = upLinePathBtn[i].GetComponentInChildren<TMP_Text>();
            upLinePathBtn[i].SetActive(false);
        }
    }
    /*
     GoFile �Լ�
    1. �� ��ο� ���� ��ΰ� ���� ����
    2. ���� ��� ������ ��� ��Ƽ�긦 ���ְ�
    3. �� ��ο� �ִ� ���ϵ��� ��� Ƽ�긦 ���ش�.
     */
    public void GoFile(string nowPath, string goPath)
    {
        Debug.Log($"���� ��� : {nowPath}, ������ ��� : {goPath}");

        foreach (var fileTree in fileTrees)
        {
            if (fileTree.path == nowPath)
            {
                foreach (var file in fileTree.Files)
                {
                    file.SetActive(false);
                }
            }

            if (fileTree.path == goPath)
            {
                foreach (var file in fileTree.Files)
                {
                    file.SetActive(true);
                }
            }
        }

        backBtn.nowPath = goPath;       // �ڷΰ��� �����
        this.nowPath = goPath;      // ���� ��� ǥ��

        // �� ��� ǥ�ÿ�
        if (this.nowPath.LastIndexOf('\\') != -1)
        {
            string[] path = this.nowPath.Split('\\');
            for (int i = 0;i < path.Length - 1; ++i)
            {
                upLinePathBtn[i].SetActive(true);
                upLinePathText[i].text = path[i + 1];
            }
            for (int i = path.Length; i < 3; ++i)
            {
                upLinePathBtn[i].SetActive(false);      // ��ΰ� ���� ���̸� �����ֱ�
            }
        }
    }

    public void GoMain()
    {
        GoFile(nowPath, "����");
    }
}

