import AuthContext, { IAuthContext } from "../providers/AuthProvider";

import { useContext } from "react";

const useAuth = () => {
	const {
		isLoggedIn,
		setIsLoggedIn,
		permanentUsername,
		setPermanentUsername,
	} = useContext<IAuthContext>(AuthContext);
	return {
		isLoggedIn,
		setIsLoggedIn,
		permanentUsername,
		setPermanentUsername,
	};
};

export default useAuth;
