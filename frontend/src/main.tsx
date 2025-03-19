import ReactDOM from "react-dom/client";
import { BrowserRouter, Routes, Route } from "react-router";
import Home from "./pages/Home";
import SingleLobby from "./pages/SingleLobby";
import MultiLobby from "./pages/MultiLobby";
import MultiJoin from "./pages/MultiJoin";
import MultiHost from "./pages/MultiHost";
import SinglePlayer from "./pages/SinglePlayer";
import RoomHosted from "./pages/RoomHosted";
import RoomJoined from "./pages/RoomJoined";

const root = document.getElementById("root");

ReactDOM.createRoot(root!).render(
	<BrowserRouter>
		<Routes>
			<Route path="/" element={<Home />} />
			<Route path="/single-lobby" element={<SingleLobby />} />
			<Route path="/multi-lobby" element={<MultiLobby />} />
			<Route path="/multi-join" element={<MultiJoin />} />
			<Route path="/multi-host" element={<MultiHost />} />
			<Route path="/singleplayer" element={<SinglePlayer />} />
			<Route path="/room-hosted" element={<RoomHosted />} />
			<Route path="/room-joined" element={<RoomJoined />} />
		</Routes>
	</BrowserRouter>
);
