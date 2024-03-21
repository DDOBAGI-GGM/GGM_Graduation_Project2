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
public struct Chat
{
    public ChatState state;
    public string[] text;        // �� �ϴ� ��.
}

[Serializable]
public struct Ask
{
    public string[] ask;
    public string[] reply;
}

[CreateAssetMenu(fileName = "ChatSO", menuName = "SO/ChatSO")]
public class ChatSO : ScriptableObject
{
    public Chat[] chat;
}       // ���ø� �ϴ� SO

/*
 SO �� �� ��.

�������� ���� ��  
1. ������ ��
2. ���� ��

�������� ���� ��
1. ���� ������ �� �ִ� ��������
2. �� �������� ���� ������ ��
3. ���� �� �������� �����ߴ��� ���ߴ���

ť�� �Ἥ ȭ���� ���� ������ �ؽ�Ʈ�� �������� �ϱ�

�� é�;� ������ �����
 */