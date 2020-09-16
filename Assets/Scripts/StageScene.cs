using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BirdEscape {
    // ステージ画面における進行制御を管理します。
    public class StageScene : MonoBehaviour {
        // フェードインアウト演出用の画像を指定します。
        [SerializeField]
        private Image transitionImage = null;
        // フェードインアウト演出用の黒画像を指定します。
        [SerializeField]
        private Sprite blackSprite = null;
        // フェードインアウト演出用の白画像を指定します。
        [SerializeField]
        private Sprite whiteSprite = null;

        // UI表示用のRootオブジェクトを指定します。
        [SerializeField]
        private Transform uiRoot = null;

        // カウントダウンUIのプレハブを指定します。
        [SerializeField]
        private GameObject countDownPrefab = null;

        // ゲームオーバーUIのプレハブを指定します。
        [SerializeField]
        private GameObject gameOverPrefab = null;

        // ステージクリアーUIのプレハブを指定します。
        [SerializeField]
        private GameObject stageClearPrefab = null;

        // ポーズUIのプレハブを指定します。
        [SerializeField]
        private GameObject pausePrefab = null;

        // シーン遷移用のアニメーター
        Animator animator;
        // アニメーションのパラメーター
        static readonly int fadeOutTrigger = Animator.StringToHash("FadeOut Trigger");

        // ステージ数を指定します。
        [SerializeField]
        private int stageCount = 4;

        // 現在プレイしているステージ番号を取得または設定します。
        public static int StageNo {
            get { return stageNo; }
            set { stageNo = value; }
        }
        // 現在プレイしているステージ番号
        static int stageNo = 0;

        // ステージ画面内の進行状態を表します。
        enum SceneState {
            // ステージ開始演出中
            Start,
            // ステージプレイ中
            Play,
            // ゲームオーバーが確定していて演出中
            GameOver,
            // ステージクリアーが確定していて演出中
            StageClear,
        }
        SceneState sceneState = SceneState.Start;

        // ポーズ状態の場合はtrue、プレイ状態の場合はfalse
        bool isPaused = false;

        #region インスタンスへのstaticなアクセスポイント
        // このクラスのインスタンスを取得します。
        public static StageScene Instance {
            get { return instance; }
            set { instance = value; }
        }
        private static StageScene instance = null;

        // AwakeメソッドはStartメソッドよりも先に実行したい初期化処理を記述します。
        void Awake() {
            // 生成されたインスタンス（自分自身）をstaticな変数に保存
            instance = this;

            // Resourcesフォルダーからステージのプレハブを読み込む
            var prefabName = string.Format("stage{0}", stageNo);
            var stagePrefab = Resources.Load<GameObject>(prefabName);
            Instantiate(stagePrefab, transform);
        }
        #endregion

        // Start is called before the first frame update
        void Start() {
            animator = GetComponent<Animator>();
            sceneState = 0;
            Instantiate(countDownPrefab, uiRoot);
        }

        // ステージのプレイを開始します。
        public void StartStage() {
            sceneState = SceneState.Play;
        }

        // Update is called once per frame
        void Update() {
            switch (sceneState) {
                case SceneState.Play:
                    if (!isPaused) {
                        // ポーズ
                        if (Input.GetButtonDown("Cancel")) {
                            Pause();
                        }
                    }
                    break;
                case SceneState.Start:
                case SceneState.GameOver:
                case SceneState.StageClear:
                default:
                    break;
            }
        }

        #region ポーズ
        // このステージをポーズします。
        private void Pause() {
            if (!isPaused) {
                isPaused = true;
                Time.timeScale = 0;
                Instantiate(pausePrefab, uiRoot);
            }
        }

        // ポーズ状態を解除します。
        public void Resume() {
            if (isPaused) {
                isPaused = false;
                Time.timeScale = 1;
            }
        }
        #endregion

        #region ステージクリアー
        // このステージをクリアーさせます。
        public void StageClear() {
            // ステージプレイ中のみ
            if (sceneState == SceneState.Play) {
                sceneState = SceneState.StageClear;
                Instantiate(stageClearPrefab, uiRoot);
            }
        }

        // 次のシーンを読み込みます。
        public void LoadNextScene() {
            StageNo++;
            // 残りのステージがまだ存在する場合
            if (StageNo < stageCount) {
                // 次のステージを読み込む
                LoadSceneWithFadeOut("Stage", whiteSprite);
            }
            else {
                // ゲームクリアーの場合
                LoadSceneWithFadeOut("GameClear", whiteSprite);
                StageNo = 0;
            }
        }
        #endregion

        #region ゲームオーバー
        // このステージをゲームオーバーで終了させます。
        public void GameOver() {
            // ステージプレイ中のみ
            if (sceneState == SceneState.Play) {
                sceneState = SceneState.GameOver;
                Instantiate(gameOverPrefab, uiRoot);
            }
        }

        // このステージを再読み込みします。
        public void Retry() {
            // ポーズ状態の場合は解除する
            Resume();
            LoadSceneWithFadeOut("Stage", blackSprite);
        }

        // プレイをあきらめてタイトル画面に戻ります。
        public void GiveUp() {
            // ポーズ状態の場合は解除する
            Resume();
            LoadSceneWithFadeOut("Title", blackSprite);
            StageNo = 0;
        }
        #endregion

        // フェイドアウト後に指定したシーンを読み込みます。
        public void LoadSceneWithFadeOut(string sceneName, Sprite sprite) {
            StartCoroutine(OnLoadSceneWithFadeOut(sceneName, sprite));
        }

        IEnumerator OnLoadSceneWithFadeOut(string sceneName, Sprite sprite) {
            // フェードアウト
            transitionImage.sprite = sprite;
            animator.SetTrigger(fadeOutTrigger);
            yield return new WaitForSeconds(1);
            SceneManager.LoadScene(sceneName);
        }
    }
}