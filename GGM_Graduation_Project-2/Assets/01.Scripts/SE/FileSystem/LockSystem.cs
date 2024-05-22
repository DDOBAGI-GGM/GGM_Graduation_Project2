using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LockSystem : MonoBehaviour
{
    public TMP_Text guidingText;        // ��(��) ����ֽ��ϴ�. ������.
    public TMP_InputField inputField;       // ��ǲ�ʵ�
    private string password;     // ���
    private Image lockImage;

    private void OnEnable()
    {
        inputField.text = "";
        guidingText.color = Color.black;
        guidingText.text = $"\'\'��(��) ����ֽ��ϴ�.";
    }

    public void Init(string fileName, string password, Image image)
    {
        guidingText.text = $"\'{fileName}\'��(��) ����ֽ��ϴ�.";
        this.password = password;
        lockImage = image;
    }

    public void OkBtn()
    {
        if (password == inputField.text)
        {
            //Debug.Log("����");
            //lockImage.gameObject.transform.parent.GetComponent<Folder>().is_Lock = false;
            //lockImage.gameObject.SetActive(false);
            //this.gameObject.SetActive(false);
        }
        else
        {
            guidingText.color = Color.red;
            guidingText.text = "��ȣ�� �߸��Ǿ����ϴ�.";
        }
    }
}
