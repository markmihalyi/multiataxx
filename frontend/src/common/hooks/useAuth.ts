import AuthContext, { IAuthContext } from "../providers/AuthProvider";

import { useContext } from "react";

const useAuth = () => {
	return useContext<IAuthContext>(AuthContext);
};

export default useAuth;
