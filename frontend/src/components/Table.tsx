import React from "react";

type TableProps = {
	cells: CellState[][];
};

const Table: React.FC<TableProps> = ({ cells }) => {
	console.log(cells.length);
	return <>Table</>;
};

export default Table;
