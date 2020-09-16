using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BirdEscape {
    // タイトル画面における進行制御を管理します。
    public class TitleScene : MonoBehaviour {
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
            // 決定ボタンの入力を待機
            while (true) {
                if (Input.GetButtonDown("Submit")) {
                    // フェードアウトのステートへ遷移させる
                    sceneAnimator.SetTrigger(fadeOutTrigger);
                    // アニメーション終了まで待機
                    yield return new WaitForSeconds(1);
                    // ステージ画面を読み込む
                    SceneManager.LoadScene("Stage");
                    break;
                }
                yield return null;
            }
        }
    }
}