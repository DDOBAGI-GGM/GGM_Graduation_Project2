using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ChatState
{
    Assistant = 0,      // ������ ���ϴ� ��
    Detective = 1,      // ���簡 ���ϴ� ��
}

[Serializable]
public struct Chat      // ���� � ���� �ϴ°�
{
    public ChatState state;
    public string text;        // �� �ϴ� ��.
}


[CreateAssetMenu(fileName = "ChatSO", menuName = "SO/ChatSO")]
public class ChatSO : ScriptableObject
{
    public Chat[] chat;
    public bool is_Ask;     // ���� ���� �ֳ�
    public string askName;      // is_Ask �� true ��� Ask�� �̸��� �����ֱ�
}       // ���ø� �ϴ� SO


/*
 SO �� �� ��.

�������� ���� ��  
1. ������ ��
2. ���� ��

�������� ���� ��
0. ���� ������ ���� ���� ����.
1. ���� ������ �� �ִ� ��������
2. �� �������� ���� ������ ��
3. ���� �� �������� �����ߴ��� ���ߴ���

ť�� �Ἥ ȭ���� ���� ������ �ؽ�Ʈ�� �������� �ϱ�

�� é�;� ������ �����
 */