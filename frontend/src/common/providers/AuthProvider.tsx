import { createContext, useState } from "react";

export interface IAuthContext {
	isLoggedIn: boolean;
	setIsLoggedIn: React.Dispatch<React.SetStateAction<boolean>>;
}

const AuthContext = createContext<IAuthContext>({} as IAuthContext);

const AuthContextProvider: React.FC<React.PropsWithChildren> = (props) => {
	const [isLoggedIn, setIsLoggedIn] = useState(false);

	return (
		<AuthContext.Provider value={{ isLoggedIn, setIsLoggedIn }}>
			{props.children}
		</AuthContext.Provider>
	);
};

export default AuthContext;
export { AuthContextProvider };
