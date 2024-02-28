using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    private Player player;
    private float footstepTimer;
    private float footstepTimerMax = .1f;


    private void Awake()
    {
        player = GetComponent<Player>();
    }

    void Update()
    {
        footstepTimer -= Time.deltaTime;
        if (footstepTimer < 0f) {
            footstepTimer = footstepTimerMax;

            if (player.IsWalking) {
                SoundManager.Instance.PlayFootstepSound(player.transform.position, 1f);
            }
        }
    }
}
