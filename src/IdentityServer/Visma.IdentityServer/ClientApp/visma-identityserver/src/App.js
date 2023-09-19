import logo from './logo.svg';
import Login from './components/login';
import { BrowserRouter, Route, Switch } from 'react-router-dom';
import './App.css';

function App() {
  const baseUrl = window.location.pathname.replace(/\/$/, '');
  document.title = 'Visma Identity';

  return (
    <BrowserRouter basename={baseUrl}>
    <Switch>
        <Route exact path="/" component={Login} />
    </Switch>
</BrowserRouter>
  );
}

export default App;
