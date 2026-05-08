using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MemoryGame.Client.Domain.Game;
using UnityEngine;

namespace MemoryGame.Client.Presentation.Game
{
    public sealed class BoardView : MonoBehaviour
    {
        [Header("Cards")]
        [SerializeField] private Transform cardContainer = null;
        [SerializeField] private CardView cardPrefab = null;

        private readonly List<CardView> _cardViews = new();
        private readonly CardFaceImageLoader _cardFaceImageLoader = new();

        public event Action<CardView> CardClicked;

        public async Task ShowCards(IReadOnlyList<CardState> cards, CancellationToken cancellationToken = default)
        {
            ClearCards();

            if (cardContainer == null || cardPrefab == null)
            {
                Debug.LogError("BoardView is missing a card container or card prefab reference.");
                return;
            }

            Dictionary<string, Sprite> spritesByImageUrl = await LoadCardSprites(cards, cancellationToken);

            foreach (CardState card in cards)
            {
                CardView cardView = Instantiate(cardPrefab, cardContainer);
                spritesByImageUrl.TryGetValue(card.ImageUrl, out Sprite faceSprite);
                cardView.Bind(card, faceSprite);
                cardView.Clicked += HandleCardClicked;
                _cardViews.Add(cardView);
            }
        }

        public void RefreshCards()
        {
            foreach (CardView cardView in _cardViews)
                cardView.Refresh();
        }

        private void ClearCards()
        {
            if (cardContainer == null)
                return;

            foreach (CardView cardView in _cardViews)
                if (cardView != null)
                    cardView.Clicked -= HandleCardClicked;

            _cardViews.Clear();

            for (int i = cardContainer.childCount - 1; i >= 0; i--)
                Destroy(cardContainer.GetChild(i).gameObject);
        }

        private void HandleCardClicked(CardView cardView)
        {
            CardClicked?.Invoke(cardView);
        }

        private async Task<Dictionary<string, Sprite>> LoadCardSprites(
            IReadOnlyList<CardState> cards,
            CancellationToken cancellationToken)
        {
            Dictionary<string, Sprite> spritesByImageUrl = new();

            foreach (CardState card in cards)
            {
                if (string.IsNullOrWhiteSpace(card.ImageUrl) || spritesByImageUrl.ContainsKey(card.ImageUrl))
                    continue;

                try
                {
                    Sprite sprite = await _cardFaceImageLoader.LoadSprite(card.ImageUrl, cancellationToken);
                    spritesByImageUrl[card.ImageUrl] = sprite;
                }
                catch (Exception exception)
                {
                    Debug.LogWarning($"Could not load image for card '{card.Name}': {exception.Message}");
                    spritesByImageUrl[card.ImageUrl] = null;
                }
            }

            return spritesByImageUrl;
        }
    }
}
