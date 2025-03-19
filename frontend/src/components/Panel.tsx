import { NavLink } from "react-router";
import Dropdown from "../components/Dropdown";
import "../styles/Panel.css";

type PanelProps = {
	panelTitle: string;
	bgColor: string;
	navLink: string;
	buttonText: string;
	dropdown: boolean;
	options1?: string[];
	options2?: string[];
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
	const handleSelect = (value: string) => {
		console.log("Selected:", value);
	};

	if (dropdown) {
		return (
			<div className={`panel-container ${bgColor}`}>
				<h2>{panelTitle}</h2>
				<div className="dropdown-container">
					<Dropdown options={options1} onSelect={handleSelect} />
					<Dropdown options={options2} onSelect={handleSelect} />
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
				<input
					type="number"
					inputMode="numeric" //for mobile
					placeholder="000000"
					pattern="\d{6}"
				></input>
				<NavLink to={`${navLink}`} end>
					<button className="panel-button">{buttonText}</button>
				</NavLink>
			</div>
		);
	}
};

export default Panel;
