using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SuspectPanel : MonoBehaviour
{
    // ������ SO �ް�...
    // ���� ��Ű�� ��...
    [Header("Profile")]
    [SerializeField] private Image face;
    [SerializeField] private TextMeshProUGUI name;
    [SerializeField] private TextMeshProUGUI feature;
    [SerializeField] private Slider doubtValue;

    //[SerializeField] private string nickName;
    //[SerializeField] private string feature;
    //[SerializeField] private Sprite image;
    //[SerializeField] private int doubtLevel;
    //private Slider slider;

    //private void Awake()
    //{
    //    slider = GetComponent<Slider>();
    //}

    //public void ChangeName(string _name)
    //{
    //    name = _name;
    //    slider.GetComponentInChildren<TextMeshProUGUI>().text =
    //        $"�̸� : {name} \nƯ¡ : {feature}";
    //}

    //public void ChangeFeature(string _feature)
    //{
    //    feature = _feature;
    //    slider.GetComponentInChildren<TextMeshProUGUI>().text =
    //        $"�̸� : {name} \nƯ¡ : {feature}";
    //}

    //public void ChangeDoubt(int _doubt)
    //{
    //    doubtLevel = _doubt;
    //    slider.GetComponentInChildren<Slider>().value = doubtLevel;
    //}
}
