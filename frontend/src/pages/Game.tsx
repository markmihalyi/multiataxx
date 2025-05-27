import "../styles/Error&Ready.css";

import {
	Booster,
	Cell,
	GameResult,
	GameState,
	GameStateChangedResponse,
	GameType,
	JoinSuccessfulResponse,
} from "../types";
import { useEffect, useRef, useState } from "react";
import { useNavigate, useSearchParams } from "react-router";

import { CellState } from "../constants";
import Error from "../components/Error";
import GameCodeCopier from "../components/GameCodeCopier";
import GameOverPopup from "../components/GameOverPopup";
import Navbar from "../layouts/Navbar";
import Ready from "../components/Ready";
import Table from "../components/Table";
import ThreeDotLoading from "../components/ThreeDotLoading";
import api from "../api";
import useAuth from "../common/hooks/useAuth";
import useSocket from "../common/hooks/useSocket";

function secondsToTime(seconds: number): string {
	const minutes = Math.floor(seconds / 60);
	const remainingSeconds = seconds % 60;
	const mm = String(minutes).padStart(2, "0");
	const ss = String(remainingSeconds).padStart(2, "0");
	return `${mm}:${ss}`;
}

function Game() {
	const { socket, connect, joinGame, sendPlayerIsReady, tryUseBooster } =
		useSocket();
	const { username, isLoggedIn } = useAuth();
	const [searchParams] = useSearchParams();
	const navigate = useNavigate();

	const [connected, setConnected] = useState(false);
	const [subscribed, setSubscribed] = useState(false);
	const [joined, setJoined] = useState(false);
	const [showError, setShowError] = useState(false);

	const [gameType, setGameType] = useState<GameType | null>(null);
	const [boosters, setBoosters] = useState<Booster[]>([]);
	const [ownPlayerId, setOwnPlayerId] = useState<number>(-1);
	const [otherPlayerName, setOtherPlayerName] = useState<string | null>(null);
	const [gameState, setGameState] = useState<GameState | null>(null);
	const [cells, setCells] = useState<Cell[][] | null>(null);
	const [playerIsReady, setPlayerIsReady] = useState(false);
	const [timeRemaining, setTimeRemaining] = useState<number[] | null>(null);
	const [gameResult, setGameResult] = useState<GameResult | null>(null);

	useEffect(() => {
		async function connectToHub() {
			try {
				await connect();
				setConnected(true);
				console.log("✅ SignalR connection established.");
			} catch (error) {
				console.warn("⚠️ Initial SignalR connection failed:", error);

				try {
					await api.post(
						"/api/auth/refresh",
						{},
						{ withCredentials: true }
					);
					await connect();
					setConnected(true);
					console.log(
						"✅ SignalR connection established after token refresh."
					);
				} catch (refreshError) {
					console.error(
						"❌ SignalR connection failed even after token refresh:",
						refreshError
					);
					setShowError(true);
				}
			}

			await updateBoosters();
		}

		connectToHub();
		// eslint-disable-next-line react-hooks/exhaustive-deps
	}, []);

	const updateCells = (newCellStates: CellState[][]) => {
		const cells: Cell[][] = [];
		for (let i = 0; i < newCellStates.length; i++) {
			const cellsInRow: Cell[] = [];
			for (let j = 0; j < newCellStates.length; j++) {
				cellsInRow.push({
					state: newCellStates[i][j],
					isTipStartPoint: false,
					isTipDestPoint: false,
				});
			}
			cells.push(cellsInRow);
		}
		setCells(cells);
	};

	const updateBoosters = async () => {
		if (isLoggedIn) {
			const { data } = await api.get<Booster[]>("/api/game/boosters");
			if (data !== null) {
				setBoosters(data);
			}
		}
	};

	useEffect(() => {
		if (socket === null) return;

		socket.on("JoinSuccessful", (data: JoinSuccessfulResponse) => {
			console.log("JoinSuccessful", data);
			setJoined(true);
			setGameType(data.gameType);
			setOwnPlayerId(data.ownPlayerId);
			setOtherPlayerName(data.otherPlayerName);
			setGameState(data.state);
			setTimeRemaining(data.timeRemaining);
			updateCells(data.cells);
		});

		socket.on("JoinFailed", () => {
			console.log("JoinFailed");
			socket.stop();
			setShowError(true);
		});

		socket.on("PlayerJoined", (playerName: string) => {
			console.log("PlayerJoined", playerName);
			setOtherPlayerName(playerName);
		});

		socket.on("GameStateChanged", (data: GameStateChangedResponse) => {
			console.log("GameStateChanged", data);
			setGameState(data.state);
			setTimeRemaining(data.timeRemaining);
			updateCells(data.cells);
			if (data.gameResult) {
				setGameResult(data.gameResult);
			}
		});

		socket.on(
			"TipReceived",
			async (
				startRow: number,
				startCol: number,
				destRow: number,
				destCol: number
			) => {
				setCells((cells) => {
					if (cells === null) return cells;
					cells[startRow - 1][startCol - 1].isTipStartPoint = true;
					cells[destRow - 1][destCol - 1].isTipDestPoint = true;
					return cells;
				});
				await updateBoosters();
			}
		);

		setSubscribed(true);
		// eslint-disable-next-line react-hooks/exhaustive-deps
	}, [socket]);

	useEffect(() => {
		if (connected && subscribed) {
			const gameCode = searchParams.get("code");
			if (!gameCode) return;
			joinGame(gameCode);
		}
		// eslint-disable-next-line react-hooks/exhaustive-deps
	}, [connected, subscribed]);

	useEffect(() => {
		if (socket !== null && gameResult !== null) {
			socket.stop();
		}
		// eslint-disable-next-line react-hooks/exhaustive-deps
	}, [gameResult]);

	const handlePlayerIsReady = async () => {
		if (!playerIsReady) {
			await sendPlayerIsReady();
			setPlayerIsReady(true);
		}
	};

	const intervalRef = useRef<number | null>(null);
	useEffect(() => {
		if (
			gameType !== "MultiPlayer" ||
			timeRemaining === null ||
			gameResult !== null
		)
			return;

		intervalRef.current = setInterval(() => {
			setTimeRemaining((times) => {
				if (times === null) return times;
				const player1NewTime =
					gameState === "Player1Turn" ? times[0] - 1 : times[0];
				const player2NewTime =
					gameState === "Player2Turn" ? times[1] - 1 : times[1];
				if (
					player1NewTime === times[0] &&
					player2NewTime === times[1]
				) {
					return times;
				}
				return [player1NewTime, player2NewTime];
			});
		}, 1000);

		return () => {
			if (intervalRef.current !== null) {
				clearInterval(intervalRef.current);
			}
		};
		// eslint-disable-next-line react-hooks/exhaustive-deps
	}, [timeRemaining]);

	return (
		<>
			<Navbar />
			<div className="container">
				{!joined && !showError && <ThreeDotLoading />}
				{!joined && showError && <Error />}
				{gameState === "Waiting" ? (
					<Ready
						buttonHandle={handlePlayerIsReady}
						isReady={playerIsReady}
						gameCode={searchParams.get("code") ?? ""}
					/>
				) : (
					cells !== null &&
					gameState !== null && (
						<>
							<div>
								{boosters.map((booster) => (
									<button
										key={booster.id}
										onClick={() =>
											tryUseBooster(booster.id)
										}
									>
										{booster.name} ({booster.amount})
									</button>
								))}
							</div>

							<div className="game-container">
								<div
									className="user-container"
									id={
										gameState === "Player1Turn"
											? "blue-border"
											: "neutral-border"
									}
								>
									<div className="user-name Player1">
										<p id="user1-name-p">
											{ownPlayerId === 0
												? username
												: otherPlayerName}
										</p>
									</div>
									<div className="user-time">
										<p id="user1-time-p">
											{timeRemaining !== null &&
											gameType === "MultiPlayer"
												? secondsToTime(
														timeRemaining[0]
												  )
												: "-"}
										</p>
									</div>
								</div>
								<Table
									cells={cells}
									ownCellState={ownPlayerId + 1}
									gameState={gameState}
								/>
								<div
									className="user-container"
									id={
										gameState === "Player2Turn"
											? "green-border"
											: "neutral-border"
									}
								>
									<div className="user-name Player2">
										<p id="user2-name-p">
											{ownPlayerId === 1
												? username
												: otherPlayerName}
										</p>
									</div>
									<div className="user-time">
										<p id="user2-time-p">
											{timeRemaining !== null &&
											gameType === "MultiPlayer"
												? secondsToTime(
														timeRemaining[1]
												  )
												: "-"}
										</p>
									</div>
								</div>
							</div>
							<div id="in-game-copier">
								<GameCodeCopier
									gameCode={searchParams.get("code") ?? ""}
								/>
							</div>
						</>
					)
				)}
			</div>

			{gameResult && (
				<GameOverPopup
					isVisible={gameResult !== null}
					gameResult={gameResult}
					ownPlayerId={ownPlayerId}
					onClose={() => {
						setGameResult(null);
						navigate(-1);
					}}
				/>
			)}
		</>
	);
}

export default Game;
