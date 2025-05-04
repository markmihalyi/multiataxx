import { createContext, useState } from "react";

export interface IAuthContext {
	isLoggedIn: boolean;
	setIsLoggedIn: React.Dispatch<React.SetStateAction<boolean>>;
	permanentUsername: string;
	setPermanentUsername: React.Dispatch<React.SetStateAction<string>>;
}

const AuthContext = createContext<IAuthContext>({} as IAuthContext);

const AuthContextProvider: React.FC<React.PropsWithChildren> = (props) => {
	const [isLoggedIn, setIsLoggedIn] = useState(false);
	const [permanentUsername, setPermanentUsername] = useState("");

	return (
		<AuthContext.Provider
			value={{
				isLoggedIn,
				setIsLoggedIn,
				permanentUsername,
				setPermanentUsername,
			}}
		>
			{props.children}
		</AuthContext.Provider>
	);
};

export default AuthContext;
export { AuthContextProvider };
