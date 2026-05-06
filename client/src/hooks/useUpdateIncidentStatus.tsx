import useBackendApi from "./useBackendApi";

export function useUpdateIncidentStatus() {
    const api = useBackendApi();

    const updateStatus = async (incidentId: string, newStatus: string) => {
        try {
            await api.patch(`/incidents/${incidentId}/status`, { status: newStatus });
            return true; 
        } catch (error) {
            console.error("Failed to update status", error);
            return false; 
        }
    }

    return { updateStatus }
}