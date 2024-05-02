using System;
using UnityEngine;

namespace ChatVisual
{
    public enum ESaveLocation
    {
        None,
        Assistant,      // ����
        HyeonSeok,      // ����
        JiHyeon,        // ����
        JunWon,         // �ؿ�
    }

    public enum EChatState
    {
        Other = 0,      // �������� ���ϴ� ��
        Me = 1,      // ���簡 ���ϴ� ��
        Ask = 2,        // ���簡 ���� ����.
        LoadNext        // �� �̾��� ��ȭ�� �ִٸ�. �ٸ� ����̸�.
    }

    public enum EFace
    {
        Default,        // ��ǥ��
        Blush,          // ��Ȳ (�󱼻�����.)
        Difficult       // ����������, �����
    }

    public enum EChatEvent
    {
        Vibration,      // �ؽ�Ʈ ����
        Round,      // ���� �߰����ֱ�      
        Camera,     // ī�޶� ȿ�� �־��ֱ�
    }

    [Serializable]
    public class Chat      // ���� � ���� �ϴ°�
    {
        public EChatState state;     // ���ϴ� ���� Ÿ��
        public string text;        // �� �ϴ� ��.
        public EFace face;       // �� �� ���� ǥ��
        public EChatEvent[] textEvent;
    }

    [Serializable]
    public struct AskAndReply
    {
        public string ask;        // ���� �� �ִ� ������
        public Chat[] reply;     // �׿� ���� ����
        public bool is_UseThis;     // ����ߴ���
    }

    [Serializable]
    public struct LockAskAndReply
    {
        public string[] evidence;
        public string ask;        // ���� �� �ִ� ������
        public Chat[] reply;     // �׿� ���� ����
        public bool is_UseThis;     // ����ߴ���
    }

    [Serializable]
    public class Chapters
    {
        public string showName;     // ������ �̸�
        public ESaveLocation saveLocation;     // ������
        public Chat[] chat;         // ê��
        public AskAndReply[] askAndReply;           // ������
        public LockAskAndReply[] lockAskAndReply;       // ��� ������
        public string[] round;           // ���ŷ� ���Ǿ�
    }

    public class ChatStruct : MonoBehaviour { }
}