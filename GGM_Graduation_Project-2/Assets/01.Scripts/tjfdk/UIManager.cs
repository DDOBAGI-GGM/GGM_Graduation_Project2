using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    [SerializeField] List<GameObject> panels;

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
}
