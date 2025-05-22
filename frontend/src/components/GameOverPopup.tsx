import "../styles/Popup.css";
import { GameResult } from "../types";

import React, { useState, useEffect } from "react";

interface GameOverPopupProps {
	isVisible: boolean;
	gameResult: GameResult;
	ownPlayerId: number;
	onClose: () => void;
}

function formatTitleResult(isWinner: boolean | null): string {
	switch (isWinner) {
		case true:
			return "Victory!";
		case false:
			return "Defeat!";
		default:
			return "Nice!";
	}
}

function formatTextResult(isWinner: boolean | null): string {
	switch (isWinner) {
		case true:
			return "Congratulations, you won :)";
		case false:
			return "Sorry, you lost :(";
		default:
			return "It's a draw!";
	}
}

const GameOverPopup: React.FC<GameOverPopupProps> = ({
	isVisible,
	gameResult,
	ownPlayerId,
	onClose,
}) => {
	const [isWinner, setIsWinner] = useState<boolean | null>(null);

	useEffect(() => {
		if (gameResult === `Player${ownPlayerId + 1}Won`) {
			setIsWinner(true);
		} else if (gameResult === `Player${ownPlayerId === 0 ? 2 : 1}Won`) {
			setIsWinner(false);
		} else {
			setIsWinner(null); // Draw
		}
	}, [gameResult, ownPlayerId]);

	return (
		<div className={`overlay fade-in ${isVisible ? "" : "hidden"}`}>
			<div
				className="login-popup slide-in"
				onClick={(e) => e.stopPropagation()}
			>
				<h2
					className={
						isWinner === null
							? "draw-h2"
							: isWinner
							? "win-h2"
							: "loose-h2"
					}
				>
					{formatTitleResult(isWinner)}
				</h2>
				<p
					className={
						isWinner === null
							? "draw-p"
							: isWinner
							? "win-p"
							: "loose-p"
					}
				>
					{formatTextResult(isWinner)}
				</p>
				<button className="loginButton w80" onClick={onClose}>
					QUIT
				</button>
			</div>
		</div>
	);
};

export default GameOverPopup;
