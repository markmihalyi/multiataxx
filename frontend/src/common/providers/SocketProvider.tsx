import * as signalR from "@microsoft/signalr";

import { createContext, useState } from "react";

import { API_BASE_URL } from "../../api";
import { CellPosition } from "../../types";

export interface ISocketContext {
	socket: signalR.HubConnection | null;
	connect: () => Promise<void>;
	joinGame: (gameCode: string) => Promise<void>;
	sendPlayerIsReady: () => Promise<void>;
	attemptMove: (
		startPosition: CellPosition,
		destPosition: CellPosition
	) => Promise<void>;
	tryUseBooster: (id: number) => Promise<void>;
}

const SocketContext = createContext<ISocketContext>({} as ISocketContext);

const SocketContextProvider: React.FC<React.PropsWithChildren> = (props) => {
	const [socket, setSocket] = useState<signalR.HubConnection | null>(null);

	const connect = async () => {
		const connection = new signalR.HubConnectionBuilder()
			.withUrl(`${API_BASE_URL}/game`, {
				withCredentials: true,
			})
			.withAutomaticReconnect()
			.build();
		await connection.start();
		setSocket(connection);
	};

	const joinGame = async (gameCode: string) => {
		if (socket === null) return;
		await socket.invoke("JoinGame", gameCode);
	};

	const sendPlayerIsReady = async () => {
		if (socket === null) return;
		await socket.invoke("PlayerIsReady");
	};

	const attemptMove = async (
		startPosition: CellPosition,
		destPoisiton: CellPosition
	) => {
		if (socket === null) return;
		await socket.invoke(
			"AttemptMove",
			startPosition.row + 1,
			startPosition.col + 1,
			destPoisiton.row + 1,
			destPoisiton.col + 1
		);
	};

	const tryUseBooster = async (id: number) => {
		if (socket === null) return;
		await socket.invoke("UseBooster", id);
	};

	return (
		<SocketContext.Provider
			value={{
				socket,
				connect,
				joinGame,
				sendPlayerIsReady,
				attemptMove,
				tryUseBooster,
			}}
		>
			{props.children}
		</SocketContext.Provider>
	);
};

export default SocketContext;
export { SocketContextProvider };
