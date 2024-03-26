using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct AskReply       // �ϳ��� ������ �׿� ���� �亯. �亯�� 2�� �̻� �޼����� ��� \�̰ɷ� ������ ǥ���Ѵ�.
{
    public string ask;        // ���� �� �ִ� ��������
    public string reply;     // �׿� ���� ����
    public bool is_used;

    public string[] GetReplys()
    {
        return reply.Split('\\');
    }
}

[CreateAssetMenu(fileName = "AskAndReplySO", menuName = "SO/AskAndReplySO")]           // �̰� ���� ��ũ��Ʈ �ļ� ������ֱ�
public class AskAndReplySO : ScriptableObject
{
    public string askName;      // ���� ���� �̸�
    public AskReply ask;     // ���� ��
}
