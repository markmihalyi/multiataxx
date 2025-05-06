import { useNavigate } from "react-router";
import { VscError } from "react-icons/vsc";
import "../styles/Error&Ready.css";

export default function Error() {
	const navigate = useNavigate();

	const handleGoBack = () => {
		navigate(-1);
	};

	return (
		<div className="nav-container">
			<div className="upper-section" id="red">
				<VscError className="error-icon" />
				<p className="error-title">Error</p>
			</div>
			<div className="lower-section">
				<p>Could not connect to the game.</p>
				<button className="error-button" onClick={handleGoBack}>
					Go back
				</button>
			</div>
		</div>
	);
}
