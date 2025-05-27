import Navbar from "../layouts/Navbar";
import Panel from "../components/Panel";

function MultiHost() {
	return (
		<>
			<Navbar />
			<div className="container">
				<Panel
					panelTitle="Set total time for turns & field size"
					bgColor="panel-purple"
					navLink="/game"
					buttonText="Create Game"
					dropdown={true}
					canDisable={true}
					options1={[
						"00:30",
						"01:00",
						"02:00",
						"03:00",
						"05:00",
						"10:00",
						"20:00",
						"30:00",
					]}
					options2={["Small (5x5)", "Medium (7x7)", "Large (9x9)"]}
					gameType="multiplayer"
				/>
			</div>
		</>
	);
}

export default MultiHost;
