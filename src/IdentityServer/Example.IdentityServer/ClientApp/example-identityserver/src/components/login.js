
import { useState } from "react";
import { Declares } from "../constants/Declares";
import axios from 'axios';
import './login.scss';

function Login() {
    const [credentials, setCredentials] = useState({ email: '', password: '' });
    const [errorMessage, setErrorMessage] = useState('');

    const handleChangeEmail = (e) => {
        setCredentials((prevCredentials) => ({
            ...prevCredentials,
            email: e.target.value,
        }));
    };

    const handleChangePassword = (e) => {
        setCredentials((prevCredentials) => ({
            ...prevCredentials,
            password: e.target.value,
        }));
    };

    const getErrorMessage = (errorData) => {
        setErrorMessage(errorData);
        console.log(Declares.ERR_WCF_UNAVAILABLE + ' = ' + errorData.error_code);
        switch (errorData.error_code) {
            case Declares.ERR_STATUP_NOT_READY:
                return this.state.language.serverNotReady;
            case Declares.ERR_WRONG_USERNAME_OR_PASSWORD:
                return this.state.language.credentialError;
            case Declares.ERR_TOO_MANY_USERS:
                return this.state.language.tooManyOperators
            case Declares.ERR_WCF_UNAVAILABLE:
                return this.state.language.serviceUnavailable;
            case Declares.ERR_INTERNAL_SERVER_ERROR:
                return this.state.language.internalServerError;
            default:
                return errorData.error + ': ' + errorData.error_description;
        }
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        const queryParam = new URLSearchParams(window.location.search);
        let returnUrl = queryParam.get('ReturnUrl');
        try {
            setErrorMessage('');
            // call the API
            const email = credentials.email;
            const password = credentials.password;

            const response = await axios.post('https://Example-economic.com:7032/api/authenticate', {
                email,
                password,
                returnUrl
            }, {
                headers: {
                    'Content-Type': 'application/json'
                },
                withCredentials: true
            });

            const data = response.data;

            if (data) {
                if (data.status === Declares.LOGIN_STATUS_SUCCESS) {
                    window.location = data.redirectUrl;
                } else {
                    console.log(getErrorMessage(data));
                }
            } else {
                setErrorMessage('No data from API call');
            }
        } catch (e) {
            setErrorMessage(e.code);
            console.log("authenticate error: ", e);
        }
    };

    const loginWithGoogle = async () => {
        try {
            console.log(`Login with google`);
            const queryParam = new URLSearchParams(window.location.search);
            let returnUrl = queryParam.get('ReturnUrl');

            const response = await fetch(
                `https://Example-economic.com:7032/api/authenticate/ExternalLogin?returnUrl=${returnUrl}&provider=Google`
            );

            const data = response.data;

            if (data) {
                if (data.status === Declares.LOGIN_STATUS_SUCCESS) {
                    console.log(`Window location`, data.redirectUrl);
                    window.location = data.redirectUrl;
                } else {
                    console.log(getErrorMessage(data));
                }
            } else {
                setErrorMessage('No data from API call');
                console.log('No data');
            }
        } catch (e) {
            setErrorMessage('Authentication error');
            console.log('authenticate error: ', e);
        }
    };

    const disallowSubmit = (credentials.email === "" || credentials.password === "");
    const inputValidationState = errorMessage && 'error';

    return (
        <div>
            <div className="sc-Example-supergraphic">
                <div className="login-container">
                    <div>
                    </div>
                    <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column' }}>
                        <label htmlFor="username-text-input">Email</label>
                        <input
                            className="input-text-field"
                            id="username-text-input"
                            value={credentials.email}
                            onChange={handleChangeEmail}
                            validation={inputValidationState}
                        />
                        <label htmlFor="password-text-input">Password</label>
                        <input
                            type="password"
                            className="input-text-field"
                            id="password-text-input"
                            label="Password"
                            value={credentials.password}
                            onChange={handleChangePassword}
                            validation={inputValidationState}
                            style={{ marginTop: '2rem' }}
                        />
                        <button
                            id="login-button"
                            className={disallowSubmit ? 'button-disabled' : 'button'}
                            type="submit"
                            disabled={disallowSubmit}
                            style={{ margin: '4rem auto 0 0' }}
                        >
                            Login
                        </button>
                    </form>
                    <div class="google-btn">
                        <div class="google-icon-wrapper">
                            <img alt="test" class="google-icon" src="https://upload.wikimedia.org/wikipedia/commons/5/53/Google_%22G%22_Logo.svg" />
                        </div>
                        <p class="btn-text" onClick={loginWithGoogle}><b>Sign in with google</b></p>
                    </div>
                    {errorMessage && errorMessage !== '' && (
                        <div class="error-message">
                            <h3>{errorMessage}</h3>
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
}

export default Login;