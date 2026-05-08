using System.Collections;
using System.Threading;
using MemoryGame.Client.Composition;
using MemoryGame.Client.Domain.Game;
using MemoryGame.Client.Presentation.Audio;
using UnityEngine;
using UnityEngine.UI;

namespace MemoryGame.Client.Presentation.Game
{
    public sealed class GameSceneController : MonoBehaviour
    {
        [Header("Scene")]
        [SerializeField] private string mainMenuSceneName = "MainMenuScene";

        [Header("HUD")]
        [SerializeField] private Text scoreText = null;
        [SerializeField] private Text attemptsText = null;
        [SerializeField] private Text timerText = null;
        [SerializeField] private Text cardCountText = null;
        [SerializeField] private Text comboText = null;

        [Header("Controls")]
        [SerializeField] private Button pauseButton = null;
        [SerializeField] private Image pauseButtonImage = null;
        [SerializeField] private Sprite pauseSprite = null;
        [SerializeField] private Sprite playSprite = null;
        [SerializeField] private Button restartButton = null;
        [SerializeField] private Button exitButton = null;

        [Header("Audio")]
        [SerializeField] private GameAudioController audioController = null;

        [Header("Board")]
        [SerializeField] private BoardView boardView = null;
        [SerializeField] private GameObject loadingText = null;
        [SerializeField] private GameObject cardGrid = null;
        [SerializeField] private GameObject gameOverPanel = null;
        [SerializeField] private float mismatchHideDelay = 0.8f;
        [SerializeField] private float comboPopupDuration = 1f;

        private GameState _gameState;
        private bool _isResolvingMismatch;
        private float _elapsedSeconds;
        private bool _isTimerRunning;
        private bool _isPaused;
        private bool _isRestarting;
        private CancellationTokenSource _boardCancellationTokenSource;
        private Coroutine _comboPopupCoroutine;

        private void OnDestroy()
        {
            _boardCancellationTokenSource?.Cancel();
            _boardCancellationTokenSource?.Dispose();

            if (boardView != null)
                boardView.CardClicked -= HandleCardClicked;

            if (pauseButton != null)
                pauseButton.onClick.RemoveListener(TogglePause);

            if (restartButton != null)
                restartButton.onClick.RemoveListener(RestartGame);

            if (exitButton != null)
                exitButton.onClick.RemoveListener(ExitGame);
        }

        private async void Start()
        {
            if (pauseButton != null)
                pauseButton.onClick.AddListener(TogglePause);

            if (restartButton != null)
                restartButton.onClick.AddListener(RestartGame);

            if (exitButton != null)
                exitButton.onClick.AddListener(ExitGame);

            if (ClientCompositionRoot.Instance == null)
            {
                Debug.LogError("ClientCompositionRoot instance is not available.");
                return;
            }

            _gameState = ClientCompositionRoot.Instance.GameSession.CurrentGame;

            if (_gameState == null)
            {
                Debug.LogError("No active game found. Start the game from the main menu scene first.");
                return;
            }

            UpdateHud();
            HideComboPopup();
            ShowLoadingState();
            SetControlsInteractable(false);
            await ShowBoard();
            ShowGameInProgressState();
            SetControlsInteractable(true);
            StartTimer();
            UpdatePauseButtonIcon();

            Debug.Log(
                $"Game scene loaded. " +
                $"Game ID: {_gameState.GameId}, " +
                $"Pairs: {_gameState.Pairs}, " +
                $"Cards: {_gameState.Cards.Count}");
        }

        private void Update()
        {
            if (!_isTimerRunning || _isPaused)
                return;

            _elapsedSeconds += Time.deltaTime;
            UpdateTimerText();
        }

        private void UpdateHud()
        {
            SetText(scoreText, $"Score: {_gameState.Score}");
            SetText(attemptsText, $"Attempts: {_gameState.Attempts}");
            UpdateTimerText();
            SetText(cardCountText, $"Cards: {_gameState.Cards.Count}");
        }

        private async System.Threading.Tasks.Task ShowBoard()
        {
            if (boardView == null)
            {
                Debug.LogError("GameSceneController is missing a BoardView reference.");
                return;
            }

            _boardCancellationTokenSource?.Cancel();
            _boardCancellationTokenSource?.Dispose();
            _boardCancellationTokenSource = new CancellationTokenSource();

            boardView.CardClicked -= HandleCardClicked;
            await boardView.ShowCards(_gameState.Cards, _boardCancellationTokenSource.Token);
            boardView.CardClicked += HandleCardClicked;
        }

        private void HandleCardClicked(CardView cardView)
        {
            if (_gameState == null || _isPaused || _isRestarting || _isResolvingMismatch || cardView == null)
                return;

            CardSelectionResult result = _gameState.SelectCard(cardView.CardState);
            boardView.RefreshCards();
            UpdateHud();
            PlaySelectionSound(result);

            if (result.Type == CardSelectionResultType.MismatchFound)
                StartCoroutine(HideMismatchAfterDelay(result.FirstCard, result.SecondCard));

            if (result.Type == CardSelectionResultType.MatchFound || result.Type == CardSelectionResultType.GameCompleted)
                ShowComboPopupIfNeeded();

            if (result.Type == CardSelectionResultType.GameCompleted)
            {
                StopTimer();
                ShowGameCompletedState();
                Debug.Log("Game completed.");
            }
        }

