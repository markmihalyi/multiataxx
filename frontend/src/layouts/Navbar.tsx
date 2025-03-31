import "../styles/Navbar.css";

import { NavLink } from "react-router";
import Popup from "./Popup";
import useAuth from "../common/hooks/useAuth";
import { useState } from "react";
import { PiUserCircleCheckDuotone } from "react-icons/pi";

export default function Navbar() {
	const [isPopupOpen, setIsPopupOpen] = useState(false);

	const { isLoggedIn } = useAuth();

	return (
		<nav className="navbar">
			<NavLink to="/" className="navbar-brand" end>
				MultiAtaxx
			</NavLink>
			{isLoggedIn ? (
				<div
					className="btn-logged-in"
					onClick={() => setIsPopupOpen(true)}
				>
					<PiUserCircleCheckDuotone />
				</div>
			) : (
				<button className="btn" onClick={() => setIsPopupOpen(true)}>
					Login
				</button>
			)}
			<Popup isOpen={isPopupOpen} setIsOpen={setIsPopupOpen} />
		</nav>
	);
}
