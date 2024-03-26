using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleFolder : MonoBehaviour
{
    public int myPuzzleNum;     // 1���� ������, 2���� 8����
    
    [SerializeField]
    private Transform board;
    [SerializeField]
    private GameObject puzzlePrefab;

    public void OpenPuzzle()
    {
        FileManager.instance.OpenPuzzleLock(myPuzzleNum);
        if (myPuzzleNum == 1)
        {
            Sudoku.Instance.Setting();
        }
        else if (myPuzzleNum == 2)
        {
            Instantiate(puzzlePrefab, board);
        }
    }
}
