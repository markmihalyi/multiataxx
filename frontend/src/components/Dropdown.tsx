import "../styles/Panel.css";
import "../styles/Dropdown.css";

import { AnimatePresence, motion } from "framer-motion";
import { useEffect, useRef, useState } from "react";

type DropdownProps = {
	options: string[];
	selected: string | null;
	setSelected: React.Dispatch<React.SetStateAction<string | null>>;
};

const Dropdown: React.FC<DropdownProps> = ({
	options,
	selected,
	setSelected,
}) => {
	const [isOpen, setIsOpen] = useState(false);
	const dropdownRef = useRef<HTMLDivElement>(null);

	// Toggle dropdown open/close
	const toggleDropdown = () => setIsOpen((prev) => !prev);

	// Handle option selection
	const handleSelect = (option: string) => {
		setSelected(option);
		setIsOpen(false);
	};

	// Close dropdown if clicked outside
	useEffect(() => {
		const handleClickOutside = (event: MouseEvent) => {
			if (
				dropdownRef.current &&
				!dropdownRef.current.contains(event.target as Node)
			) {
				setIsOpen(false);
			}
		};
		document.addEventListener("mousedown", handleClickOutside);
		return () =>
			document.removeEventListener("mousedown", handleClickOutside);
	}, []);

	return (
		<div className="dropdown-ref" ref={dropdownRef}>
			<button className="dropdown-button" onClick={toggleDropdown}>
				{selected}
				<span className={`arrow ${isOpen ? "rotate-180" : ""}`}>▼</span>
			</button>

			{/* Dropdown Menu */}
			<AnimatePresence>
				{isOpen && (
					<motion.ul
						className="list"
						initial={{ opacity: 0, y: -10 }}
						animate={{ opacity: 1, y: 0 }}
						exit={{ opacity: 0, y: -10 }}
						transition={{ duration: 0.2 }}
					>
						{options.map((option, index) => (
							<li
								key={index}
								className={`list-item ${
									selected === option ? "selected-item" : ""
								}`}
								onClick={() => handleSelect(option)}
							>
								{option}
							</li>
						))}
					</motion.ul>
				)}
			</AnimatePresence>
		</div>
	);
};

export default Dropdown;
