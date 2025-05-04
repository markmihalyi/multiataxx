import "../styles/Register.css";

import { IoMdLock } from "react-icons/io";
import { MdPerson } from "react-icons/md";
import Navbar from "../layouts/Navbar";
import api from "../api";
import { handleAxiosError } from "../api";
import { useState } from "react";

function Register() {
	const [username, setUsername] = useState("");
	const [password, setPassword] = useState("");
	const [passwordConfirm, setPasswordConfirmed] = useState("");
	const [isSuccess, setIsSuccess] = useState(false);
	const [message, setMessage] = useState("-----");

	const handleRegister = async (e: React.FormEvent<HTMLFormElement>) => {
		e.preventDefault(); //prevent reloading

		try {
			if (password !== passwordConfirm) {
				setMessage("Passwords do not match.");
				setTimeout(() => {
					setMessage("-----");
				}, 3000);
				return;
			}

			const response = await api.post<ApiResponse>("/api/auth/register", {
				username,
				password,
			});

			const successData: ApiResponse = response.data;
			setMessage(successData.message);
			setIsSuccess(successData.success);
			setUsername("");
			setPassword("");
			setPasswordConfirmed("");

			setTimeout(() => {
				setMessage("-----");
			}, 10000);
		} catch (error) {
			const errorData: ApiResponse = handleAxiosError(error);
			setMessage(errorData.message);

			setTimeout(() => {
				setMessage("-----");
			}, 3000);
		}
	};

	return (
		<>
			<Navbar />
			<div className="container" id="cont">
				<div className="register-container">
					<h2>Registration form</h2>

					<form onSubmit={handleRegister} className="register-form">
						<div className="input-group">
							<label>Username:</label>
							<input
								type="text"
								value={username}
								onChange={(e) =>
									setUsername(e.currentTarget.value)
								}
								placeholder="Username (3-20 characters)"
								autoComplete="username"
								required
							/>
							<i>
								<MdPerson />
							</i>
						</div>

						<div className="input-group">
							<label>New password:</label>
							<input
								type="password"
								value={password}
								onChange={(e) =>
									setPassword(e.currentTarget.value)
								}
								placeholder="Password (6-32 characters)"
								autoComplete="new-password"
								required
							/>
							<i>
								<IoMdLock />
							</i>
						</div>

						<div className="input-group">
							<label>Confirm new password:</label>
							<input
								type="password"
								value={passwordConfirm}
								onChange={(e) =>
									setPasswordConfirmed(e.currentTarget.value)
								}
								placeholder="Password again"
								autoComplete="new-password"
								required
							/>
							<i>
								<IoMdLock />
							</i>
						</div>

						<p className={`${isSuccess ? "success" : "error"}`}>
							{message}
						</p>

						<button type="submit" className="registerButton">
							Register
						</button>
					</form>
				</div>
			</div>
		</>
	);
}

export default Register;
