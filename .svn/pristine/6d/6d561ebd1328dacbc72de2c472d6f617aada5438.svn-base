using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleVisible : MonoBehaviour
{
    public GameObject[] Object;

    public void ToggleTrails()
    {
        Toggle tempstate = this.GetComponent<Toggle>();
        foreach (GameObject entity in Object)
        {
            TrailRenderer temp = entity.GetComponent<TrailRenderer>();
            temp.enabled = tempstate.isOn;
        }

    }

    public void ToggleObject()
    {
        Toggle temp = this.GetComponent<Toggle>();
        foreach (GameObject entity in Object)
        {
            entity.SetActive(temp.isOn);
        }
    }
}
