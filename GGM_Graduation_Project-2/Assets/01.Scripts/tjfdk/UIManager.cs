using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] List<GameObject> panels;
    [SerializeField] GameObject alarmIcon;

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
