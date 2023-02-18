using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerKillController : NetworkBehaviour
{
    public int health = 100;
    int killCount = 0;

    void Start() { }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            // raycast forward
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, 100))
            {
                print("Hit: " + hit.collider.gameObject.name);
                if (hit.collider.gameObject.tag == "Player")
                {
                    PlayerKillController hitPlayer =
                        hit.collider.gameObject.GetComponent<PlayerKillController>();
                    hitPlayer.OnHit();

                    if (hitPlayer.health <= 0)
                    {
                        OnKill();
                    }
                }
            }
        }
    }

    public void OnHit()
    {
        health -= 10;
        print("Player hit! Health: " + health);

        if (health <= 0)
        {
            health = 100;
            GetComponent<PlayerMovement>().Respawn();
        }
    }

    void OnKill()
    {
        killCount++;
        GameManager.globalKillCount++;
    }
}
