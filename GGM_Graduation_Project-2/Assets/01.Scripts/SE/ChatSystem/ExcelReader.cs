using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ExcelReader : MonoBehaviour
{
    private const string address = "https://docs.google.com/spreadsheets/d/1jBb3gjagzX7KhxNcG_g8yCLGGm0PDfHa";
    public string locations = "B3";     // ä�õ��� ��ġ�� ��Ƶ� ��.
    public const long id = 562033554; 
    private int nowReadLine = 0;        // ���� ���� �а� �ִ� ��
    private int nowAskIndex = 0;        // ���� ���� �������ִ� ���� �ε���

    private Chat[] chat;            // ä�õ�
    private AskAndReply[] askAndReplySO;   // ������
    private Round[] round;

    private void Start()
    {
        StartCoroutine(LoadData());
    }

    private IEnumerator LoadData()
    {
        UnityWebRequest www = UnityWebRequest.Get(GetTSVAddress(locations, id));        /// ä�õ��� �ִ� �� ��������
        yield return www.SendWebRequest();

        string[] chatsLocation = www.downloadHandler.text.Split('\n');       // �µ��� ��ġ���� �� ���� �� ������ �����ֱ�
        ChattingManager.Instance.Chapters = new Chapters[chatsLocation.Length];      // �� ��ȭ�� ����

        for (int i = 0; i < chatsLocation.Length; i++)
        {
            UnityWebRequest www2 = UnityWebRequest.Get(GetTSVAddress(chatsLocation[i], id));        // �ϳ��� ���ÿ��� ��� ���� �������ֱ�
            yield return www2.SendWebRequest();

            string[] lineCut = www2.downloadHandler.text.Split("\n");       // �� ���� �� ������ �����ֱ�

            ChattingManager.Instance.Chapters[i].who = lineCut[0].Split('\t')[0];        // �̸� �־��ֱ�

            // ������ �ؽ�Ʈ�� ũ�⸦ ��������
            string[] chatSize = lineCut[2].Split('\t');
            if (int.TryParse(chatSize[0], out int askCount))
            {
                askAndReplySO = new AskAndReply[askCount];
            }
            else { Debug.LogError("���� ���� ���� ����"); }

            if (int.TryParse(chatSize[1], out int chatCount))
            {
                chat = new Chat[chatCount + askCount];
            }
            else { Debug.LogError("��ȭ ���� ���� ����"); }

            for (int j = 4; j < chat.Length + 4; j++)         // ��ȭ �־��ֱ�
            {
                string[] chatAndState = lineCut[j + nowReadLine].Split('\t');
                chat[j - 4].text = chatAndState[1];       // �ý�Ʈ �־���.

                ChatState state = (ChatState)Enum.Parse(typeof(ChatState), chatAndState[0]);
                if (state == ChatState.Ask)
                {
                    chat[j - 4].state = ChatState.Ask;            // �����̶�� Ÿ���� �־���.
                    askAndReplySO[nowAskIndex].ask = chatAndState[1];       // �����ʿ� �ؽ�Ʈ�� �־���. ���߿� �̰ɷ� ã�Ƽ� ����� ���ٰ���. ��ųʸ��� ����? ��Ʈ���� �迭��Ʈ������?
                    if (int.TryParse(lineCut[j + nowReadLine + 1].Split('\t')[1], out int replyCount))      // ������ ���� ����� ����
                    {
                        nowReadLine += 2;       // ���� ���� ����
                        askAndReplySO[nowAskIndex].reply = new string[replyCount];
                        for (int k = 0; k < replyCount; k++)
                        {
                            askAndReplySO[nowAskIndex].reply[k] = lineCut[j + nowReadLine].Split('\t')[1];
                            nowReadLine++;
                        }
                        nowReadLine--;
                        nowAskIndex++;
                    }
                }
                else
                {
                    chat[j - 4].state = state;
                }
            }

            int roundStartLine = nowReadLine + chat.Length + 5;        // Round ���� ����.
            round = new Round[lineCut.Length - roundStartLine];
            int roundCount = 0;
            for (int j = roundStartLine; j < lineCut.Length; j++)
            {
                round[roundCount].round = lineCut[j].Split('\t')[0];
                round[roundCount].text = lineCut[j].Split('\t')[1];
                roundCount++;
            }

            // é�� ������ֱ�
            ChattingManager.Instance.Chapters[i].chat = chat;
            ChattingManager.Instance.Chapters[i].askAndReply = askAndReplySO;
            ChattingManager.Instance.Chapters[i].round = round;
            
            // �ʱ�ȭ
            nowAskIndex = 0;
            nowReadLine = 0;
        }

        // ���� �ý��� ���ֱ�
        ChattingManager.Instance.StartChatting(0);
    }

    public string GetTSVAddress(string range, long sheetID)
    {
        return $"{address}/export?format=tsv&range={range}&gid={sheetID}";
    }

}
