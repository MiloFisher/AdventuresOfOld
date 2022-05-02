using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIDropDownOptionHide : MonoBehaviour
{
    // Start is called before the first frame update
    //Cheese script to make the dropdown option "Not selected" HIDDEN
    void Start()
    {
        Toggle toggle = gameObject.GetComponent<Toggle>();
        if (toggle != null && toggle.name == "Item 3: Not selected")
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }

}
