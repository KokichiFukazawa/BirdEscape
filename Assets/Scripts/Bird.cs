using BirdEscape;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    // プレイヤーの位置座標
    [SerializeField]
    private Transform  player = null;
    // Start is called before the first frame update
    float speed = 7;
    new Rigidbody2D rigidbody;
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        UpdateTransform();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTransform();
        UpdateBirdSpeed();
    }

    // 鳥の位置
    private void UpdateTransform()
    {
        var position = transform.position;
        position.y = player.position.y;
        transform.position = position;

    }

    // 鳥のスピード調整
    private void UpdateBirdSpeed()
    {
        var velocity = rigidbody.velocity;
        var position = transform.position;
        // ステージNoを取得して
        switch (StageScene.StageNo)
        {
            case 0:if(position.x < 178)
                {
                    velocity.x = speed;
                    rigidbody.velocity = velocity;
                }
                else if(position.x > 178 && position.x < 356)
                {

                    velocity.x = speed * 1.1f;
                    rigidbody.velocity = velocity;
                }
                else if(position.x > 356)
                {

                    velocity.x = speed * 1.2f;
                    rigidbody.velocity = velocity;
                }
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ゴール判定
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(OnGameOver());
        }
    }

    IEnumerator OnGameOver()
    {
        yield return 0;
        Debug.Log("gameover");
        rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
    }
}
