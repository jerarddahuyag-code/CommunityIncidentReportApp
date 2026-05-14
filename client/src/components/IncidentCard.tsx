import type { Incident } from "../types/incident";
import useAuth from "../hooks/useAuth";

interface IncidentCardProps {
  incident: Incident,
  hideCommentButton?: boolean,
  onStatusChange: (id: string, newStatus: string) => void,
  handleIncidentClick?: (incident: Incident) => void
}

export default function IncidentCard({ incident, hideCommentButton = false, onStatusChange, handleIncidentClick }: IncidentCardProps) {
  const { auth } = useAuth();
  
  // Feature: Pull storage URL from environment, fallback to localhost for dev
  const storageUrl = import.meta.env.VITE_STORAGE_URL || 'http://localhost:9000/incidents/';

  const getStatusColor = (status: string | number) => {
    if (status === "Resolved" || status === 1) return "bg-green-100 text-green-800";
    if (status === "Denied" || status === 2) return "bg-red-100 text-red-800";
    return "bg-yellow-100 text-yellow-800";
  };

  // Fixed: Removed the useEffect trap. Handle change directly.
  const handleDropdownChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    const newStatus = e.target.value;
    // The parent component is responsible for calling the API and updating the incident object
    onStatusChange(incident.id, newStatus);
  };

  return (
    <div className="mb-6 rounded-xl border border-gray-200 bg-white shadow-sm w-full">
      {/* Card Header */}
      <div className="flex items-center justify-between p-4">
        <div className="flex items-center gap-3">
          <img
            src={`https://ui-avatars.com/api/?name=${incident.username}&background=0D8ABC&color=fff`}
            alt={incident.username}
            className="h-10 w-10 rounded-full"
          />
          <div>
            <p className="text-sm font-bold text-gray-900">{incident.username}</p>
            <p className="text-xs text-gray-500">{new Date(incident.createdAt).toLocaleDateString()}</p>
          </div>
        </div>
        {/* Removed empty <button /> */}
      </div>

      <div className="px-4 pb-3">
        <div className="flex justify-between items-center">
          <div className="mb-2 flex items-center gap-2">
            <span className="self-start mt-1 mb-1 rounded bg-blue-100 px-2 py-0.5 text-xs font-medium text-blue-800">
              {incident.category}
            </span>
            <h3 className="text-lg font-bold text-gray-900">{incident.title}</h3>
          </div>
          
          {(auth.role === "Admin" || auth.role === "SuperAdmin") && (
            <select
              value={incident.status} // Controlled directly by the prop
              onChange={handleDropdownChange}
              className={`self-start mt-1 rounded-full px-3 py-1 text-xs font-semibold focus:ring-2 focus:ring-blue-500 outline-none cursor-pointer ${getStatusColor(incident.status)}`}
            >
              <option value="Reported">Reported</option>
              <option value="Resolved">Resolved</option>
              <option value="Denied">Denied</option>
            </select>
          )}
          {auth.role === "Resident" && (
            <span className={`self-start mt-1 rounded-full px-3 py-1 text-xs font-semibold ${getStatusColor(incident.status)}`}>
              {incident.status}
            </span>
          )}
        </div>
        <p className="text-sm text-gray-700">{incident.description}</p>

        {incident.latitude && incident.longitude && (
          <p className="mt-2 text-xs text-gray-400">
            📍 Location: {incident.latitude.toFixed(4)}, {incident.longitude.toFixed(4)}
          </p>
        )}
      </div>

      {incident.imageUrl && (
        <div className="max-h-96 w-full overflow-hidden bg-gray-100">
          <img
            // Fixed: Using Environment Variable
            src={`${storageUrl}${incident.imageUrl}`}
            alt="Incident media"
            className="h-full w-full object-cover"
          />
        </div>
      )}

      {!hideCommentButton && handleIncidentClick && (
        <div className="border-t border-gray-100 px-4 py-3 justify-end flex">
          <button
            onClick={() => handleIncidentClick(incident)}
            className="text-sm font-medium text-gray-500 hover:text-blue-600 transition-colors"
          >
            💬 Comments
          </button>
        </div>
      )}
    </div>
  );
}