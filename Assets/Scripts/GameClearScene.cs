using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BirdEscape {
    // ゲームクリアー画面における進行制御を管理します。
    public class GameClearScene : MonoBehaviour {
        // シーン遷移用のアニメーターを指定します。
        [SerializeField]
        private Animator sceneAnimator = null;
        // アニメーションのパラメーター
        static readonly int fadeOutTrigger = Animator.StringToHash("FadeOut Trigger");

        // Start is called before the first frame update
        void Start() {
            StartCoroutine(OnStart());
        }

        IEnumerator OnStart() {
            // 2秒間は決定ボタンの入力を受け付けない
            yield return new WaitForSeconds(2);
            // 決定ボタンの入力を待機
            while (true) {
                if (Input.GetButtonDown("Submit")) {
                    // フェードアウトのステートへ遷移させる
                    sceneAnimator.SetTrigger(fadeOutTrigger);
                    // アニメーション終了まで待機
                    yield return new WaitForSeconds(1);
                    // タイトル画面を読み込む
                    SceneManager.LoadScene("Title");
                    break;
                }
                yield return null;
            }
        }
    }
}