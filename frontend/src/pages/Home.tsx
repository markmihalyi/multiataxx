import { NavLink } from "react-router";
import "../styles/Home.css";
import Navbar from "../components/Navbar";
import CustomButton from "../components/Button";
import { MdPeopleAlt, MdPerson } from "react-icons/md";

function Home() {
	return (
		<>
			<Navbar />
			<div className="container">
				<NavLink to="/single-lobby" end>
					<CustomButton
						text="Single Player"
						bgColor="green"
						icon={<MdPerson className="button-icon" />}
					/>
				</NavLink>
				<NavLink to="/multi-lobby" end>
					<CustomButton
						text="Multiplayer"
						bgColor="blue"
						icon={<MdPeopleAlt className="button-icon" />}
					/>
				</NavLink>
			</div>
		</>
	);
}

export default Home;
