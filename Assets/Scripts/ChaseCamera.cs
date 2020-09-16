using UnityEngine;

namespace BirdEscape {
    // プレイヤーを追尾するカメラを表します。
    public class ChaseCamera : MonoBehaviour {
        // プレイヤーの位置座標
        [SerializeField]
        private Transform player = null;

        // Start is called before the first frame update
        void Start() {
            UpdateTransform();
        }

        // Update is called once per frame
        void Update() {
            UpdateTransform();
        }

        private void UpdateTransform() {
            // プレイヤーのx座標を追尾する
            var position = transform.position;
            position.x = player.position.x + 4.5f;
            position.y = player.position.y + 1.5f;
            transform.position = position;
        }
    }
}