        private IEnumerator HideMismatchAfterDelay(CardState firstCard, CardState secondCard)
        {
            _isResolvingMismatch = true;
            yield return new WaitForSeconds(mismatchHideDelay);

            _gameState.HideUnmatchedCards(firstCard, secondCard);
            boardView.RefreshCards();
            UpdateHud();
            _isResolvingMismatch = false;
        }

        private void PlaySelectionSound(CardSelectionResult result)
        {
            if (audioController == null)
                return;

            switch (result.Type)
            {
                case CardSelectionResultType.FirstCardSelected:
                    audioController.PlayFlip();
                    break;
                case CardSelectionResultType.MatchFound:
                case CardSelectionResultType.GameCompleted:
                    audioController.PlayMatch();
                    break;
                case CardSelectionResultType.MismatchFound:
                    audioController.PlayMismatch();
                    break;
            }
        }

        public void TogglePause()
        {
            if (_gameState == null || _isRestarting || _gameState.IsComplete)
                return;

            _isPaused = !_isPaused;
            UpdatePauseButtonIcon();
        }

        public void RestartGame()
        {
            if (_isRestarting)
                return;

            _ = RestartGameAsync();
        }

        public void ExitGame()
        {
            if (ClientCompositionRoot.Instance != null)
                ClientCompositionRoot.Instance.GameSession.ClearCurrentGame();

            StopTimer();
            UnityEngine.SceneManagement.SceneManager.LoadScene(mainMenuSceneName);
        }

        private async System.Threading.Tasks.Task RestartGameAsync()
        {
            try
            {
                if (ClientCompositionRoot.Instance == null)
                {
                    Debug.LogWarning("ClientCompositionRoot instance is not available.");
                    return;
                }

                _isRestarting = true;
                SetControlsInteractable(false);
                StopTimer();

                _gameState = await ClientCompositionRoot.Instance.StartGameUseCase.Execute();
                _isPaused = false;
                _isResolvingMismatch = false;

                UpdateHud();
                HideComboPopup();
                ShowLoadingState();
                await ShowBoard();
                ShowGameInProgressState();
                StartTimer();
                UpdatePauseButtonIcon();
            }
            catch (System.Exception exception)
            {
                Debug.LogWarning($"Failed to restart game: {exception.Message}");
            }
            finally
            {
                _isRestarting = false;
                SetControlsInteractable(true);
            }
        }

        private void StartTimer()
        {
            _elapsedSeconds = 0f;
            _isTimerRunning = true;
            UpdateTimerText();
        }

        private void StopTimer()
        {
            _isTimerRunning = false;
            UpdateTimerText();
        }

        private void UpdateTimerText()
        {
            int totalSeconds = Mathf.FloorToInt(_elapsedSeconds);
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;

            SetText(timerText, $"Time: {minutes:00}:{seconds:00}");
        }

        private void UpdatePauseButtonIcon()
        {
            if (pauseButtonImage == null)
                return;

            Sprite selectedSprite = _isPaused ? playSprite : pauseSprite;

            if (selectedSprite != null)
                pauseButtonImage.sprite = selectedSprite;
        }

        private void ShowGameInProgressState()
        {
            SetActive(loadingText, false);
            SetActive(cardGrid, true);
            SetActive(gameOverPanel, false);
        }

        private void ShowGameCompletedState()
        {
            SetActive(loadingText, false);
            SetActive(cardGrid, false);
            SetActive(gameOverPanel, true);
        }

        private void ShowComboPopupIfNeeded()
        {
            if (comboText == null || _gameState.Combo <= 1)
                return;

            if (_comboPopupCoroutine != null)
                StopCoroutine(_comboPopupCoroutine);

            comboText.text = $"Combo x{_gameState.Combo}!";
            comboText.gameObject.SetActive(true);
            _comboPopupCoroutine = StartCoroutine(HideComboPopupAfterDelay());
        }

        private IEnumerator HideComboPopupAfterDelay()
        {
            yield return new WaitForSeconds(comboPopupDuration);
            HideComboPopup();
        }

        private void HideComboPopup()
        {
            if (_comboPopupCoroutine != null)
            {
                StopCoroutine(_comboPopupCoroutine);
                _comboPopupCoroutine = null;
            }

            if (comboText != null)
                comboText.gameObject.SetActive(false);
        }

        private void ShowLoadingState()
        {
            SetActive(loadingText, true);
            SetActive(cardGrid, false);
            SetActive(gameOverPanel, false);
        }

        private void SetControlsInteractable(bool isInteractable)
        {
            if (pauseButton != null)
                pauseButton.interactable = isInteractable;

            if (restartButton != null)
                restartButton.interactable = isInteractable;

            if (exitButton != null)
                exitButton.interactable = isInteractable;
        }

        private static void SetText(Text target, string value)
        {
            if (target != null)
                target.text = value;
        }

        private static void SetActive(GameObject target, bool isActive)
        {
            if (target != null)
                target.SetActive(isActive);
        }
    }
}
