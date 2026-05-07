using System;
using MemoryGame.Client.Composition;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MemoryGame.Client.Presentation.MainMenu
{
    public sealed class MainMenuController : MonoBehaviour
    {
        [Header("Scene")]
        [SerializeField] private string gameSceneName = "GameScene";

        [Header("UI")]
        [SerializeField] private GameObject titlePanel = null;
        [SerializeField] private Button startGameButton = null;
        [SerializeField] private GameObject loadingText = null;
        [SerializeField] private GameObject errorText = null;

        private bool _isStartingGame;

        private void Awake()
        {
            ShowIdleState();
        }

        public void StartGame()
        {
            if (_isStartingGame)
                return;

            _ = StartGameAsync();
        }

        private async System.Threading.Tasks.Task StartGameAsync()
        {
            try
            {
                if (ClientCompositionRoot.Instance == null)
                {
                    ShowStartGameError("ClientCompositionRoot instance is not available.");
                    return;
                }

                _isStartingGame = true;
                ShowLoadingState();

                await ClientCompositionRoot.Instance.StartGameUseCase.Execute();

                SceneManager.LoadScene(gameSceneName);
            }
            catch (Exception exception)
            {
                ShowStartGameError($"Failed to start game: {exception.Message}");
            }
            finally
            {
                _isStartingGame = false;
            }
        }

        private void ShowIdleState()
        {
            SetActive(titlePanel, true);
            SetButtonVisible(startGameButton, true);
            SetButtonInteractable(startGameButton, true);
            SetActive(loadingText, false);
            SetActive(errorText, false);
        }

        private void ShowLoadingState()
        {
            SetActive(titlePanel, false);
            SetButtonVisible(startGameButton, false);
            SetButtonInteractable(startGameButton, false);
            SetActive(loadingText, true);
            SetActive(errorText, false);
        }

        private void ShowErrorState()
        {
            SetActive(titlePanel, true);
            SetButtonVisible(startGameButton, true);
            SetButtonInteractable(startGameButton, true);
            SetActive(loadingText, false);
            SetActive(errorText, true);
        }

        private void ShowStartGameError(string message)
        {
            Debug.LogWarning(message);
            ShowErrorState();
        }

        private static void SetActive(GameObject target, bool isActive)
        {
            if (target != null)
                target.SetActive(isActive);
        }

        private static void SetButtonInteractable(Button button, bool isInteractable)
        {
            if (button != null)
                button.interactable = isInteractable;
        }

        private static void SetButtonVisible(Button button, bool isVisible)
        {
            if (button != null)
                button.gameObject.SetActive(isVisible);
        }
    }
}
