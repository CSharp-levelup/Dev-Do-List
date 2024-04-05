import { BrowserRouter, Routes, Route } from "react-router-dom";
import Login from "./pages/Login";
// import Register from "./Pages/Register";
// import Dashboard from "./Pages/Dashboard";
// import NewNote from "./Pages/NewNote";
// import ViewNote from "./Pages/ViewNote";
// import Profile from "./Pages/Profile";

function App() {
  return (
    <BrowserRouter>
      <main>
        <Routes>
          <Route path="/" element={<Login />} />
          {/* <Route path="/Dashboard" element={<Dashboard />} />
          <Route path="/Register" element={<Register />} />
          <Route path="/ViewNote" element={<ViewNote />} />
          <Route path="/NewNote" element={<NewNote />} />
          <Route path="/Profile" element={<Profile />} /> */}
        </Routes>
      </main>
    </BrowserRouter>
  );
}

export default App;
