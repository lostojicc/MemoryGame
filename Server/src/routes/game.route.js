import express from "express";
import { createGame, healthCheck } from "../controllers/game.controller.js";

const router = express.Router();

router.get("/health", healthCheck);
router.get("/new", createGame);

export default router;
