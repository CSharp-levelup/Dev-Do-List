import "./Login.css";
import { useState } from "react";
// import axios from "axios";
import logo from "../assets/logo.svg";
import { useNavigate } from "react-router-dom";

/* Server information */
// const hostname = "localhost";
// const port = 8080;

const Login = () => {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [rememberMe, setRememberMe] = useState(false); //for remember me functionality
  const [errorMessage, setErrorMessage] = useState("");
  const navigate = useNavigate();

  const handleRegister = async () => {
    navigate(`/Register`);
  };

  /*
   * Oauth Login needs to be implemented
   */
  const handleLogin = async () => {
    setErrorMessage("");

    /* TODO - TEMP */
    navigate(`/Dashboard`);

    /* TODO - Implement Oauth login */
    // try {
    //   const response = await axios.post(
    //     `http://${hostname}:${port}/user/login`,
    //     {
    //       username: username,
    //       password: password,
    //       rememberMe: rememberMe,
    //     }
    //   );

    //   // console.log("Login request success (web)", response.data);

    //   if (response.data.success == true) {
    //     /* Store user ID */
    //     const userID = response.data.userID;
    //     localStorage.setItem("userID", userID);

    //     /* Store jwt token */
    //     // const JWTTOKEN = response.data.token;
    //     // localStorage.setItem("jwtToken", JWTTOKEN);
    //     /* Store avatar preference */
    //     localStorage.setItem("avatar", response.data.avatar);

    //     navigate(`/Dashboard`);
    //   } else {
    //     console.error("Incorrect username or password");
    //     setErrorMessage(response.data.message);
    //   }
    // } catch (error) {
    //   if (error.response) {
    //     setErrorMessage(error.response.data.message);
    //   }
    //   setErrorMessage("Login failed.");
    //   console.error("Login failed: ", error.response.data.message);
    // }
  };

  return (
    <div>
      <div className="login-page">
        <div className="logo-container-login">
          <img src={logo} />
        </div>
        <div className="login-container">
          <div className="input-container-login">
            <div className="input-container-title-login">
              Login to your account
            </div>
            <div className="input-container-username">
              {" "}
              Username
              <input
                type="text"
                value={username}
                onChange={(e) => setUsername(e.target.value)}
              />
            </div>
            <div className="input-container-password">
              {" "}
              Password
              <input
                type="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
              />
            </div>
            <div className="remember-me-container">
              <input
                type="checkbox"
                checked={rememberMe}
                onChange={() => setRememberMe(!rememberMe)}
              />
              <label>Remember Me</label>
            </div>
            <div className="login-button-container">
              <button className="login-button" onClick={handleLogin}>
                Login
              </button>
              <div className="register-link" onClick={handleRegister}>
                New Here? Register Now.
              </div>
              {errorMessage && (
                <div className="error-message">{errorMessage}</div>
              )}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Login;
