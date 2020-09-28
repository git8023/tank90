using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{

    [Header("子弹击中音效")]
    public AudioClip hitAudio;

    [Header("音效播放器")]
    public AudioSource audioSource;

    // 播放子弹击中音效
    public void playHitAudio()
    {
        audioSource.clip = hitAudio;
        audioSource.Play();
    }

}
