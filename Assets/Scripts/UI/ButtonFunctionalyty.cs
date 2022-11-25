using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonFunctionalyty : MonoBehaviour
{
    GameObject text;

    private void OnEnable()
    {
        text = GetComponentInChildren<Text>().gameObject;
        text.SetActive(false);
    }
    public void OnClick()
    {
        text.SetActive(!text.activeInHierarchy);
    }
}
