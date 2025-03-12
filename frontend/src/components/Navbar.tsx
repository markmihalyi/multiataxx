import { NavLink } from "react-router";
import "../styles/Navbar.css";

export default function Navbar() {
	return (
		<nav className="navbar">
			<NavLink to="/" className="navbar-brand" end>
				MultiAtaxx
			</NavLink>
			<button className="btn" type="submit">
				Log in
			</button>
		</nav>
	);
}
