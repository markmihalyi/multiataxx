import Navbar from "../components/Navbar";
import Panel from "../components/Panel";

function SingleLobby() {
	return (
		<>
			<Navbar />
			<div className="container">
				<Panel
					panelTitle="Difficulty level & field size"
					bgColor="panel-green"
					navLink="/singleplayer"
					buttonText="▶ Play"
					dropdown={true}
					options1={["Easy", "Medium", "Hard"]}
					options2={["Small (5x5)", "Medium (9x9)", "Large (11x11)"]}
				/>
			</div>
		</>
	);
}

export default SingleLobby;
