import { NavLink } from "react-router";
import Dropdown from "../components/Dropdown";
import "../styles/Panel.css";
import useAuth from "../common/hooks/useAuth";

type PanelProps = {
	panelTitle: string;
	bgColor: string;
	navLink: string;
	buttonText: string;
	dropdown: boolean;
	canDisable: boolean;
	options1: string[];
	options2: string[];
};

const Panel: React.FC<PanelProps> = ({
	panelTitle,
	bgColor,
	navLink,
	buttonText,
	dropdown,
	canDisable,
	options1,
	options2,
}) => {
	const { isLoggedIn } = useAuth();
	if (dropdown && canDisable) {
		return (
			<div className={`panel-container ${bgColor}`}>
				<h2>{panelTitle}</h2>
				<div className="dropdown-container">
					<Dropdown options={options1} />
					<Dropdown options={options2} />
				</div>
				<NavLink
					to={`${navLink}`}
					end
					className={`${isLoggedIn ? "" : "disabled-link"}`}
				>
					<button className="panel-button">{buttonText}</button>
				</NavLink>
			</div>
		);
	} else if (dropdown && !canDisable) {
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
				<NavLink
					to={`${navLink}`}
					end
					className={`${isLoggedIn ? "" : "disabled-link"}`}
				>
					<button className="panel-button">{buttonText}</button>
				</NavLink>
			</div>
		);
	}
};

export default Panel;
