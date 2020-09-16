using UnityEngine;
using UnityEngine.UI;

namespace BirdEscape {
    // ステージ開始前のカウントダウン演出を管理します。
    public class CountDown : MonoBehaviour {
        // ステージ番号表示用のUIを指定します。
        [SerializeField]
        private Text stageNoUI = null;

        // Start is called before the first frame update
        void Start() {
            stageNoUI.text = string.Format("STAGE {0}", StageScene.StageNo);
        }

        // ステージのプレイを開始します。
        // Startアニメーション内のEventで実行されます。
        public void OnStartStageEvent() {
            StageScene.Instance.StartStage();
            // ３秒後に削除
            Destroy(gameObject, 3);
        }
    }
}