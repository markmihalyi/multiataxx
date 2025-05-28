import "../styles/Game.css";

import { CellState } from "../constants";
import { Cell as CellType } from "../types";

type CellProps = {
	cellData: CellType;
	onClick: () => void;
	selected: boolean;
};

const Cell: React.FC<CellProps> = ({ cellData, onClick, selected }) => {
	const cellState = CellState[cellData.state];

	return (
		<div
			className={`cell ${cellState} ${selected ? "selected" : ""}`}
			id={
				cellData.isTipStartPoint
					? "start"
					: cellData.isTipDestPoint
					? "dest"
					: ""
			}
			onClick={onClick}
		></div>
	);
};

export default Cell;
