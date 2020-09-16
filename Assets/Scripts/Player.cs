using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BirdEscape {
    // プレイヤーキャラクターの操作機能を表します。
    public class Player : MonoBehaviour {
        // 通常時の移動速度を指定します。
        [SerializeField]
        private float walkSpeed = 6;
        // ダッシュ時の移動速度を指定します。
        [SerializeField]
        private float runSpeed = 12;
        // 現在の移動速度
        float speed = 6;
        
        // ジャンプ力を指定します。
        [SerializeField]
        private Vector2 jumpForce = new Vector2(0, 200);
        // 壁に側面から衝突した際のノックバック力を指定します。
        [SerializeField]
        private Vector2 knockBackForce = new Vector2(-1, 0);
        // キノコに側面から衝突した際のノックバック力を指定します。
        [SerializeField]
        private Vector2 mushroomForce = new Vector2(-1, 0);
        [SerializeField]
        private LayerMask mashroomLayer = default;
        // 地面判定用の対象レイヤーを指定します。
        [SerializeField]
        private LayerMask groundLayer = default;
        // 着地判定用のコライダーを指定します。
        [SerializeField]
        private BoxCollider2D groundChecker = null;
        // 壁判定用のコライダーを指定します。
        [SerializeField]
        private BoxCollider2D wallChecker = null;


        // プレイヤーの状態を表します。
        enum PlayerState {
            // 通常の走行状態
            Walk,
            // ダッシュ時の走行状態
            Run,
            // 地面から浮いてジャンプ中の状態
            Jump,
        }
        // 現在のプレイヤー状態
        PlayerState currentState = PlayerState.Walk;

        // ダッシュ時のサウンドを指定します。
        [SerializeField]
        private AudioClip soundOnRun = null;

        // 転倒によるゲームオーバーと判定するタイムアウト時間（秒）を指定します。
        [SerializeField]
        float tumbleTimeout = 1.5f;
        // 転倒している累積時間(秒)
        float tumbleTime = 0;

        // コンポーネントを参照する変数
        new Rigidbody2D rigidbody;
        AudioSource audioSource;
        Animator animator;
        // アニメーションのパラメーター
        static readonly int walkTrigger = Animator.StringToHash("Walk Trigger");
        static readonly int runTrigger = Animator.StringToHash("Run Trigger");
        static readonly int jumpTrigger = Animator.StringToHash("Jump Trigger");

        // Startメソッドは最初のフレームの実行前に一度だけ呼び出されます。
        // ゲームで最初に初期化しておきたい処理を記述する。
        void Start() {
            // コンポーネントを取得
            rigidbody = GetComponent<Rigidbody2D>();
            audioSource = GetComponent<AudioSource>();
            animator = GetComponent<Animator>();

            speed = walkSpeed;
        }

        // 毎フレームに一度呼び出される更新処理を記述します。
        void Update() {
            // プレイヤーの状態別に分岐処理
            switch (currentState) {
                case PlayerState.Walk:
                    UpdateForWalkState();
                    break;
                case PlayerState.Run:
                    UpdateForRunState();
                    break;
                case PlayerState.Jump:
                    UpdateForJumpState();
                    break;
                default:
                    break;
            }
        }

        // Walkステートに遷移させます。
        private void SetWalkState() {
            animator.SetTrigger(walkTrigger);
            speed = walkSpeed;
            currentState = PlayerState.Walk;
        }

        // Runステートに遷移させます。
        private void SetRunState() {
            animator.SetTrigger(runTrigger);
            speed = runSpeed;
            // コロコロ音を再生
            audioSource.clip = soundOnRun;
            audioSource.loop = true;
            audioSource.Play();
            currentState = PlayerState.Run;
        }

        // Jumpステートに遷移させます。
        private void SetJumpState() {
            rigidbody.AddForce(jumpForce, ForceMode2D.Impulse);
            animator.SetTrigger(jumpTrigger);
            currentState = PlayerState.Jump;
        }

        // Walkステートの際のフレーム更新処理です。
        private void UpdateForWalkState() {
            // ジャンプ
            if (Input.GetButtonDown("Jump")) {
                SetJumpState();
            }
            // ダッシュ
            else if (Input.GetButtonDown("Fire1")) {
                SetRunState();
            }
            // 等速度運動
            var velocity = rigidbody.velocity;
            velocity.x = speed;
            rigidbody.velocity = velocity;
        }

        // Runステートの際のフレーム更新処理です。
        private void UpdateForRunState() {
            if (Input.GetButtonUp("Fire1")) {
                // コロコロ音を停止
                audioSource.Stop();
                SetWalkState();
            }
            // 等速度運動
            var velocity = rigidbody.velocity;
            velocity.x = speed;
            rigidbody.velocity = velocity;
        }

        // Jumpステートの際のフレーム更新処理です。
        private void UpdateForJumpState() {
            // とくに処理はない
        }

        // 固定フレームレートで呼び出される更新処理を記述します。
        void FixedUpdate() {
            // 地面との交差判定
            Vector2 point = groundChecker.transform.position;
            point += groundChecker.offset;
            var result = Physics2D.OverlapBox(
                point,
                groundChecker.size,
                groundChecker.transform.rotation.eulerAngles.z,
                groundLayer);
            // 接地している場合
            if (result) {
                // 今回着地した場合
                if (currentState == PlayerState.Jump) {
                    // 転倒していない場合
                    if (!IsTumble()) {
                        SetWalkState();
                    }
                }
            }
            // 空中に浮遊している場合
            else {
                // 落下判定
                if (currentState != PlayerState.Jump) {
                    animator.SetTrigger(jumpTrigger);
                    // コロコロ音を停止
                    audioSource.Stop();
                    currentState = PlayerState.Jump;
                }
            }

            // 壁との側面衝突判定
            point = wallChecker.transform.position;
            point += wallChecker.offset;
            result = Physics2D.OverlapBox(
                point,
                wallChecker.size,
                wallChecker.transform.rotation.eulerAngles.z,
                groundLayer);
            if (result) {
                // x軸方向の速度を停止させる
                var velocity = rigidbody.velocity;
                velocity.x = 0;
                rigidbody.velocity = velocity;
                // キノコと側面衝突したとき
                if (wallChecker.CompareTag("mushroom"))
                {
                    if (!IsTumble())
                    {
                        Debug.Log("きのこに衝突");
                        // ノックバックさせる
                        var force = transform.TransformDirection(mushroomForce);
                        rigidbody.AddForce(force, ForceMode2D.Impulse);
                    }
                }
                else
                {
                    if (!IsTumble())
                    {
                        // ノックバックさせる
                        var force = transform.TransformDirection(knockBackForce);
                        rigidbody.AddForce(force, ForceMode2D.Impulse);
                    }
                }
            }


            // 転倒状態を判定
            if (IsTumble()) {
                // 転倒状態がキープされている時間を計測する
                tumbleTime += Time.deltaTime;
                if (tumbleTime > tumbleTimeout) {
                    // ゲームオーバーとする
                    StageScene.Instance.GameOver();
                }
            }
            // 転倒状態の判定を解除
            else {
                tumbleTime = 0;
            }
        }

        // 転倒状態の場合はtrueを返します。
        bool IsTumble() {
            // 転倒判定
            var zAngle = Mathf.Repeat(transform.eulerAngles.z, 360);
            if (zAngle < 60 || zAngle > 300) {
                return false;
            }
            return true;
        }

        // プレイヤーが他のオブジェクトのトリガーに侵入した際に呼び出されます。
        private void OnTriggerEnter2D(Collider2D collision) {
            // ゴール判定
            if (collision.CompareTag("Finish")) {
                StageScene.Instance.StageClear();
            }
            // ゲームオーバー判定
            else if (collision.CompareTag("GameOver")) {
                StageScene.Instance.GameOver();
            }
        }
    }
}