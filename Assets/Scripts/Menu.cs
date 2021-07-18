using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
public class Menu : MonoBehaviourPunCallbacks
{
    public GameObject mainScreen;
    public GameObject lobbyScreen;

    public Button CreateRoomButton;
    public Button JoinRoomButton;

    public TextMeshProUGUI playerListText;
    public TMP_InputField roomNameInput;
    public TMP_InputField playerNameInput;
    public Button startGameButton;

     void Start()
    {
        CreateRoomButton.interactable = false;
        JoinRoomButton.interactable = false;
    }
    public override void OnConnectedToMaster()
    {
        CreateRoomButton.interactable = true;
        JoinRoomButton.interactable = true;
    }

    void SetScreen(GameObject screen)
    {
        mainScreen.SetActive(false);
        lobbyScreen.SetActive(false);

        screen.SetActive(true);
    } 

    public void OnCreateRoomName()
    {
        NetworkManager.instance.CreateRoom(roomNameInput.text);
        Debug.Log("Room Created: "+roomNameInput.text);
    }

    public void OnJoinRoomButton()
    {
        NetworkManager.instance.JoinRoom(roomNameInput.text);
    }

    public void OnPlayerNameUpdate()
    {
        PhotonNetwork.NickName = playerNameInput.text;
    }

    public override void OnJoinedRoom()
    {
        SetScreen(lobbyScreen);
        photonView.RPC("UpdateLobbyUI",RpcTarget.All);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateLobbyUI();
    }
    [PunRPC]
    public void UpdateLobbyUI()
    {
        playerListText.text = "";

        foreach(Player player in PhotonNetwork.PlayerList)
        {
            playerListText.text += player.NickName + "\n";
            Debug.Log("Player name: " + player.NickName);
        }

        if (PhotonNetwork.IsMasterClient)
        {
            startGameButton.interactable = true;
        }
        else
        {
            startGameButton.interactable = false;
        }
    }

    public void OnLeaveLobbyButton()
    {
        PhotonNetwork.LeaveRoom();
        SetScreen(mainScreen);
    }

    public void OnStartGameButton()
    {
        NetworkManager.instance.photonView.RPC("ChangeScene",RpcTarget.All,"Game");
    }
}
