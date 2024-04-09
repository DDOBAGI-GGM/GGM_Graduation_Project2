using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChatState
{
    Other = 0,      // �������� ���ϴ� ��
    Me = 1,      // ���簡 ���ϴ� ��
    Ask = 2,        // ���簡 ���� ����.
    LoadNext        // �� �̾��� ��ȭ�� �ִٸ�. �ٸ� ����̸�.
}

[Serializable]
public struct Chat      // ���� � ���� �ϴ°�
{
    public ChatState state;
    public string text;        // �� �ϴ� ��.
}

[Serializable]
public struct AskAndReply
{
    public string ask;        // ���� �� �ִ� ������
    public string[] reply;     // �׿� ���� ����
}

[Serializable]
public struct Chapters
{
    public Chat[] chat;
    public AskAndReply[] askAndReply;
    public string who;     // ������?
}

public class ChatStruct : MonoBehaviour { }
