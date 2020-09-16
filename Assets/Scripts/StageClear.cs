using System.Collections;
using UnityEngine;

namespace BirdEscape {
    // ステージクリアーUIの進行制御を管理します。
    public class StageClear : MonoBehaviour {
        // Start is called before the first frame update
        void Start() {
            StartCoroutine(OnStageClear());
        }

        // ステージクリアーの演出を記述します。
        IEnumerator OnStageClear() {
            // ２秒待機
            yield return new WaitForSeconds(2);
            // 決定ボタンの入力を待機
            while (true) {
                if (Input.GetButtonDown("Submit")) {
                    break;
                }
                yield return null;
            }
            StageScene.Instance.LoadNextScene();
        }
    }
}