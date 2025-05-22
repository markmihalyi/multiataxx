import "../styles/Game.css";
import { CellState } from "../constants";

type CellProps = {
	state: CellState;
	onClick: () => void;
	selected: boolean;
};

const Cell: React.FC<CellProps> = ({ state, onClick, selected }) => {
	const cellState = CellState[state];

	return (
		<div
			className={`cell ${cellState} ${selected ? "selected" : ""}`}
			onClick={onClick}
		></div>
	);
};

export default Cell;
