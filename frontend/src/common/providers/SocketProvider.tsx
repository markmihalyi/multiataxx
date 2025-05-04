import * as signalR from "@microsoft/signalr";

import { createContext, useState } from "react";

import { API_BASE_URL } from "../../api";

export interface ISocketContext {
	socket: signalR.HubConnection | null;
	connect: () => Promise<void>;
	joinGame: (gameCode: string) => Promise<void>;
	sendPlayerIsReady: () => Promise<void>;
	attemptMove: (
		startX: number,
		startY: number,
		destX: number,
		destY: number
	) => Promise<void>;
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
		startX: number,
		startY: number,
		destX: number,
		destY: number
	) => {
		if (socket === null) return;
		await socket.invoke("AttemptMove", [startX, startY, destX, destY]);
	};

	return (
		<SocketContext.Provider
			value={{
				socket,
				connect,
				joinGame,
				sendPlayerIsReady,
				attemptMove,
			}}
		>
			{props.children}
		</SocketContext.Provider>
	);
};

export default SocketContext;
export { SocketContextProvider };
