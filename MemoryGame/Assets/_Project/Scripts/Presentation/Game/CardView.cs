using MemoryGame.Client.Domain.Game;
using UnityEngine;
using UnityEngine.UI;

namespace MemoryGame.Client.Presentation.Game
{
    public sealed class CardView : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Button button = null;
        [SerializeField] private Text labelText = null;

        private CardState _cardState;

        public CardState CardState
        {
            get { return _cardState; }
        }

        private void Awake()
        {
            if (button != null)
                button.onClick.AddListener(Reveal);
        }

        private void OnDestroy()
        {
            if (button != null)
                button.onClick.RemoveListener(Reveal);
        }

        public void Bind(CardState cardState)
        {
            _cardState = cardState;
            ShowHidden();
        }

        public void ShowHidden()
        {
            SetLabel("?");
        }

        public void Reveal()
        {
            if (_cardState == null)
                return;

            _cardState.Reveal();
            SetLabel(_cardState.Name);
        }

        private void SetLabel(string value)
        {
            if (labelText != null)
                labelText.text = value;
        }
    }
}
