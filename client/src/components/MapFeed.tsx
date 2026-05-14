import { MapContainer, Marker, TileLayer } from "react-leaflet";
import type { Incident } from "../types/incident";

interface MapFeedProps {
    incidents: Incident[],
    onMarkerClick?: (incident: Incident) => void
}

interface LocationMarkerProps {
    incident: Incident,
    onMarkerClick: (incident: Incident) => void
}

function LocationMarker({ incident, onMarkerClick } : LocationMarkerProps) {
    const handleClickEvent = () => {
        onMarkerClick(incident)
    }

    return (
        <Marker position={{ lat: incident.latitude, lon: incident.longitude }}
            eventHandlers={{ click: handleClickEvent }}
        ></Marker>
    )
}

export default function MapFeed({ incidents, onMarkerClick }: MapFeedProps) {
    const defaultCenter = [7.0700, 125.6000] as [number, number];

    const mapOptions = {
        center: defaultCenter,
        zoom: 13,
        maxZoom: 18,
        minZoom: 5,
    };
    
    return (
        <div className="h-full overflow-hidden relative">
            <MapContainer
                {...mapOptions}
                style={{ height: '100%', width: '100%', zIndex: 0 }}
            >
                <TileLayer
                    url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
                />
                {incidents.map(x => (
                    <LocationMarker key={x.id} onMarkerClick={() => onMarkerClick && onMarkerClick(x)} incident={x}/>
                ))}
            </MapContainer>
        </div>
    );
}