using UnityEngine;

namespace BirdEscape {
    // ポーズUIのボタン操作などを管理します。
    public class Pause : MonoBehaviour {
        // 「RESUME」と「RESTART」と「EXIT」ボタンを指定します。
        [SerializeField]
        private RectTransform[] buttons = null;
        // 現在選択されているボタンID
        public int buttonIndex = 0;

        // Update is called once per frame
        void Update() {
            // 上下入力を取得
            if (Input.GetButtonDown("Vertical")) {
                var v = Input.GetAxisRaw("Vertical");
                if (v < 0) {
                    if (buttonIndex < buttons.Length - 1) {
                        buttonIndex++;
                    }
                }
                else if (v > 0) {
                    if (buttonIndex > 0) {
                        buttonIndex--;
                    }
                }
            }
            // 決定ボタンが入力された場合
            else if (Input.GetButtonDown("Submit")) {
                switch (buttonIndex) {
                    case 0:
                        // ポーズを解除する
                        StageScene.Instance.Resume();
                        Destroy(gameObject);
                        break;
                    case 1:
                        // Stageをリトライする
                        StageScene.Instance.Retry();
                        break;
                    case 2:
                        // ギブアップする
                        StageScene.Instance.GiveUp();
                        break;
                }
            }
            // ボタンの選択状態を更新
            for (int index = 0; index < buttons.Length; index++) {
                // 選択状態
                if (index == buttonIndex) {
                    buttons[index].localScale = new Vector3(1, 1, 1);
                }
                // 非選択状態
                else {
                    buttons[index].localScale = new Vector3(0.95f, 0.95f, 1);
                }
            }
        }
    }
}
