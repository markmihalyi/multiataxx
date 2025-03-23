import { NavLink } from "react-router";
import Navbar from "../layouts/Navbar";
import CustomButton from "../components/CustomButton";
import { MdPeopleAlt, MdPerson } from "react-icons/md";
import "../styles/Home.css";

function Home() {
	return (
		<>
			<Navbar />
			<div className="container">
				<NavLink to="/single-lobby" end>
					<CustomButton
						text="Single Player"
						bgColor="button-green"
						icon={<MdPerson className="button-icon" />}
					/>
				</NavLink>
				<NavLink to="/multi-lobby" end>
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
