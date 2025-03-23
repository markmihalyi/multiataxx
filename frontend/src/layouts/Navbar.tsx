import { useState } from "react";
import { NavLink } from "react-router";
import Popup from "./Popup";
import "../styles/Navbar.css";

export default function Navbar() {
	const [isPopupOpen, setIsPopupOpen] = useState(false);

	return (
		<nav className="navbar">
			<NavLink to="/" className="navbar-brand" end>
				MultiAtaxx
			</NavLink>
			<button className="btn" onClick={() => setIsPopupOpen(true)}>
				Login
			</button>
			<Popup isOpen={isPopupOpen} onClose={() => setIsPopupOpen(false)} />
		</nav>
	);
}
