using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleFolder : MonoBehaviour
{
    public int myPuzzleNum;     // 1���� ������, 2���� 8����

    public void OpenPuzzle()
    {
        FileManager.instance.OpenPuzzleLock(myPuzzleNum);
        if (myPuzzleNum == 1)
        {
            Sudoku.Instance.Setting();
        }
        else if (myPuzzleNum == 2)
        {
            // �μ� ���� �����Ұ� �־��ֱ�
        }
    }
}
