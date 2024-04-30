using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_ChatState
{
    Other = 0,      // �������� ���ϴ� ��
    Me = 1,      // ���簡 ���ϴ� ��
    Ask = 2,        // ���簡 ���� ����.
    LoadNext        // �� �̾��� ��ȭ�� �ִٸ�. �ٸ� ����̸�.
}

public enum E_Face
{
    Default,        // ��ǥ��
    Blush,          // ��Ȳ (�󱼻�����.)
    Difficult       // ����������, �����
}

public enum E_Event
{
    None,
    Speed,
    Camera,
}

[Serializable]
public struct Chat      // ���� � ���� �ϴ°�
{
    public E_ChatState state;     // ���ϴ� ���� Ÿ��
    public string text;        // �� �ϴ� ��.
    public E_Face face;       // �� �� ���� ǥ��
    public E_Event textEvent;
    public int speed;
}

[Serializable]
public struct AskAndReply
{
    public string ask;        // ���� �� �ִ� ������
    public string[] reply;     // �׿� ���� ����
}

[Serializable]
public struct Round
{
    public string round;
    public string text;
}

[Serializable]
public class Chapters
{
    public string who;     // ������
    public Chat[] chat;         // ê��
    public AskAndReply[] askAndReply;           // ������
    public Round[] round;               // ������ ���ϵ�
    public string[] evidence;           // ����
}

public class ChatStruct : MonoBehaviour { }
