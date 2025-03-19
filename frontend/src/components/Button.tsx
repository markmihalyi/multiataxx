import React from "react";
import "../styles/Button.css";

type ButtonProps = {
	text: string;
	bgColor: string;
	icon: React.ReactNode;
};

const CustomButton: React.FC<ButtonProps> = ({ text, bgColor, icon }) => {
	return (
		<button className={`customButton ${bgColor}`}>
			{icon}
			{text}
		</button>
	);
};

export default CustomButton;
