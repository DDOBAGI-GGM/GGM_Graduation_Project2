using System;
using System.Collections.Generic;
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
        None = 0,
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
        public List<EChatEvent> textEvent = new List<EChatEvent>();
    }

    [Serializable]
    public class AskAndReply
    {
        public string ask;        // ���� �� �ִ� ������
        public List<Chat> reply = new List<Chat>();     // �׿� ���� ����
        public bool is_UseThis;     // ����ߴ���
    }

    [Serializable]
    public class LockAskAndReply
    {
        public List<string> evidence = new List<string>();
        public string ask;        // ���� �� �ִ� ������
        public List<Chat> reply = new List<Chat>();     // �׿� ���� ����
        public bool is_UseThis;     // ����ߴ���
    }

    [Serializable]
    public class Chapter
    {
        public string showName;     // ������ �̸�
        public ESaveLocation saveLocation;     // ������
        public List<Chat> chat = new List<Chat>();         // ê��
        public List<AskAndReply> askAndReply = new List<AskAndReply>();           // ������
        public List<LockAskAndReply> lockAskAndReply = new List<LockAskAndReply>();       // ��� ������
        public List<string> round = new List<string>();           // ���ŷ� ���Ǿ�
    }

    public class ChatStruct : MonoBehaviour { }
}