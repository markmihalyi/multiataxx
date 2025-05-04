import "../styles/Panel.css";

import api, { handleAxiosError } from "../api";

import Dropdown from "../components/Dropdown";
import useAuth from "../common/hooks/useAuth";
import { useNavigate } from "react-router";
import { useState } from "react";

type PanelProps = {
	panelTitle: string;
	bgColor: string;
	navLink: string;
	buttonText: string;
	dropdown: boolean;
	canDisable: boolean;
	options1: string[];
	options2: string[];
};

const Panel: React.FC<PanelProps> = ({
	panelTitle,
	bgColor,
	buttonText,
	dropdown,
	canDisable,
	options1,
	options2,
}) => {
	const { isLoggedIn } = useAuth();
	const navigate = useNavigate();

	const [selectedTime, setSelectedTime] = useState<string | null>(
		options1[0] || null
	);
	const [selectedBoardSize, setSelectedBoardSize] = useState<string | null>(
		options2[0] || null
	);
	const [difficultyLevel, setDifficultyLevel] = useState<string | null>(
		options1[0] || null
	);

	const [gameCode, setGameCode] = useState<string>("");
	const [inputError, setInputError] = useState<boolean>(false);

	const handleHostGame = async () => {
		let turnMinutes = 0;
		const timeData = selectedTime?.split(":") || ["0", "30"];
		turnMinutes += Number(timeData[0]);
		turnMinutes += Number(timeData[1]) / 60;

		let boardSize = "";
		switch (selectedBoardSize) {
			case "Small (5x5)":
				boardSize = "small";
				break;
			case "Medium (7x7)":
				boardSize = "medium";
				break;
			case "Large (9x9)":
				boardSize = "large";
				break;
			default:
				break;
		}

		try {
			const { status, data } = await api.post<HostGameApiResponse>(
				"/api/game",
				{
					boardSize,
					turnMinutes,
				}
			);
			if (status === 200) {
				navigate(`/game?code=${data.gameCode}`);
			}
		} catch (error) {
			const errorData: ApiResponse = handleAxiosError(error);
			console.log("Error:", errorData.message);
		}
	};

	const handleJoinGame = () => {
		if (!gameCode || gameCode.length !== 8) {
			setInputError(true);
			setTimeout(() => setInputError(false), 1000);
			return;
		}
		navigate(`/game?code=${gameCode}`);
	};

	if (dropdown && canDisable) {
		return (
			<div className={`panel-container ${bgColor}`}>
				<h2>{panelTitle}</h2>
				<div className="dropdown-container">
					<Dropdown
						options={options1}
						selected={selectedTime}
						setSelected={setSelectedTime}
					/>
					<Dropdown
						options={options2}
						selected={selectedBoardSize}
						setSelected={setSelectedBoardSize}
					/>
				</div>
				<button
					className={`panel-button ${
						!isLoggedIn ? "disabled-link" : ""
					}`}
					onClick={handleHostGame}
					disabled={!isLoggedIn}
				>
					{buttonText}
				</button>
			</div>
		);
	} else if (dropdown && !canDisable) {
		return (
			<div className={`panel-container ${bgColor}`}>
				<h2>{panelTitle}</h2>
				<div className="dropdown-container">
					<Dropdown
						options={options1}
						selected={difficultyLevel}
						setSelected={setDifficultyLevel}
					/>
					<Dropdown
						options={options2}
						selected={selectedBoardSize}
						setSelected={setSelectedBoardSize}
					/>
				</div>
				<button className="panel-button" onClick={handleHostGame}>
					{buttonText}
				</button>
			</div>
		);
	} else {
		return (
			<div className={`panel-container ${bgColor}`}>
				<h2>{panelTitle}</h2>
				<input
					className={`${inputError ? "inputError" : ""}`}
					type="text"
					value={gameCode}
					onChange={(e) => setGameCode(e.currentTarget.value)}
					placeholder="xxxxxxxx"
				></input>
				<button
					className={`panel-button ${
						!isLoggedIn ? "disabled-link" : ""
					}`}
					onClick={handleJoinGame}
					disabled={!isLoggedIn}
				>
					{buttonText}
				</button>
			</div>
		);
	}
};

export default Panel;
