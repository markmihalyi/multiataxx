import ReactDOM from "react-dom/client";
import { BrowserRouter, Routes, Route } from "react-router";
import Home from "./pages/Home";
import SingleLobby from "./pages/SingleLobby";
import MultiLobby from "./pages/MultiLobby";
import MultiJoin from "./pages/MultiJoin";
import MultiHost from "./pages/MultiHost";
import Game from "./pages/Game";
import Register from "./pages/Register";

const root = document.getElementById("root");

ReactDOM.createRoot(root!).render(
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
);
