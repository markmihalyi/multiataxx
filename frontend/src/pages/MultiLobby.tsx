import { NavLink } from "react-router";
import Navbar from "../components/Navbar";
import CustomButton from "../components/Button";
import { FaArrowRightToBracket } from "react-icons/fa6";
import { BsCloudCheckFill } from "react-icons/bs";

function MultiLobby() {
	return (
		<>
			<Navbar />
			<div className="container">
				<NavLink to="/multi-join" end>
					<CustomButton
						text="Join a game"
						bgColor="teal"
						icon={<FaArrowRightToBracket className="button-icon" />}
					/>
				</NavLink>
				<NavLink to="/multi-host" end>
					<CustomButton
						text="Host a game"
						bgColor="purple"
						icon={<BsCloudCheckFill className="button-icon" />}
					/>
				</NavLink>
			</div>
		</>
	);
}

export default MultiLobby;
