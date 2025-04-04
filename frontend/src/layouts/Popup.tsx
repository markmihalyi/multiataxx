import "../styles/Popup.css";

import React, { useEffect, useState, useCallback } from "react";

import { FaUser } from "react-icons/fa6";
import { FaUnlockAlt } from "react-icons/fa";
import { NavLink } from "react-router";
import api from "../axios";
import { handleAxiosError } from "../axios";
import useAuth from "../common/hooks/useAuth";

interface PopupProps {
	isOpen: boolean;
	setIsOpen: React.Dispatch<React.SetStateAction<boolean>>;
}

const Popup: React.FC<PopupProps> = ({ isOpen, setIsOpen }) => {
	const [isVisible, setIsVisible] = useState(false);
	const [username, setUsername] = useState("");
	const [password, setPassword] = useState("");
	const [errorMessage, setErrorMessage] = useState<string | null>(null);

	const { isLoggedIn, setIsLoggedIn } = useAuth();

	const handleLogin = async (e: React.FormEvent<HTMLFormElement>) => {
		e.preventDefault(); //prevent reloading

		try {
			await api.post<ApiResponse>("/api/auth/login", {
				username,
				password,
			});
			setUsername("");
			setPassword("");
			setErrorMessage(null);
			setIsOpen(false);

			setTimeout(() => {
				setIsLoggedIn(true);
			}, 320);
		} catch (error) {
			const errorData: ApiResponse = handleAxiosError(error);
			setErrorMessage(errorData.message);

			setTimeout(() => {
				setErrorMessage(null);
			}, 3000);
		}
	};

	const handleLogOut = async () => {
		try {
			await api.post("/api/auth/logout");
			setIsOpen(false);

			setTimeout(() => {
				setIsLoggedIn(false);
			}, 320);
		} catch (error) {
			const errorData: ApiResponse = handleAxiosError(error);
			setErrorMessage(errorData.message);
		}
	};

	// closes immediately but only erases
	// input fields & error message later
	// when closeing animation is finished
	const handleClose = useCallback(() => {
		setIsOpen(false);
		setTimeout(() => {
			setUsername("");
			setPassword("");
			setErrorMessage(null);
		}, 320);
	}, [setUsername, setPassword, setIsOpen]);

	// pressing Esc closes popup
	useEffect(() => {
		const handleKeyDown = (event: KeyboardEvent) => {
			if (event.key === "Escape") {
				handleClose();
			}
		};

		if (isOpen) {
			document.addEventListener("keydown", handleKeyDown);
		}

		return () => {
			document.removeEventListener("keydown", handleKeyDown);
		};
	}, [isOpen, handleClose]);

	useEffect(() => {
		if (isOpen) {
			setIsVisible(true);
		} else {
			setTimeout(() => setIsVisible(false), 300); //Wait for animation to close
		}
	}, [isOpen]);

	if (!isLoggedIn) {
		return (
			<div
				className={`overlay ${isOpen ? "fade-in" : "fade-out"} ${
					isVisible ? "" : "hidden"
				}`}
			>
				<div
					className={`login-popup ${
						isOpen ? "slide-in" : "slide-out"
					}`}
					onClick={(e) => e.stopPropagation()}
				>
					<button className="closeButton" onClick={handleClose}>
						&times;
					</button>
					<div className="logo">
						<FaUser className="user-icon" />
					</div>
					<h2>Hi there!</h2>
					<form onSubmit={handleLogin} className="login-form">
						<input
							type="text"
							value={username}
							onInput={(e) => setUsername(e.currentTarget.value)}
							placeholder="Username"
							autoComplete="username"
							required
						/>
						<input
							type="password"
							value={password}
							onInput={(e) => setPassword(e.currentTarget.value)}
							placeholder="Password"
							autoComplete="current-password"
							required
						/>
						{errorMessage && (
							<p id="errorMessage">{errorMessage}</p>
						)}
						<button type="submit" className="loginButton w80">
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
	} else {
		return (
			<div
				className={`overlay ${isOpen ? "fade-in" : "fade-out"} ${
					isVisible ? "" : "hidden"
				}`}
				onClick={handleClose} // Close when clicking outside
			>
				<div
					className={`logout-popup ${
						isOpen ? "slide-in" : "slide-out"
					}`}
					onClick={(e) => e.stopPropagation()}
				>
					<button className="closeButton" onClick={handleClose}>
						&times;
					</button>
					<div className="logout-popup-inner">
						<FaUnlockAlt />
					</div>
					<NavLink to="/" end>
						<button className="loginButton" onClick={handleLogOut}>
							Log out
						</button>
					</NavLink>
				</div>
			</div>
		);
	}
};

export default Popup;
