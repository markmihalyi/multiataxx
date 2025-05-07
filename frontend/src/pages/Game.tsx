import "../styles/Error&Ready.css";

import {
	GameResult,
	GameState,
	GameStateChangedResponse,
	JoinSuccessfulResponse,
} from "../types";
import { useEffect, useRef, useState } from "react";

import { CellState } from "../constants";
import Error from "../components/Error";
import Navbar from "../layouts/Navbar";
import Ready from "../components/Ready";
import Table from "../components/Table";
import ThreeDotLoading from "../components/ThreeDotLoading";
import useAuth from "../common/hooks/useAuth";
import { useSearchParams } from "react-router";
import useSocket from "../common/hooks/useSocket";

function Game() {
	const { socket, connect, joinGame, sendPlayerIsReady } = useSocket();
	const { username } = useAuth();
	const [searchParams] = useSearchParams();

	const [connected, setConnected] = useState(false);
	const [subscribed, setSubscribed] = useState(false);
	const [joined, setJoined] = useState(false);
	const [showError, setShowError] = useState(false);

	const [ownPlayerId, setOwnPlayerId] = useState<number>(-1);
	const [otherPlayerName, setOtherPlayerName] = useState<string | null>(null);
	const [gameState, setGameState] = useState<GameState | null>(null);
	const [cells, setCells] = useState<CellState[][] | null>(null);
	const [playerIsReady, setPlayerIsReady] = useState(false);
	const [timeRemaining, setTimeRemaining] = useState<number[] | null>(null);
	const [gameResult, setGameResult] = useState<GameResult | null>(null);

	useEffect(() => {
		async function connectToHub() {
			try {
				await connect();
				setConnected(true);
				console.log("✅ SignalR connection estabilished.");
			} catch (error) {
				setShowError(true);
				console.error("❌ SignalR error:", error);
			}
		}
		connectToHub();
		// eslint-disable-next-line react-hooks/exhaustive-deps
	}, []);

	useEffect(() => {
		if (socket === null) return;

		socket.on("JoinSuccessful", (data: JoinSuccessfulResponse) => {
			console.log("JoinSuccessful", data);
			setJoined(true);
			setOwnPlayerId(data.ownPlayerId);
			setOtherPlayerName(data.otherPlayerName);
			setGameState(data.state);
			setCells(data.cells);
			setTimeRemaining(data.timeRemaining);
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
			setCells(data.cells);
			setTimeRemaining(data.timeRemaining);
			if (data.gameResult) {
				setGameResult(data.gameResult);
			}
		});

		setSubscribed(true);
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
		if (timeRemaining === null || gameResult !== null) return;

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
					cells != null && (
						<>
							<div>
								<p style={{ color: "black" }}>
									{ownPlayerId === 0
										? username
										: otherPlayerName}
								</p>
								<p style={{ color: "black" }}>
									P1 time:{" "}
									{timeRemaining !== null
										? timeRemaining[0]
										: "-"}
								</p>
							</div>
							<Table
								cells={cells}
								ownCellState={ownPlayerId + 1}
							/>
							<div>
								<p style={{ color: "black" }}>
									{ownPlayerId === 1
										? username
										: otherPlayerName}
								</p>
								<p style={{ color: "black" }}>
									P2 time:{" "}
									{timeRemaining !== null
										? timeRemaining[1]
										: "-"}
								</p>
							</div>
						</>
					)
				)}
			</div>
		</>
	);
}

export default Game;
