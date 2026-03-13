import { Routes, Route } from "react-router-dom";
import Login from "./pages/login";
import Register from "./pages/register";
import IncidentFeed from "./pages/incidentFeed";
import CreateIncident from "./pages/createIncident";
import 'leaflet/dist/leaflet.css';
import IncidentDetails from "./pages/incidentDetails";

function App() {
  return (
    <>
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />
        <Route path="/incidents/feed" element={<IncidentFeed />} />
        <Route path="/incidents/create" element={<CreateIncident />} />
        <Route path="/incidents/:id" element={<IncidentDetails />} />
      </Routes>
    </>
  );
}
export default App;
