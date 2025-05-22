import { useState } from "react";
import "../styles/GameCodeCopier.css";
import { FaCheck } from "react-icons/fa6";
import { TbCopy } from "react-icons/tb";
import { motion, AnimatePresence } from "framer-motion";

type CopyProps = {
	gameCode: string;
};

export default function GameCodeCopier({ gameCode }: CopyProps) {
	const [copied, setCopied] = useState(false);
	//console.log("GameCodeDisplay rendered");

	const handleCopy = async () => {
		try {
			await navigator.clipboard.writeText(gameCode);
			setCopied(true);
			setTimeout(() => setCopied(false), 1500);
		} catch (err) {
			console.error("Failed to copy:", err);
		}
	};

	return (
		<div className="game-code-container">
			<input
				className="game-code-input"
				type="text"
				value={gameCode}
				readOnly
			/>
			<div className="copy-button" onClick={handleCopy}>
				<AnimatePresence mode="wait" initial={false}>
					{copied ? (
						<motion.div
							key="check"
							initial={{ opacity: 0 }}
							animate={{ opacity: 1 }}
							exit={{ opacity: 0 }}
							transition={{ duration: 0.1 }}
							style={{
								display: "flex",
								alignItems: "center",
								justifyContent: "center",
							}}
						>
							<FaCheck />
						</motion.div>
					) : (
						<motion.div
							key="copy"
							initial={{ opacity: 0 }}
							animate={{ opacity: 1 }}
							exit={{ opacity: 0 }}
							transition={{ duration: 0.1 }}
							style={{
								display: "flex",
								alignItems: "center",
								justifyContent: "center",
							}}
						>
							<TbCopy />
						</motion.div>
					)}
				</AnimatePresence>
			</div>
		</div>
	);
}
