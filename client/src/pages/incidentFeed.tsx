import { useEffect, useState } from "react";
import type { Incident } from "../types/incident";
import useBackendApi from "../hooks/useBackendApi";
import IncidentCard from "../components/IncidentCard";
import { Link } from "react-router-dom";
import { useUpdateIncidentStatus } from "../hooks/useUpdateIncidentStatus";

const GET_INCIDENTS_URL = "/incidents";

export default function IncidentFeed() {
  const [incidents, setIncidents] = useState([] as Incident[]);
  const [errMsg, setErrMsg] = useState("");
  const [isLoading, setIsLoading] = useState(true);
  const { updateStatus } = useUpdateIncidentStatus();
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

  return (
    <div className="flex min-h-screen justify-center bg-gray-50">
      <div className="mx-auto px-5">
        {/* Feed Header */}
        <div className="mb-6 flex items-center justify-between">
          <h1 className="text-2xl font-extrabold text-gray-900">
            Community Watch
          </h1>
          <Link
            to="/incidents/create"
            className="rounded-lg bg-blue-600 px-4 py-2 text-sm font-bold text-white shadow hover:bg-blue-700"
          >
            + Report Incident
          </Link>
        </div>

        {/* State Handling: Loading */}
        {isLoading && (
          <div className="py-10 text-center text-gray-500">
            <p className="animate-pulse font-medium">Loading feed...</p>
          </div>
        )}

        {/* State Handling: Error */}
        {errMsg && (
          <div className="rounded-lg bg-red-50 p-4 text-center text-red-700 border border-red-200">
            {errMsg}
          </div>
        )}

        {/* Empty State */}
        {!isLoading && !errMsg && incidents.length === 0 && (
          <div className="rounded-lg bg-white p-10 text-center shadow-sm border border-gray-200">
            <h3 className="text-lg font-bold text-gray-900">All clear!</h3>
            <p className="mt-2 text-sm text-gray-500">
              No incidents have been reported in the community yet.
            </p>
          </div>
        )}

        {/* The Feed */}
        <div className="flex flex-col">
          {incidents.map((incident) => (
            <IncidentCard key={incident.id} incident={incident} onStatusChange={handleStatusChange}/>
          ))}
        </div>
      </div>
    </div>
  );
}
