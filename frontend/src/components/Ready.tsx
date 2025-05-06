import "../styles/Error&Ready.css";
import ThreeDotLoading from "../components/ThreeDotLoading";

type ReadyProps = {
	buttonHandle: () => void;
	isReady: boolean;
};

export default function Ready({ buttonHandle, isReady }: ReadyProps) {
	return (
		<div className="nav-container" id="wide">
			<div className="upper-section" id="green">
				<h2 className="ready-title">
					Copy game code and share it with your opponent:
				</h2>
			</div>
			<div className="lower-section">
				{isReady ? (
					<>
						<p>Waiting for the other player...</p>
						<ThreeDotLoading />
					</>
				) : (
					<>
						<p>Press when ready to join the game.</p>
						<button
							className="ready-button"
							onClick={buttonHandle}
							disabled={isReady}
						>
							Ready
						</button>
					</>
				)}
			</div>
		</div>
	);
}
