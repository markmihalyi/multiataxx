import { BrowserRouter, Route, Routes } from "react-router";

import { AuthContextProvider } from "./common/providers/AuthProvider";
import Game from "./pages/Game";
import Home from "./pages/Home";
import MultiHost from "./pages/MultiHost";
import MultiJoin from "./pages/MultiJoin";
import MultiLobby from "./pages/MultiLobby";
import ReactDOM from "react-dom/client";
import Register from "./pages/Register";
import SingleLobby from "./pages/SingleLobby";
import { SocketContextProvider } from "./common/providers/SocketProvider";

const root = document.getElementById("root");

ReactDOM.createRoot(root!).render(
	<AuthContextProvider>
		<SocketContextProvider>
			<BrowserRouter>
				<Routes>
					<Route index element={<Home />} />
					<Route path="single-lobby" element={<SingleLobby />} />
					<Route path="multi-lobby" element={<MultiLobby />} />
					<Route path="multi-join" element={<MultiJoin />} />
					<Route path="multi-host" element={<MultiHost />} />
					<Route path="game" element={<Game />} />
					<Route path="register" element={<Register />} />
				</Routes>
			</BrowserRouter>
		</SocketContextProvider>
	</AuthContextProvider>
);
