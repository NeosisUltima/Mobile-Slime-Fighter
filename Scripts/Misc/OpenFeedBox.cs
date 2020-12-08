using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenFeedBox : MonoBehaviour
{
    private PlayerInfo pi;

    private void Start()
    {
        pi = GameObject.Find("PlayerInfo").GetComponent<PlayerInfo>();
    }

    public void OpenBox() {this.gameObject.SetActive(true); }

    public void CloseBox() { this.gameObject.SetActive(false); }
}
