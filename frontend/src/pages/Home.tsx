import { NavLink } from "react-router";
import "../styles/Home.css";
import Navbar from "../components/Navbar";
import CustomButton from "../components/Button";

function Home() {
	return (
		<>
			<Navbar />
			<div className="container">
				<NavLink to="/single-lobby" end>
					<CustomButton text="Single Player" bgColor="green" />
				</NavLink>
				<NavLink to="/multi-lobby" end>
					<CustomButton text="Multiplayer" bgColor="red" />
				</NavLink>
			</div>
		</>
	);
}

export default Home;
