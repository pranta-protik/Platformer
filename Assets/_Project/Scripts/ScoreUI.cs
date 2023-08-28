using System.Collections;
using UnityEngine;
using TMPro;

namespace Platformer
{
    public class ScoreUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _scoreText;

        private void Start()
        {
            UpdateScore();
        }

        public void UpdateScore()
        {
            StartCoroutine(UpdateScoreNextFrame());
        }

        private IEnumerator UpdateScoreNextFrame()
        {
            // Make sure all logic has run before updating the score
            yield return null;
            _scoreText.text = GameManager.Instance.Score.ToString();
        }
    }
}