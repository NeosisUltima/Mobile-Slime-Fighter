using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfo : MonoBehaviour
{
    //Needs to COnnect to INventory System
    [SerializeField]
    private TextMeshProUGUI itmName, itmDesc, Amount;

    [SerializeField]
    private Button useButton,addButton,subButton;

    private int num = 0;
    private int AtkBoost, DefBoost, SpdBoost;

    private Items thisItem;

    private PlayerInfo pi;

    void Awake()
    {
        pi = GameObject.Find("PlayerInfo").GetComponent<PlayerInfo>();
        useButton.interactable = false;

        AtkBoost = thisItem.iteminfo["Attack"];
        DefBoost = thisItem.iteminfo["Defense"];
        SpdBoost = thisItem.iteminfo["Speed"];

    }

    // Update is called once per frame
    void Update()
    {
        if (num > 0) useButton.interactable = true;
        else useButton.interactable = false;
    }

    //Add 1 to the item use
    public void AddAmount()
    {
        num++;
        Amount.text = num.ToString();
    }

    //Subtract 1 to item use
    public void SubtractAmount()
    {
        if (num >= 0) { num--; subButton.interactable = true; }
        else subButton.interactable = false;
        Amount.text = num.ToString();
    }

    public void UseItem()
    {
        pi.mySlime.setAtk(pi.mySlime.getAtk() + (AtkBoost * num));
        pi.mySlime.setAtk(pi.mySlime.getDef() + (DefBoost * num));
        pi.mySlime.setAtk(pi.mySlime.getSpd() + (SpdBoost * num));
        
        num = 0;
        
        useButton.interactable = false;

    }
}
