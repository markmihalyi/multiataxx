import { createContext, useEffect, useState } from "react";

import { MeApiResponse } from "../../types";
import api from "../../api";

export interface IAuthContext {
	isLoggedIn: boolean;
	setIsLoggedIn: React.Dispatch<React.SetStateAction<boolean>>;
	updateUserData: () => Promise<void>;
	userId: number | null;
	username: string | null;
}

const AuthContext = createContext<IAuthContext>({} as IAuthContext);

const AuthContextProvider: React.FC<React.PropsWithChildren> = (props) => {
	const [isLoggedIn, setIsLoggedIn] = useState(false);
	const [userId, setUserId] = useState<number | null>(null);
	const [username, setUsername] = useState<string | null>(null);

	const updateUserData = async () => {
		try {
			const { data, status } = await api.get<MeApiResponse>(
				"/api/auth/me"
			);
			if (status === 200) {
				setUserId(data.id);
				setUsername(data.username);
				setIsLoggedIn(true);
			}
		} catch {
			setIsLoggedIn(false);
			setUserId(null);
			setUsername(null);
		}
	};

	useEffect(() => {
		updateUserData();
	}, []);

	return (
		<AuthContext.Provider
			value={{
				isLoggedIn,
				setIsLoggedIn,
				updateUserData,
				userId,
				username,
			}}
		>
			{props.children}
		</AuthContext.Provider>
	);
};

export default AuthContext;
export { AuthContextProvider };
