import AuthContext, { IAuthContext } from "../providers/AuthProvider";

import { useContext } from "react";

const useAuth = () => {
	const { isLoggedIn, setIsLoggedIn } = useContext<IAuthContext>(AuthContext);
	return { isLoggedIn, setIsLoggedIn };
};

export default useAuth;
