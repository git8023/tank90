using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HQ : MonoBehaviour
{

    [Header("摧毁图片")]
    public Sprite brokenSprite;

    [Header("爆炸特效")]
    public GameObject explosionPrefab;

    [Header("爆炸音效")]
    public AudioClip explosionAudio;

    [Header("音效播放器")]
    public AudioSource audioSource;

    // 图片渲染器
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // 总部被摧毁, 游戏结束
    public void Died()
    {
        Instantiate(explosionPrefab, transform.position, transform.rotation);
        spriteRenderer.sprite = brokenSprite;

        // 播放音效
        audioSource.clip = explosionAudio;
        audioSource.Play();

        // 标记游戏结束
        PlayerManager.Instance.isDefeat = true;
    }

}
