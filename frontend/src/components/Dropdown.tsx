import { useState, useRef, useEffect } from "react";
import { motion, AnimatePresence } from "framer-motion";
import "../styles/Panel.css";
import "../styles/Dropdown.css";

interface DropdownProps {
	options: string[];
}

const Dropdown: React.FC<DropdownProps> = ({ options }) => {
	const [isOpen, setIsOpen] = useState(false);
	const [selected, setSelected] = useState<string | null>(options[0] || null);
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
			{/* Dropdown Button */}
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
