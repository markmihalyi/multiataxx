import Navbar from "../layouts/Navbar";
import Panel from "../components/Panel";

function MultiJoin() {
	return (
		<>
			<Navbar />
			<div className="container">
				<Panel
					panelTitle="Enter game code:"
					bgColor="panel-teal"
					navLink="/game"
					buttonText="Join Game"
					dropdown={false}
					options1={[]}
					options2={[]}
				/>
			</div>
		</>
	);
}

export default MultiJoin;
