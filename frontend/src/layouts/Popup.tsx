import React, { useEffect, useState } from "react";
import { NavLink } from "react-router";
import "../styles/Popup.css";
import { FaUser } from "react-icons/fa6";

interface PopupProps {
	isOpen: boolean;
	onClose: () => void;
}

const Popup: React.FC<PopupProps> = ({ isOpen, onClose }) => {
	const [isVisible, setIsVisible] = useState(false);

	const handleLogin = () => {
		//TODO
	};

	useEffect(() => {
		if (isOpen) {
			setIsVisible(true);
		} else {
			setTimeout(() => setIsVisible(false), 300); //Wait for animation to close
		}
	}, [isOpen]);

	return (
		<div
			className={`overlay ${isOpen ? "fade-in" : "fade-out"} ${
				isVisible ? "" : "hidden"
			}`}
			// onClick={onClose}
		>
			<div
				className={`popup ${isOpen ? "slide-in" : "slide-out"}`}
				onClick={(e) => e.stopPropagation()}
			>
				<button className="closeButton" onClick={onClose}>
					&times;
				</button>
				<div className="logo">
					<FaUser className="user-icon" />
				</div>
				<h2>Hi there!</h2>
				<form onSubmit={handleLogin} className="login-form">
					<input
						type="text"
						placeholder="Username"
						autoComplete="username"
						required
					/>
					<input
						type="password"
						placeholder="Password"
						autoComplete="current-password"
						required
					/>
					<button type="submit" className="loginButton">
						Login
					</button>
					<p>
						No account yet?{" "}
						<NavLink to="/register" id="register-link" end>
							Register here!
						</NavLink>
					</p>
				</form>
			</div>
		</div>
	);
};

export default Popup;
