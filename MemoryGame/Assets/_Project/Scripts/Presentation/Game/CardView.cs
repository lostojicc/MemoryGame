using System;
using MemoryGame.Client.Domain.Game;
using UnityEngine;
using UnityEngine.UI;

namespace MemoryGame.Client.Presentation.Game
{
    public sealed class CardView : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Button button = null;
        [SerializeField] private Image cardImage = null;
        [SerializeField] private Image faceImage = null;
        [SerializeField] private Text labelText = null;
        [SerializeField] private Color hiddenColor = new Color(0.7143196f, 1f, 0f, 1f);
        [SerializeField] private Color revealedColor = Color.black;

        private CardState _cardState;
        private Sprite _faceSprite;

        public event Action<CardView> Clicked;

        public CardState CardState
        {
            get { return _cardState; }
        }

        private void Awake()
        {
            SetCardColor(hiddenColor);

            if (faceImage != null)
            {
                faceImage.raycastTarget = false;
                faceImage.gameObject.SetActive(false);
            }

            if (button != null)
                button.onClick.AddListener(HandleClick);
        }

        private void OnDestroy()
        {
            if (button != null)
                button.onClick.RemoveListener(HandleClick);
        }

        public void Bind(CardState cardState, Sprite faceSprite)
        {
            _cardState = cardState;
            _faceSprite = faceSprite;
            Refresh();
        }

        public void Refresh()
        {
            if (_cardState == null)
            {
                SetLabel("?");
                SetInteractable(false);
                return;
            }

            bool isFaceVisible = _cardState.IsRevealed || _cardState.IsMatched;

            SetCardColor(isFaceVisible ? revealedColor : hiddenColor);
            SetFaceVisible(isFaceVisible && _faceSprite != null);
            SetLabelVisible(!isFaceVisible || _faceSprite == null);
            SetLabel(isFaceVisible && _faceSprite == null ? _cardState.Name : "?");
            SetInteractable(!_cardState.IsRevealed && !_cardState.IsMatched);
        }

        private void HandleClick()
        {
            if (_cardState == null)
                return;

            Clicked?.Invoke(this);
        }

        private void SetLabel(string value)
        {
            if (labelText != null)
                labelText.text = value;
        }

        private void SetLabelVisible(bool isVisible)
        {
            if (labelText != null)
                labelText.gameObject.SetActive(isVisible);
        }

        private void SetFaceVisible(bool isVisible)
        {
            if (faceImage == null)
                return;

            faceImage.gameObject.SetActive(isVisible);
            faceImage.sprite = _faceSprite;
        }

        private void SetCardColor(Color color)
        {
            if (cardImage != null)
                cardImage.color = color;
        }

        private void SetInteractable(bool isInteractable)
        {
            if (button != null)
                button.interactable = isInteractable;
        }
    }
}
