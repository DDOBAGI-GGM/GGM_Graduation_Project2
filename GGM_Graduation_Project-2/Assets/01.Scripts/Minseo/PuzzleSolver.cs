using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleState : IComparable<PuzzleState>
{
    public int[,] Board { get; } // ������ ���¸� ��Ÿ���� 2���� �迭
    public int GCost { get; } // ���� ���·κ����� �̵� ��� (��������� �̵� Ƚ��)
    public int HCost { get; } // ���� ���·κ��� ��ǥ ���±����� ���� �̵� ��� (�޸���ƽ)

    public PuzzleState(int[,] board, int gCost, int hCost)
    {
        Board = board;
        GCost = gCost;
        HCost = hCost;
    }

    // A* �˰����� ���� F �� ���
    public int FCost => GCost + HCost;

    // ���� ���¸� ���
    public void Print()
    {
        for (int i = 0; i < Board.GetLength(0); i++)
        {
            for (int j = 0; j < Board.GetLength(1); j++)
            {
                Console.Write(Board[i, j] + " ");
            }
            Console.WriteLine();
        }
    }

    // IComparable �������̽� ����
    public int CompareTo(PuzzleState other)
    {
        return FCost.CompareTo(other.FCost);
    }
}


public class PuzzleSolver : MonoBehaviour
{
    // �̵� ������ ������ Ÿ���� ��ġ
    private static readonly int[,] moves = new int[4, 2] { { 0, 1 }, { 0, -1 }, { 1, 0 }, { -1, 0 } };

    // A* �˰����� ����Ͽ� �ּ� �̵� Ƚ���� ����ϴ� �Լ�
    public static int CalculateMinimumMoves(int[,] initialState, int[,] goalState)
    {
        HashSet<string> visited = new HashSet<string>();
        Queue<PuzzleState> openSet = new    Queue<PuzzleState>();

        // ���� ���¸� ť�� �߰�
        PuzzleState startState = new PuzzleState(initialState, 0, CalculateHeuristic(initialState, goalState));
        openSet.Enqueue(startState);

        while (openSet.Count > 0)
        {
            PuzzleState currentState = openSet.Dequeue();

            // ���� ���°� ��ǥ �������� Ȯ��
            if (IsEqual(currentState.Board, goalState))
            {
                return currentState.GCost; // �ּ� �̵� Ƚ�� ��ȯ
            }

            // ���� ���¿��� ������ ��� ���� ���¸� Ž��
            foreach (PuzzleState nextState in GetNextStates(currentState, goalState))
            {
                string nextStateHash = HashState(nextState.Board);

                // �湮�� �������� Ȯ��
                if (!visited.Contains(nextStateHash))
                {
                    visited.Add(nextStateHash);
                    openSet.Enqueue(nextState);
                }
            }
        }
        return -1;
    }

    // �� ���°� ������ Ȯ���ϴ� �Լ�
    private static bool IsEqual(int[,] state1, int[,] state2)
    {
        for (int i = 0; i < state1.GetLength(0); i++)
        {
            for (int j = 0; j < state1.GetLength(1); j++)
            {
                if (state1[i, j] != state2[i, j])
                    return false;
            }
        }
        return true;
    }

    // ���¸� �ؽ÷� ��ȯ�ϴ� �Լ�
    private static string HashState(int[,] state)
    {
        string hash = "";

        for (int i = 0; i < state.GetLength(0); i++)
        {
            for (int j = 0; j < state.GetLength(1); j++)
            {
                hash += state[i, j].ToString();
            }
        }

        return hash;
    }

    // ���� ���¿��� ������ ���� ���¸� ��ȯ�ϴ� �Լ�
    private static List<PuzzleState> GetNextStates(PuzzleState currentState, int[,] goalState)
    {
        List<PuzzleState> nextStates = new List<PuzzleState>();

        int emptyTileX = -1;
        int emptyTileY = -1;

        // �� Ÿ���� ��ġ�� ã��
        for (int i = 0; i < currentState.Board.GetLength(0); i++)
        {
            for (int j = 0; j < currentState.Board.GetLength(1); j++)
            {
                if (currentState.Board[i, j] == 0)
                {
                    emptyTileX = i;
                    emptyTileY = j;
                    break;
                }
            }
        }

        // ���� ���¿��� ������ ���� ���¸� ����
        for (int i = 0; i < moves.GetLength(0); i++)
        {
            int newX = emptyTileX + moves[i, 0];
            int newY = emptyTileY + moves[i, 1];

            if (IsValidPosition(newX, newY, currentState.Board.GetLength(0), currentState.Board.GetLength(1)))
            {
                int[,] newBoard = (int[,])currentState.Board.Clone();
                newBoard[emptyTileX, emptyTileY] = newBoard[newX, newY];
                newBoard[newX, newY] = 0;

                PuzzleState nextState = new PuzzleState(newBoard, currentState.GCost + 1, CalculateHeuristic(newBoard, goalState));
                nextStates.Add(nextState);
            }
        }

        return nextStates;
    }

    // ��ȿ�� ��ġ���� Ȯ���ϴ� �Լ�
    private static bool IsValidPosition(int x, int y, int maxX, int maxY)
    {
        return x >= 0 && x < maxX && y >= 0 && y < maxY;
    }

    private static int CalculateHeuristic(int[,] currentState, int[,] goalState)
    {
        int heuristic = 0;

        for (int i = 0; i < currentState.GetLength(0); i++)
        {
            for (int j = 0; j < currentState.GetLength(1); j++)
            {
                int value = currentState[i, j];

                if (value != 0) // �� Ÿ���� ������� ����
                {
                    // ���� Ÿ���� ��ġ�� ã��
                    for (int x = 0; x < goalState.GetLength(0); x++)
                    {
                        for (int y = 0; y < goalState.GetLength(1); y++)
                        {
                            if (goalState[x, y] == value)
                            {
                                // ���� Ÿ���� ��ǥ ��ġ���� ����ư �Ÿ��� ����Ͽ� �޸���ƽ�� �߰�
                                heuristic += Math.Abs(i - x) + Math.Abs(j - y);
                                break;
                            }
                        }
                    }
                }
            }
        }

        return heuristic;
    }
}
