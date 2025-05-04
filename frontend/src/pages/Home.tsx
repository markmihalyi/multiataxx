import "../styles/Home.css";

import { NavLink } from "react-router";
import Navbar from "../layouts/Navbar";
import CustomButton from "../components/CustomButton";
import { MdPeopleAlt, MdPerson } from "react-icons/md";
import useAuth from "../common/hooks/useAuth";

function Home() {
	const { isLoggedIn } = useAuth();
	return (
		<>
			<Navbar />
			<div className="container">
				<NavLink to="/single-lobby" end>
					<CustomButton
						text="Single player"
						bgColor="button-green"
						icon={<MdPerson className="button-icon" />}
					/>
				</NavLink>
				<NavLink
					to="/multi-lobby"
					end
					className={`${isLoggedIn ? "" : "disabled-link"}`}
				>
					<CustomButton
						text="Multiplayer"
						bgColor="button-blue"
						icon={<MdPeopleAlt className="button-icon" />}
					/>
				</NavLink>
			</div>
		</>
	);
}

export default Home;
