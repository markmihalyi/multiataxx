import Navbar from "../layouts/Navbar";
import "../styles/Register.css";
import { IoMdLock } from "react-icons/io";
import { MdPerson } from "react-icons/md";

function Register() {
	const handleRegister = () => {
		//TODO
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
								placeholder="Create a creative username"
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
								placeholder="Password"
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
								placeholder="Password again"
								autoComplete="new-password"
								required
							/>
							<i>
								<IoMdLock />
							</i>
						</div>

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
