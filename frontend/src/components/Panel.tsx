import { NavLink } from "react-router";
import Dropdown from "../components/Dropdown";
import "../styles/Panel.css";

type PanelProps = {
	panelTitle: string;
	bgColor: string;
	navLink: string;
	buttonText: string;
	dropdown: boolean;
	options1: string[];
	options2: string[];
};

const Panel: React.FC<PanelProps> = ({
	panelTitle,
	bgColor,
	navLink,
	buttonText,
	dropdown,
	options1,
	options2,
}) => {
	if (dropdown) {
		return (
			<div className={`panel-container ${bgColor}`}>
				<h2>{panelTitle}</h2>
				<div className="dropdown-container">
					<Dropdown options={options1} />
					<Dropdown options={options2} />
				</div>
				<NavLink to={`${navLink}`} end>
					<button className="panel-button">{buttonText}</button>
				</NavLink>
			</div>
		);
	} else {
		return (
			<div className={`panel-container ${bgColor}`}>
				<h2>{panelTitle}</h2>
				<input type="text" placeholder="xxxxxxxx"></input>
				<NavLink to={`${navLink}`} end>
					<button className="panel-button">{buttonText}</button>
				</NavLink>
			</div>
		);
	}
};

export default Panel;
