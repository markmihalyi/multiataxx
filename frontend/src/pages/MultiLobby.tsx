import { NavLink } from "react-router";
import Navbar from "../layouts/Navbar";
import CustomButton from "../components/CustomButton";
import { FaArrowRightToBracket } from "react-icons/fa6";
import { BsCloudCheckFill } from "react-icons/bs";
import useAuth from "../common/hooks/useAuth";

function MultiLobby() {
	const { isLoggedIn } = useAuth();
	return (
		<>
			<Navbar />
			<div className="container">
				<NavLink
					to="/multi-join"
					end
					className={`${isLoggedIn ? "" : "disabled-link"}`}
				>
					<CustomButton
						text="Join a game"
						bgColor="button-teal"
						icon={<FaArrowRightToBracket className="button-icon" />}
					/>
				</NavLink>
				<NavLink
					to="/multi-host"
					end
					className={`${isLoggedIn ? "" : "disabled-link"}`}
				>
					<CustomButton
						text="Host a game"
						bgColor="button-purple"
						icon={<BsCloudCheckFill className="button-icon" />}
					/>
				</NavLink>
			</div>
		</>
	);
}

export default MultiLobby;
