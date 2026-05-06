import cards from "../data/cards.json" with { type: "json" };
import { shuffle } from "../utils/shuffle.js";

const MIN_PAIRS = 4;
const MAX_PAIRS = 10;

export const healthCheck = async (req, res) => {
    res.status(200).json({
        success: true,
        message: "Memory game server is running."
    });
};

export const createGame = async (req, res) => {
    try {
        const pairs = getPairCount(req.query.pairs);
        const selectedCards = shuffle(cards).slice(0, pairs);
        const gameCards = shuffle(createPairs(selectedCards));

        res.status(200).json({
            success: true,
            game: {
                gameId: `game_${Date.now()}`,
                pairs,
                cardCount: gameCards.length,
                cards: gameCards
            }
        });
    } catch (error) {
        res.status(error.status || 500).json({
            success: false,
            error: error.message
        });
    }
};

const getPairCount = (requestedPairs) => {
    const maxPairs = Math.min(MAX_PAIRS, cards.length);

    if (cards.length < MIN_PAIRS)
        throw createError(500, `At least ${MIN_PAIRS} cards are required.`);

    if (!requestedPairs)
        return randomNumber(MIN_PAIRS, maxPairs);

    const pairs = Number(requestedPairs);

    if (!Number.isInteger(pairs))
        throw createError(400, "Pairs must be an integer.");

    if (pairs < MIN_PAIRS || pairs > maxPairs)
        throw createError(400, `Pairs must be between ${MIN_PAIRS} and ${maxPairs}.`);

    return pairs;
};

const createPairs = (selectedCards) => selectedCards.flatMap((card) => [
    createCard(card, "a"),
    createCard(card, "b")
]);

const createCard = (card, copy) => ({
    instanceId: `${card.id}_${copy}`,
    matchId: card.id,
    name: card.name,
    imageUrl: card.imageUrl
});

const randomNumber = (min, max) => Math.floor(Math.random() * (max - min + 1)) + min;

const createError = (status, message) => {
    const error = new Error(message);
    error.status = status;
    return error;
};
