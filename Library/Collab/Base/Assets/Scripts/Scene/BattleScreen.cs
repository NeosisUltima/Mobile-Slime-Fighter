using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;



public class BattleScreen : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    /*BattleScreen Script
     * 
     * -Should allow connectivity for rock paper scissors
     * -sends info back to player for whether or not they won or lost
     * -if player did lose, should refer back to their playerinfo script for reincarnation function
     * -if player won, should boost stats of the player's slime
     */

    [SerializeField]
    private GameObject p1SlimePanel, p2SlimePanel,DisconnectPanel;
    [SerializeField]
    private TextMeshProUGUI infoText;
    [SerializeField]
    private CanvasGroup buttons;
    private BattlePanel p1, p2;
    private bool p1PanelSet, p2PanelSet = false;
    private bool p1MoveMade, p2MoveMade;
    public int turn = 0;
    private int prevHealth;
    public enum Results { None = 0, Draw, Win, Loss}

    private PhotonView pv;

    public MultiplayerSetupController msc;
    private PlayerInfo pi;
    //Events
    public const byte SetPlayerOverlayEvent = 1;

    void Awake()
    {
        p1 = p1SlimePanel.GetComponent<BattlePanel>();
        p2 = p2SlimePanel.GetComponent<BattlePanel>();
        pv = GetComponent<PhotonView>();
        //msc = GameObject.Find()
        pv.RPC("SetBattleOverlay",RpcTarget.All);
        prevHealth = (int)PhotonNetwork.LocalPlayer.CustomProperties["PlayerSlimeHealth"];
        pi = GameObject.Find("PlayerInfo").GetComponent<PlayerInfo>();
        Debug.Log(PhotonNetwork.LocalPlayer.ActorNumber);

        if (PhotonNetwork.LocalPlayer.ActorNumber == 1) p2.MyCurrentHealthText.gameObject.SetActive(false);
        else p1.MyCurrentHealthText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        if (this.theirMove != -1) print(this.theirMove);
        if (this.myMove != -1 && this.theirMove != -1)
        {
            FindWinner();

            if(this.myResult == Results.Win)
            {
                pv.RPC("Attack",RpcTarget.All,PhotonNetwork.LocalPlayer, PhotonNetwork.LocalPlayer.GetNext());
            }

            buttons.interactable = true;
            
        }

        if((int)PhotonNetwork.LocalPlayer.CustomProperties["PlayerSlimeHealth"] != prevHealth)
        {
            Debug.Log("Health has changed for this player.... was " + prevHealth + ", now it's " + (int)PhotonNetwork.LocalPlayer.CustomProperties["PlayerSlimeHealth"]);
            prevHealth = (int)PhotonNetwork.LocalPlayer.CustomProperties["PlayerSlimeHealth"];
        }
    }

   

    //ROCK PAPER SCISSORS
    private static int ROCK = 0;
    private static int PAPER = 1;
    private static int SCISSORS = 2;
    private static int NONE = -1;

    [SerializeField]
    private int myMove = -1; 
    [SerializeField]
    private int theirMove = -1;
    private Results myResult;

    public const byte  MoveMadeEvent = 2;

    public void ChooseRock()
    {
        this.MakeMove(ROCK);
    }


    public void ChoosePaper()
    {
        this.MakeMove(PAPER);
    }


    public void ChooseScissors()
    {
        this.MakeMove(SCISSORS);
    }

    public void MakeMove(int selection)
    {
        this.myMove = selection;
        p1MoveMade = true;
        buttons.interactable = false;
        pv.RPC("SendMove", RpcTarget.Others, this.myMove);
    }

    [PunRPC]
    public void SendMove(int choice)
    {
        if (this.theirMove == -1)
        {
            this.theirMove = choice;
            p2MoveMade = true;
        }
    }

    

    public void FindWinner()
    {
        Debug.Log("Determining Winner");
        this.myResult = Results.Draw;
        if (this.myMove == this.theirMove) return;

        if(this.myMove == NONE)
        {
            this.myResult = Results.Loss;
            return;
        }

        if(this.theirMove == NONE)
        {
            this.myResult = Results.Win;
            return;
        }

        if (this.myMove == ROCK) this.myResult = (this.theirMove == SCISSORS) ? Results.Win : Results.Loss;
        if (this.myMove == PAPER) this.myResult = (this.theirMove == ROCK) ? Results.Win : Results.Loss;
        if (this.myMove == SCISSORS) this.myResult = (this.theirMove == PAPER) ? Results.Win : Results.Loss;
        
        this.myMove = -1;
        this.theirMove = -1;
    }

    //IENumerators
    public IEnumerator ShowResults()
    {
        if (this.myResult == Results.Draw) infoText.text = "Draw, but the faster slime snuck a light attack in.";
        else infoText.text = this.myResult == Results.Win ? "You attack the oponent." : "You have been attacked.";
        yield return new WaitForSeconds(5.0f);
        
    }

    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;

    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if(eventCode == MoveMadeEvent)
        {
            object[] data = (object[])photonEvent.CustomData;
            this.theirMove = (int)data[0];
        }
    }

    [PunRPC]
    public void UpdateScene()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.NickName == p1.myPlayeerName)
            {
                p1.myCurrentSlimeHealth = (int)PhotonNetwork.LocalPlayer.CustomProperties["PlayerSlimeHealth"];
                p1.MyCurrentHealthText.text = p1.myCurrentSlimeHealth + "/100";
            }
            else if (player.NickName == p2.myPlayeerName)
            {
                p2.myCurrentSlimeHealth = (int)PhotonNetwork.LocalPlayer.CustomProperties["PlayerSlimeHealth"];
                p2.MyCurrentHealthText.text = p1.myCurrentSlimeHealth + "/100";
            }
        }
        p1.UpdateHealthColor();
        p2.UpdateHealthColor();
    }


    //PUN References
    [PunRPC]
    public void SetBattleOverlay()
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            int myNumber = p.ActorNumber;

            if (myNumber == 1)
            {
                this.p1.myPlayeerName = p.NickName;
                this.p1.Name.text = (string)p.CustomProperties["PlayerSlimeName"];
                this.p1.myCurrentSlimeHealth = (int)p.CustomProperties["PlayerSlimeHealth"];
                this.p1.SlimeImage.sprite = p1.ssl[(int)p.CustomProperties["PlayerSlimeEvolStage"] - 1].SlimeStage[(int)p.CustomProperties["PlayerSlimeElement"]];
                this.p1.MyCurrentHealthText.text = p1.myCurrentSlimeHealth + "/100";
                p1PanelSet = true;
            }
            else if (myNumber == 2)
            {
                this.p2.myPlayeerName = p.NickName;
                this.p2.Name.text = (string)p.CustomProperties["PlayerSlimeName"];
                this.p2.myCurrentSlimeHealth = (int)p.CustomProperties["PlayerSlimeHealth"];
                this.p2.SlimeImage.sprite = p2.ssl[(int)p.CustomProperties["PlayerSlimeEvolStage"] - 1].SlimeStage[(int)p.CustomProperties["PlayerSlimeElement"]];
                this.p2.MyCurrentHealthText.text = p2.myCurrentSlimeHealth + "/100";
            }
        }
    }
    
    [PunRPC]
    public void Attack(Player a, Player b)
    {
        Hashtable temp = new Hashtable();
        float modifier = 1;
        Debug.Log(a.CustomProperties["PlayerSlimeName"] + " attacked " + b.CustomProperties["PlayerSlimeName"] + "!");

        if ((int)a.CustomProperties["PlayerSlimeElement"] == (int)a.CustomProperties["PlayerSlimeElement"]) modifier = 1;
        if ((int)a.CustomProperties["PlayerSlimeElement"] == (int)a.CustomProperties["PlayerSlimeStrElement"]) modifier = 0.5f;
        if ((int)a.CustomProperties["PlayerSlimeElement"] == (int)a.CustomProperties["PlayerSlimeWkElement"]) modifier = 2;
        else modifier = 1;

        int dmg = (int)(((int)a.CustomProperties["PlayerSlimeAttack"] * 2 - (((int)b.CustomProperties["PlayerSlimeDefense"]) * 3)/2)*modifier);
        if (dmg <= 0) dmg = 1;

        int curhlth = (int)b.CustomProperties["PlayerSlimeHealth"] - dmg;
        temp.Add("PlayerSlimeHealth", curhlth);
        b.SetCustomProperties(temp);
        Debug.Log(b.CustomProperties["PlayerSlimeName"] + " currently has " + b.CustomProperties["PlayerSlimeHealth"] + "!");
        pv.RPC("UpdateScene", RpcTarget.All);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.CurrentRoom.RemovedFromList = true;
        DisconnectPanel.SetActive(true);
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        LeavingRoom();
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        LeavingRoom();
    }

    public void LeavingRoom()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.CurrentRoom.RemovedFromList = true;
        DisconnectPanel.SetActive(true);
    }

    public void ReturnToLobby()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LeaveLobby();
        StartCoroutine(rejoinLobby());
        SceneManager.LoadScene("Fight");
    }

    IEnumerator rejoinLobby()
    {
        yield return new WaitForSeconds(1);
        PhotonNetwork.JoinLobby();
    }
}
