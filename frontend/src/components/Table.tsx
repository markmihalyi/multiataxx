import React, { useEffect, useState } from "react";

import Cell from "./Cell";
import { CellPosition } from "../types";
import { CellState } from "../constants";
import useSocket from "../common/hooks/useSocket";

type TableProps = {
	cells: CellState[][];
	ownCellState: CellState;
};

const Table: React.FC<TableProps> = ({ cells, ownCellState }) => {
	const [startPosition, setStartPosition] = useState<CellPosition | null>(
		null
	);
	const [destPosition, setDestPosition] = useState<CellPosition | null>(null);

	const { attemptMove } = useSocket();

	const handleCellClick = (state: CellState, coordinate: CellPosition) => {
		if (state === ownCellState) {
			console.log("startPosition", coordinate);
			setStartPosition(coordinate);
		} else if (state === CellState.Empty) {
			console.log("destPosition", coordinate);
			setDestPosition(coordinate);
		}
	};

	useEffect(() => {
		async function makeMove() {
			if (startPosition === null || destPosition === null) return;
			await attemptMove(startPosition, destPosition);
			setStartPosition(null);
			setDestPosition(null);
		}
		makeMove();
		// eslint-disable-next-line react-hooks/exhaustive-deps
	}, [destPosition]);

	return (
		<div>
			<h1>Table</h1>
			{cells.map((cellRow: CellState[], row) => (
				<div key={row}>
					{cellRow.map((cellState: CellState, col) => (
						<Cell
							key={col}
							state={cellState}
							onClick={() =>
								handleCellClick(cellState, { row, col })
							}
						/>
					))}
				</div>
			))}
		</div>
	);
};

export default Table;
