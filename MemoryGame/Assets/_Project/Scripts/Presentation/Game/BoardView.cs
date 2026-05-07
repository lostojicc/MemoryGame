using System.Collections.Generic;
using MemoryGame.Client.Domain.Game;
using UnityEngine;

namespace MemoryGame.Client.Presentation.Game
{
    public sealed class BoardView : MonoBehaviour
    {
        [Header("Cards")]
        [SerializeField] private Transform cardContainer = null;
        [SerializeField] private CardView cardPrefab = null;

        public void ShowCards(IReadOnlyList<CardState> cards)
        {
            ClearCards();

            if (cardContainer == null || cardPrefab == null)
            {
                Debug.LogError("BoardView is missing a card container or card prefab reference.");
                return;
            }

            foreach (CardState card in cards)
            {
                CardView cardView = Instantiate(cardPrefab, cardContainer);
                cardView.Bind(card);
            }
        }

        private void ClearCards()
        {
            if (cardContainer == null)
                return;

            for (int i = cardContainer.childCount - 1; i >= 0; i--)
                Destroy(cardContainer.GetChild(i).gameObject);
        }
    }
}
