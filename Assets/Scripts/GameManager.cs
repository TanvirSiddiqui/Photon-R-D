using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using System;


public class GameManager : MonoBehaviourPunCallbacks
{
    public bool gameEnded = false;
    public float timeToWin;
    public float invinsibleDuration;
    private float hatPickUpTime;

    public string playerPrefabLocation;
    public Transform[] spawnPoints;
    public PlayerController[] players;
    public int playerWithHat;
    private int playerInGame;

    //instance
    public static GameManager instance;

     void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        players = new PlayerController[PhotonNetwork.PlayerList.Length];
        photonView.RPC("ImInGame", RpcTarget.AllBuffered);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    [PunRPC]
    void ImInGame()
    {
        playerInGame++;
        if (playerInGame == PhotonNetwork.PlayerList.Length)
        {
            SpawnPlayer();
        }
    }

    private void SpawnPlayer()
    {
        //instansiate the player accross the network
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLocation, spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)].position,Quaternion.identity);

        //get the player script
        PlayerController playerScript = playerObj.GetComponent<PlayerController>();
        playerScript.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }

    public PlayerController GetPlayer(int playerId)
    {
        return players.First(x=>x.id==playerId);
    }

    public PlayerController GetPlayer(GameObject playerObj)
    {
        return players.First(x => x.gameObject == playerObj);
    }

    
    [PunRPC]
    public void GiveHat(int playerId, bool initialGive)
    {
        //remove the hat from the currently hatted player
        if (!initialGive)
            GetPlayer(playerWithHat).SetHat(false);

        //give the hat to the new player
        playerWithHat = playerId;
        GetPlayer(playerId).SetHat(true);
        hatPickUpTime = Time.time;
    }

    public bool CanGethat()
    {
        if(Time.time > hatPickUpTime + invinsibleDuration)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    [PunRPC]
    public void WinGame(int playerId)
    {
        gameEnded = true;
        PlayerController player = GetPlayer(playerId);
        GameUI.instance.SetWinText(player.photonPlayer.NickName);

        Invoke("GoBackToMenu", 3.0f);
    }

    void GoBackToMenu()
    {
        PhotonNetwork.LeaveRoom();
        NetworkManager.instance.ChangeScene("Menu");
    }
}
