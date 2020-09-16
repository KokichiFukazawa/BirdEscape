using System.Collections;
using UnityEngine;

namespace BirdEscape {
    // ゲームオーバーUIのボタン操作などを管理します。
    public class GameOver : MonoBehaviour {
        // 「はい」ボタンと「いいえ」ボタンを指定します。
        [SerializeField]
        private RectTransform[] buttons = null;
        // 現在選択されているボタンID
        int buttonIndex = 0;

        // シーン遷移用のアニメーター
        public Animator animator;

        // Start is called before the first frame update
        void Start() {
            animator = GetComponent<Animator>();
            StartCoroutine(OnStart());
        }

        // ゲームオーバーの演出を記述します。
        IEnumerator OnStart() {
            // ２秒待機
            yield return new WaitForSeconds(2);

            // ボタンの入力を待機
            while (true) {
                // 左右入力を取得
                if (Input.GetButtonDown("Horizontal")) {
                    var h = Input.GetAxisRaw("Horizontal");
                    if (h < 0) {
                        if (buttonIndex > 0) {
                            buttonIndex--;
                        }
                    }
                    else if (h > 0) {
                        if (buttonIndex < buttons.Length - 1) {
                            buttonIndex++;
                        }
                    }
                }
                // 決定ボタンが入力された場合
                else if (Input.GetButtonDown("Submit")) {
                    break;
                }
                // ボタンの選択状態を更新
                for (int index = 0; index < buttons.Length; index++) {
                    // 選択状態
                    if (index == buttonIndex) {
                        buttons[index].localScale = new Vector3(1, 1, 1);
                    }
                    // 非選択状態
                    else {
                        buttons[index].localScale = new Vector3(0.9f, 0.9f, 1);
                    }

                }
                yield return null;
            }

            // Stageをリトライする
            if (buttonIndex == 0) {
                StageScene.Instance.Retry();
            }
            // ギブアップする
            else {
                StageScene.Instance.GiveUp();
            }
        }
    }
}