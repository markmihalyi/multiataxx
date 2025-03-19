import Navbar from "../components/Navbar";
import Panel from "../components/Panel";

function MultiHost() {
	return (
		<>
			<Navbar />
			<div className="container">
				<Panel
					panelTitle="Set total time for turns & field size"
					bgColor="panel-purple"
					navLink="/room-hosted"
					buttonText="Create Game"
					dropdown={true}
					options1={[
						"00:30",
						"1:00",
						"2:00",
						"3:00",
						"5:00",
						"10:00",
						"20:00",
						"30:00",
					]}
					options2={["Small (5x5)", "Medium (9x9)", "Large (11x11)"]}
				/>
			</div>
		</>
	);
}

export default MultiHost;
