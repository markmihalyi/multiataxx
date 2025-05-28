import { CellState } from "./constants";

export type ApiResponse = {
	success: boolean;
	message: string;
};

export type HostGameApiResponse = ApiResponse & { gameCode: string };

export type MeApiResponse = { id: number; username: string };

export type JoinSuccessfulResponse = {
	gameType: GameType;
	ownPlayerId: number;
	otherPlayerName: string | null;
	state: GameState;
	cells: CellState[][];
	timeRemaining: number[];
};

export type GameType = "SinglePlayer" | "MultiPlayer";

export type GameStateChangedResponse = {
	state: GameState;
	cells: CellState[][];
	timeRemaining: number[];
	gameResult?: GameResult;
};

export type GameState = "Waiting" | "Player1Turn" | "Player2Turn" | "Ended";

export type Booster = {
	id: number;
	name: string;
	amount: number;
};

export type GameResult = "Draw" | "Player1Won" | "Player2Won";

export type CellPosition = {
	row: number;
	col: number;
};

export type Cell = {
	state: CellState;
	isTipStartPoint: boolean;
	isTipDestPoint: boolean;
};
