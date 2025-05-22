import SocketContext, { ISocketContext } from "../providers/SocketProvider";

import { useContext } from "react";

const useSocket = () => {
	return useContext<ISocketContext>(SocketContext);
};

export default useSocket;
