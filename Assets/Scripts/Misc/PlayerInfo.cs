﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviourPunCallbacks
{
    //Save and Load Information
    public bool UserNameEntered, SlimeEggTapped, SlimeHatched,joinedRoom, slimeDied;
    public bool LoggedInFirstTime = false;
    public int wins, losts, joinNumber;
    public string pName = "";
    [SerializeField]
    public Slime mySlime = null;

    private float timer = 5.0f;

    //QuestSaver for PlayerInfo
    public int QuestTimer = 0;
    public Quest myCurrentQuest;
    public bool questActive;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (slimeDied)
        {
            SlimeEggTapped = false;
            SlimeHatched = false;
        }

        if (mySlime.SlimeWins == 15) mySlime.Evolve();

        if(LoggedInFirstTime) timer -= Time.deltaTime;
        if (timer < 0f) { SaveSystem.SavePlayer(this,this.gameObject.GetComponent<MyQuestSaver>()); timer = 5.0f; Debug.Log("saving"); }
    }

    

    public void SetPName(string PNAME)
    {
        pName = PNAME;
        UserNameEntered = true;
    }

    public void LoadPlayer()
    {

        PlayerData data = SaveSystem.LoadPlayer();

        if (data != null)
        {
            pName = data.pName;
            UserNameEntered = data.UserNameEntered;
            SlimeEggTapped = data.SlimeEggTapped;
            SlimeHatched = data.SlimeHatched;

            wins = data.wins;
            losts = data.losts;

            mySlime = data.mySlime;

            //My Quest Saver
            MyQuestSaver mqs = gameObject.GetComponent<MyQuestSaver>();

            mqs.thisQuest = data.thisQuest;
            mqs.timer = data.timer;
            mqs.timerActivated = data.timerActivated;
            mqs.leftAdventure = data.leftAdventure;
            mqs.inQuest = data.inQuest;
        }
        else
        {
            Debug.Log("No SaveFile Found: Making Changes");
        }
    }

    
}
