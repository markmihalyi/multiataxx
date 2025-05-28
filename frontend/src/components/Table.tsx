import React, { useEffect, useState } from "react";

import Cell from "./Cell";
import { CellPosition } from "../types";
import { CellState } from "../constants";
import { Cell as CellType } from "../types";
import { GameState } from "../types";
import useSocket from "../common/hooks/useSocket";

type TableProps = {
	cells: CellType[][];
	ownCellState: CellState;
	gameState: GameState;
};

const Table: React.FC<TableProps> = ({ cells, ownCellState, gameState }) => {
	const [startPosition, setStartPosition] = useState<CellPosition | null>(
		null
	);
	const [destPosition, setDestPosition] = useState<CellPosition | null>(null);

	const { attemptMove } = useSocket();

	const handleCellClick = (state: CellState, coordinate: CellPosition) => {
		if (state === ownCellState && gameState === `Player${state}Turn`) {
			//console.log("startPosition", coordinate);
			setStartPosition(coordinate);
		} else if (state === CellState.Empty && startPosition !== null) {
			//console.log("destPosition", coordinate);
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
		<div
			className="table"
			style={{
				gridTemplateColumns: `repeat(${cells[0].length}, 1fr)`,
				gridTemplateRows: `repeat(${cells.length}, 1fr)`,
			}}
		>
			{cells.map((cellRow, row) =>
				cellRow.map((cell, col) => (
					<Cell
						key={`${row}-${col}`}
						cellData={cell}
						onClick={() =>
							handleCellClick(cell.state, { row, col })
						}
						selected={
							startPosition?.row === row &&
							startPosition?.col === col
						}
					/>
				))
			)}
		</div>
	);
};

export default Table;
