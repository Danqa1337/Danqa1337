using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VersionTexst : MonoBehaviour
{

    private void Start()
    {
       GetComponent<TextMeshProUGUI>().text = "V "+ Application.version;
    }
}
