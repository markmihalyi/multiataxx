import api, { handleAxiosError } from "../../api";
import { createContext, useEffect, useState } from "react";

export interface IAuthContext {
	isLoggedIn: boolean;
	setIsLoggedIn: React.Dispatch<React.SetStateAction<boolean>>;
}

const AuthContext = createContext<IAuthContext>({} as IAuthContext);

const AuthContextProvider: React.FC<React.PropsWithChildren> = (props) => {
	const [isLoggedIn, setIsLoggedIn] = useState(false);

	useEffect(() => {
		async function fetchData() {
			try {
				const { status } = await api.post("/api/auth/refresh");
				if (status === 200) {
					setIsLoggedIn(true);
				}
			} catch (error) {
				const errorData: ApiResponse = handleAxiosError(error);
				console.log("Error:", errorData.message);
			}
		}
		fetchData();
	}, []);

	return (
		<AuthContext.Provider
			value={{
				isLoggedIn,
				setIsLoggedIn,
			}}
		>
			{props.children}
		</AuthContext.Provider>
	);
};

export default AuthContext;
export { AuthContextProvider };
