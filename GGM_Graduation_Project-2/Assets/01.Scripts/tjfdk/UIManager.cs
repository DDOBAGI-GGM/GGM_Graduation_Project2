using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public List<GameObject> panels;
    public GameObject alarmIcon;

    public Action<int> startChatEvent;
    public int chatIndex = 0;

    private void Awake()
    {
        Instance = this;
    }

    public void test()
    {
        alarmIcon.SetActive(!alarmIcon.activeSelf);
    }

    public void Panle_OnOff(GameObject panel)       // ����â���� �����.
    {
        panel.SetActive(!panel.activeSelf);
    }

    public void Panel_Popup(GameObject panel)
    {
        if (panel.activeSelf == false)
        {
            foreach (GameObject obj in panels)
                obj.SetActive(false);       // ���� ���ֱ�
            panel.SetActive(true);      // ������ ���ֱ�
            if (panel.gameObject.name == panels[0].gameObject.name && startChatEvent == null)         // ���� ��ȭâ�̰� �׼��� ������ �׼� ȣ�� �� ���� ����
            {
                startChatEvent.Invoke(chatIndex);
                startChatEvent -= (index) => ChattingManager.Instance.StartChatting(index);
            }
        }
    }

    public void SceneChange(string _sceneName)
    {
        SceneManager.LoadScene(_sceneName);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
