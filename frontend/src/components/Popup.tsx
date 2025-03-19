import React, { useEffect, useRef } from "react";
import "../styles/Popup.css";
import { FaUser } from "react-icons/fa6";

interface PopupProps {
	onClose: () => void;
}

const Popup: React.FC<PopupProps> = ({ onClose }) => {
	const popupRef = useRef<HTMLDivElement>(null);
	const overlayRef = useRef<HTMLDivElement>(null);

	useEffect(() => {
		const handleClickOutside = (event: MouseEvent) => {
			if (
				overlayRef.current &&
				overlayRef.current.contains(event.target as Node) &&
				!popupRef.current?.contains(event.target as Node)
			) {
				onClose();
			}
		};
		document.addEventListener("mousedown", handleClickOutside);
		return () =>
			document.removeEventListener("mousedown", handleClickOutside);
	}, [onClose]);

	return (
		<div className="overlay" ref={overlayRef}>
			<div className="popup" ref={popupRef}>
				<button className="closeButton" onClick={onClose}>
					&times;
				</button>
				<div className="logo">
					<FaUser className="user-icon" />
				</div>
				<h2>Hi there!</h2>
				<input type="text" placeholder="Username" />
				<input type="password" placeholder="Password" />
				<button className="loginButton" onClick={onClose}>
					Login
				</button>
			</div>
		</div>
	);
};

export default Popup;
