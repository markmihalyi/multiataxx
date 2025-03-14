import React from "react";
import "../styles/Button.css";

type ButtonProps = {
	text: string;
	bgColor: string;
};

const CustomButton: React.FC<ButtonProps> = ({ text, bgColor }) => {
	return <button className={`customButton ${bgColor}`}>{text}</button>;
};

export default CustomButton;
