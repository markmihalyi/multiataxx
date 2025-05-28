import Navbar from "../layouts/Navbar";
import Panel from "../components/Panel";

function SingleLobby() {
	return (
		<>
			<Navbar />
			<div className="container">
				<Panel
					panelTitle="Difficulty level & field size"
					bgColor="panel-green"
					navLink="/game"
					buttonText="▶ Play"
					dropdown={true}
					canDisable={false}
					options1={["Easy", "Medium", "Hard"]}
					options2={["Small (5x5)", "Medium (7x7)", "Large (9x9)"]}
					gameType="singleplayer"
				/>
			</div>
		</>
	);
}

export default SingleLobby;
