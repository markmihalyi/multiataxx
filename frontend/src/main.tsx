import ReactDOM from "react-dom/client";
import { BrowserRouter, Routes, Route } from "react-router";
import Home from "./pages/Home";
import SingleLobby from "./pages/SingleLobby";
import MultiLobby from "./pages/MultiLobby";

const root = document.getElementById("root");

ReactDOM.createRoot(root!).render(
	<BrowserRouter>
		<Routes>
			<Route path="/" element={<Home />} />
			<Route path="/single-lobby" element={<SingleLobby />} />
			<Route path="/multi-lobby" element={<MultiLobby />} />
		</Routes>
	</BrowserRouter>
);
