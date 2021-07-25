using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    public int id;

    public float moveSpeed;
    public float jumpForce;
    public GameObject hat;

    public float currentHatTime;

    public Rigidbody rig;
    public Player photonPlayer;
    [PunRPC]
   public void Initialize(Player player)
    {
        photonPlayer = player;
        id = player.ActorNumber;


        GameManager.instance.players[id - 1] = this;

        // give first player the hat
        if (id == 1)
            GameManager.instance.GiveHat(id, true);

        if (!photonView.IsMine)
            rig.isKinematic = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if(currentHatTime>= GameManager.instance.timeToWin && !GameManager.instance.gameEnded)
            {
                GameManager.instance.gameEnded = true;
                GameManager.instance.photonView.RPC("WinGame", RpcTarget.All, id);
            }
        }

        if (photonView.IsMine)
        {
            Move();
            if (Input.GetKeyDown(KeyCode.Space))
            {
                TryJump();
            }

            if (hat.activeInHierarchy)
            {
                currentHatTime += Time.deltaTime;
            }
        }
    }

     public void Move()
    {
        Debug.Log("Start moving");
        float x = Input.GetAxis("Horizontal") * moveSpeed;
        float z = Input.GetAxis("Vertical") * moveSpeed;

        rig.velocity = new Vector3(x, rig.velocity.y, z);
    }

    public void TryJump()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        if(Physics.Raycast(ray, 0.7f))
        {
            rig.AddForce(Vector3.up * jumpForce*Time.deltaTime, ForceMode.Impulse);
        }
    }

    public void SetHat(bool hasHat)
    {
        hat.SetActive(hasHat);
    }

     void OnCollisionEnter(Collision collision)
    {
        if (!photonView.IsMine)
        {
            return;
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            // do they have the hat?
            if (GameManager.instance.GetPlayer(collision.gameObject).id == GameManager.instance.playerWithHat)
            {
                if (GameManager.instance.CanGethat())
                {
                    GameManager.instance.photonView.RPC("GiveHat", RpcTarget.All, id, false);
                }
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(currentHatTime);
        }
        else if (stream.IsReading) {
            currentHatTime = (float)stream.ReceiveNext();
        }
    }

}
