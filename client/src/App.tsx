import { Routes, Route, Navigate } from "react-router-dom";
import Login from "./pages/login";
import Register from "./pages/register";
import IncidentFeed from "./pages/incidentFeed";
import CreateIncident from "./pages/createIncident";
import 'leaflet/dist/leaflet.css';
import IncidentDetails from "./pages/incidentDetails";
import RequireAuth from "./components/RequireAuth";
import PersistLogin from "./components/PersistLogin";
import Layout from "./components/Layout";

function App() {
  return (
    <>
      <Routes>
        <Route index element={<Navigate to="/incidents/feed" replace />} />
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />
        <Route element={<Layout/>}>
          <Route element={<PersistLogin />}>
              <Route element={<RequireAuth />}>
                  <Route path="incidents/feed" element={<IncidentFeed />} />
                  <Route path="incidents/create" element={<CreateIncident />} />
                  <Route path="incidents/:id" element={<IncidentDetails />} />
              </Route>
          </Route>
        </Route>
      </Routes>
    </>
  );
}
export default App;
