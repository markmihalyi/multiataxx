type ApiResponse = {
	success: boolean;
	message: string;
};

type HostGameApiResponse = ApiResponse & { gameCode: string };

type JoinSuccessfulResponse = {
	ownPlayerId: number;
	otherPlayerName: string | null;
	state: GameState;
	cells: CellState[][];
	timeRemaining: number[];
};

type GameStateChangedResponse = {
	state: GameState;
	cells: CellState[][];
	timeRemaining: number[];
	gameResult?: GameResult;
};

type GameState = "Waiting" | "Player1Turn" | "Player2Turn" | "Ended";

type GameResult = "Draw" | "Player1Won" | "Player2Won";

enum CellState {
	Empty = 0,
	Player1 = 1,
	Player2 = 2,
	Wall = 3,
}
