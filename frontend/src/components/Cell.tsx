import { CellState } from "../constants";

type CellProps = {
	state: CellState;
	onClick: () => void;
};

const Cell: React.FC<CellProps> = ({ state, onClick }) => {
	return <button onClick={onClick}>{state}</button>;
};

export default Cell;
