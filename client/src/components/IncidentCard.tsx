import { useNavigate } from "react-router-dom";
import type { Incident } from "../types/incident";
import useAuth from "../hooks/useAuth";
import { useEffect, useState } from "react";

interface IncidentCardProps {
  incident: Incident;
  hideCommentButton?: boolean;
  onStatusChange: (id: string, newStatus: string) => void;
}
export default function IncidentCard({ incident, hideCommentButton = false, onStatusChange }: IncidentCardProps) {
  const navigate = useNavigate();
  const { auth } = useAuth()
  const [status, setStatus] = useState(incident.status)

  const getStatusColor = (status: string | number) => {
    if (status === "Resolved" || status === 1) return "bg-green-100 text-green-800";
    if (status === "Denied" || status === 2) return "bg-red-100 text-red-800";
    return "bg-yellow-100 text-yellow-800";
  };

  useEffect(() => {
    if (status != incident.status) onStatusChange(incident.id, status);
  }, [status])

  const handleCommentClick = () => {
    // Navigate to the details page and pass the incident object in memory
    navigate(`/incidents/${incident.id}`, { state: { incident } });
  };


  return (
    <div className="mb-6 overflow-hidden rounded-xl border border-gray-200 bg-white shadow-sm max-w-2xl">
      {/* Card Header: User Info & Status */}
      <div className="flex items-center justify-between p-4">
        <div className="flex items-center gap-3">
          <img
            src={`https://ui-avatars.com/api/?name=${incident.username}&background=0D8ABC&color=fff`}
            alt={incident.username}
            className="h-10 w-10 rounded-full"
          />
          <div>
            <p className="text-sm font-bold text-gray-900">
              {incident.username}
            </p>
            <p className="text-xs text-gray-500">
              {new Date(incident.createdAt).toLocaleDateString()}
            </p>
          </div>
        </div>
        <span>
          <button />
        </span>
      </div>

      {/* Card Content: Text */}
      <div className="px-4 pb-3">
        <div className="flex justify-between items-center">
          <div className="mb-2 flex items-center gap-2">
            <span className="rounded bg-blue-100 px-2 py-0.5 text-xs font-medium text-blue-800">
              {incident.category}
            </span>
            <h3 className="text-lg font-bold text-gray-900">{incident.title}</h3>
          </div>
          {(auth.role == "Admin" || auth.role == "SuperAdmin") && (
            <select
              id="category"
              value={status}
              onChange={(e) => setStatus(e.target.value)}
              required
              className={`rounded-full px-3 py-1 text-xs font-semibold ${getStatusColor(status)}`}>
              <option value="Reported">Reported</option>
              <option value="Resolved">Resolved</option>
              <option value="Denied">Denied</option>
            </select>
          )}
          {auth.role == "Resident" && (
            <span className={`rounded-full px-3 py-1 text-xs font-semibold ${getStatusColor(incident.status)}`}>
              {incident.status}
            </span>)
          }
        </div>
        <p className="text-sm text-gray-700">{incident.description}</p>

        {/* Optional: Show coordinates if they exist */}
        {incident.latitude && incident.longitude && (
          <p className="mt-2 text-xs text-gray-400">
            📍 Location: {incident.latitude}, {incident.longitude}
          </p>
        )}
      </div>

      {/* Card Media: Conditional Image */}
      {incident.imageUrl && (
        <div className="max-h-96 w-full overflow-hidden bg-gray-100">
          <img
            src={`http://localhost:9000/incidents/${incident.imageUrl}`}
            alt="Incident media"
            className="h-full w-full object-cover"
          />
        </div>
      )}

      {!hideCommentButton && (
        <div className="border-t border-gray-100 px-4 py-3 justify-end flex">
          <button
            onClick={handleCommentClick}
            className="text-sm font-medium text-gray-500 hover:text-blue-600 transition-colors"
          >
            💬 Comments
          </button>
        </div>
      )}
    </div>

  );
}
