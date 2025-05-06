import "../styles/Error&Ready.css";

import { useEffect, useState } from "react";

import Error from "../components/Error";
import Navbar from "../layouts/Navbar";
import Ready from "../components/Ready";
import Table from "../components/Table";
import ThreeDotLoading from "../components/ThreeDotLoading";
import { useSearchParams } from "react-router";
import useSocket from "../common/hooks/useSocket";

function Game() {
	const { socket, connect, joinGame, sendPlayerIsReady /*, attemptMove */ } =
		useSocket();

	const [subscribed, setSubscribed] = useState(false);
	const [joined, setJoined] = useState(false);
	const [showError, setShowError] = useState(false);
	const [searchParams] = useSearchParams();

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
		if (subscribed) {
			const gameCode = searchParams.get("code");
			if (!gameCode) return;
			joinGame(gameCode);
		}
		// eslint-disable-next-line react-hooks/exhaustive-deps
	}, [subscribed]);

	const handlePlayerIsReady = async () => {
		if (!playerIsReady) {
			await sendPlayerIsReady();
			setPlayerIsReady(true);
		}
	};

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
					/>
				) : (
					cells != null && <Table cells={cells} />
				)}
			</div>
		</>
	);
}

export default Game;
