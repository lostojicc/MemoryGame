# Memory Game Server

Simple Express server that returns randomized card configurations for the Unity client.

## Run locally

```bash
npm install
npm start
```

The server uses `PORT` from `.env` or Render.

## Render deployment

Use these settings:

- Build command: `npm install`
- Start command: `npm start`
- Environment: Node

## Endpoints

### `GET /api/games/health`

Returns a basic health check.

### `GET /api/games/new`

Creates a new randomized game configuration. You can optionally pass `pairs` as a query parameter.

```bash
/api/games/new?pairs=8
```

If `pairs` is omitted, the server randomly chooses a number of pairs from the available cards.

Response:

```json
{
  "gameId": "game_...",
  "pairs": 8,
  "cardCount": 16,
  "cards": [
    {
      "instanceId": "dragon_a",
      "matchId": "dragon",
      "name": "Dragon",
      "imageUrl": "https://..."
    }
  ]
}
```

Each pair shares the same `matchId`. The Unity client should compare `matchId` values to decide whether two revealed cards match.
