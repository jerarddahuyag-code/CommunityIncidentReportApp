import { useEffect, useState } from "react";
import type { Incident } from "../types/incident";
import useBackendApi from "../hooks/useBackendApi";
import IncidentCard from "../components/IncidentCard";
import { Link, useNavigate } from "react-router-dom";
import { useUpdateIncidentStatus } from "../hooks/useUpdateIncidentStatus";
import MapFeed from "../components/MapFeed";

const GET_INCIDENTS_URL = "/incidents";

export default function IncidentFeed() {
  const [incidents, setIncidents] = useState([] as Incident[]);
  const [errMsg, setErrMsg] = useState("");
  const [isLoading, setIsLoading] = useState(true);

  const [mobileView, setMobileView] = useState<'feed' | 'map'>('feed');
  const { updateStatus } = useUpdateIncidentStatus();
  const navigate = useNavigate();
  const api = useBackendApi();

  useEffect(() => {
    const fetchIncidents = async () => {
      setIsLoading(true);
      try {
        const response = await api.get(GET_INCIDENTS_URL);
        setIncidents(response?.data?.items);
      } catch (error: any) {
        console.log(error);
        setErrMsg("There was an error fetching the incidents");
      } finally {
        setIsLoading(false);
      }
    };

    fetchIncidents();
  }, [api]);

  const handleStatusChange = async (incidentId: string, newStatus: string) => {
    const success = await updateStatus(incidentId, newStatus);
    if (success) {
      setIncidents(prev => prev.map(inc =>
        inc.id === incidentId ? { ...inc, status: newStatus } : inc
      ))
    }
  }

  const handleIncidentClick = (incident: Incident) => {
    // Navigate to the details page and pass the incident object in memory
    navigate(`/incidents/${incident.id}`, { state: { incident } });
  };

  return (
   <div className="flex flex-col md:flex-row h-full bg-gray-50 overflow-hidden max-h-[calc(100vh-76px)] ">

      <div 
        className={`w-full md:w-1/2 lg:w-1/3 flex-col overflow-y-auto px-5 py-6
        ${mobileView === 'map' ? 'hidden md:flex' : 'flex'}`}
      >
        <div className="mb-6 flex items-center justify-between">
          <h1 className="text-2xl font-extrabold text-gray-900">Community Watch</h1>
          <Link
            to="/incidents/create"
            className="rounded-lg bg-blue-600 px-4 py-2 text-sm font-bold text-white shadow hover:bg-blue-700"
          >
            Report
          </Link>
        </div>

        {isLoading && (
          <div className="py-10 text-center text-gray-500">
            <p className="animate-pulse font-medium">Loading feed...</p>
          </div>
        )}

        {errMsg && (
          <div className="rounded-lg bg-red-50 p-4 text-center text-red-700 border border-red-200">
            {errMsg}
          </div>
        )}

        {!isLoading && !errMsg && incidents.length === 0 && (
          <div className="rounded-lg bg-white p-10 text-center shadow-sm border border-gray-200">
            <h3 className="text-lg font-bold text-gray-900">All clear!</h3>
            <p className="mt-2 text-sm text-gray-500">
              No incidents have been reported in the community yet.
            </p>
          </div>
        )}

        <div className="flex flex-col pb-20 md:pb-0">
          {incidents.map((incident) => (
            <IncidentCard 
              key={incident.id} 
              incident={incident} 
              onStatusChange={handleStatusChange} 
              handleIncidentClick={handleIncidentClick}
            />
          ))}
        </div>
      </div>

      <div 
        className={`w-full md:w-1/2 lg:w-2/3 h-screen bg-gray-200 
        ${mobileView === 'feed' ? 'hidden md:block' : 'block'}`}
      >
        <MapFeed incidents={incidents} onMarkerClick={handleIncidentClick}/>
      </div>
      
      <div className="md:hidden fixed bottom-6 right-5 z-1000">
        <button 
          onClick={() => setMobileView(mobileView === 'feed' ? 'map' : 'feed')}
          className="w-full max-w-xs rounded bg-blue-100 px-4 py-2 font-bold text-blue-800 transition-colors hover:bg-blue-200"
        >
          View {mobileView === 'feed' ? 'Map' : 'Feed'}
        </button>
      </div>
    </div>
  );
}
