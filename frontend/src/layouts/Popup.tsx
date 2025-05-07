import "../styles/Popup.css";

import { NavLink, useNavigate } from "react-router";
import React, { useCallback, useEffect, useState } from "react";

import { ApiResponse } from "../types";
import { FaUnlockAlt } from "react-icons/fa";
import { FaUser } from "react-icons/fa6";
import api from "../api";
import { handleAxiosError } from "../api";
import useAuth from "../common/hooks/useAuth";

interface PopupProps {
	isOpen: boolean;
	setIsOpen: React.Dispatch<React.SetStateAction<boolean>>;
}

const Popup: React.FC<PopupProps> = ({ isOpen, setIsOpen }) => {
	const navigate = useNavigate();

	const [isVisible, setIsVisible] = useState(false);
	const [username, setUsername] = useState("");
	const [password, setPassword] = useState("");
	const [errorMessage, setErrorMessage] = useState<string | null>(null);
	const [inputError, setInputError] = useState<boolean>(false);

	const {
		isLoggedIn,
		setIsLoggedIn,
		updateUserData,
		username: authUsername,
	} = useAuth();

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

			setTimeout(async () => {
				await updateUserData();
				navigate("/");
			}, 320);
		} catch (error) {
			const errorData: ApiResponse = handleAxiosError(error);
			setErrorMessage(errorData.message);
			setInputError(true);

			setTimeout(() => {
				setErrorMessage(null);
				setInputError(false);
			}, 1500);
		}
	};

	const handleLogOut = async () => {
		try {
			await api.post("/api/auth/logout");
			setIsOpen(false);

			setTimeout(() => {
				setIsLoggedIn(false);
				navigate("/");
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
							className={`${inputError ? "inputError" : ""}`}
							type="text"
							value={username}
							onInput={(e) => setUsername(e.currentTarget.value)}
							placeholder="Username"
							autoComplete="username"
							required
						/>
						<input
							className={`${inputError ? "inputError" : ""}`}
							type="password"
							value={password}
							onInput={(e) => setPassword(e.currentTarget.value)}
							placeholder="Password"
							autoComplete="current-password"
							required
						/>
						<button type="submit" className="loginButton w80">
							Login
						</button>
						{errorMessage ? (
							<p id="errorMessage">{errorMessage}</p>
						) : (
							<p>
								No account yet?{" "}
								<NavLink to="/register" id="register-link" end>
									Register here!
								</NavLink>
							</p>
						)}
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
					<div className="logout-popup-circle">
						<FaUnlockAlt />
					</div>
					<p>User logged in:</p>
					<p id="p-bottom">
						<b>{authUsername}</b>
					</p>
					<button className="loginButton" onClick={handleLogOut}>
						Log out
					</button>
				</div>
			</div>
		);
	}
};

export default Popup;
