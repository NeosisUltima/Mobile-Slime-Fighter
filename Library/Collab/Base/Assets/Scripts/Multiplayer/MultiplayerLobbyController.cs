using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;



public class MultiplayerLobbyController : MonoBehaviourPunCallbacks, ILobbyCallbacks
{
    public static MultiplayerLobbyController mlc;
    public GameObject quickMatchBtn, cancelBtn;

    [SerializeField]
    private int RoomSize;

    private List<RoomInfo> RoomListings;
    [SerializeField]
    private GameObject roomListingPrefab;

    private PlayerInfo playerInfo,myInfo;
    private string roomName;
    public Transform roomsContainer;

    private Hashtable myCustomInfo = new Hashtable();

    private void Awake()
    {
        mlc = this;
        myInfo = GameObject.Find("PlayerInfo").GetComponent<PlayerInfo>();
        
        //myCustomInfo.["PlayerInfo"] =  myInfo;
    }

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.LocalPlayer.NickName = myInfo.pName;
    }

    // Update is called once per frame


    public void QuickStart()
    {
        quickMatchBtn.SetActive(false);
        cancelBtn.SetActive(true);
        PhotonNetwork.JoinRandomRoom();
        Debug.Log("Finding Opponents");
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        RoomListings = new List<RoomInfo>();
        //PhotonNetwork.NickName = playerInfo.pName;
        quickMatchBtn.SetActive(true);
    }

    public void JoinLobbyOnClick()
    {
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
            PhotonNetwork.LocalPlayer.CustomProperties = myCustomInfo;
            quickMatchBtn.SetActive(true);
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        RemoveRoomListings();
        foreach (RoomInfo room in roomList)
        {
            print(room.Name);
            ListRoom(room);
        }
    }

    void RemoveRoomListings()
    {
        while(roomsContainer.childCount != 0)
        {
            Destroy(roomsContainer.GetChild(0).gameObject);
        }
    }

    void ListRoom(RoomInfo room)
    {
        if(room.IsOpen == true && room.IsVisible == true)
        {
            GameObject tempListing = Instantiate(roomListingPrefab, roomsContainer);
            RoomButton tempButton = tempListing.GetComponent<RoomButton>();
            tempButton.SetRoom(room.Name, myInfo.mySlime.getSlimeImage());
        }
    }

    public void OnRoomNameChanged(string nameIn)
    {
        roomName = nameIn;
    }

    public void OnRoomSizeChanged(string sizeIn)
    {
        RoomSize = int.Parse(sizeIn);
    }

    static System.Predicate<RoomInfo> ByName(string name)
    {
        return delegate (RoomInfo room)
        {
            return room.Name == name;
        };
    }

    public void CreateRoom()
    {
        //playerInfo = (PlayerInfo) PhotonNetwork.LocalPlayer.CustomProperties["PlayerInfo"];
        Debug.Log("Creating Room");
        //int randomRoomNumber = Random.Range(0, 10000); //creaing a random name for the room
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)MultiplayerSetupController.msc.maxPlayers};
        PhotonNetwork.CreateRoom("Room: " + roomName, roomOps);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Room Name is taken... trying again");
        //CreateRoom();
    }

    public void MatchmakingCancel()
    {
        quickMatchBtn.SetActive(true);
        cancelBtn.SetActive(false);
        PhotonNetwork.LeaveRoom();
    }



}
