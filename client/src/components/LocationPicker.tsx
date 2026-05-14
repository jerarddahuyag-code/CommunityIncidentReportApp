import { useState } from 'react';
import { TileLayer, Marker, useMapEvents, MapContainer } from 'react-leaflet';

interface LocationPickerProps {
  onLocationSelect: (lat: number, lng: number) => void;
}

// Helper component to handle clicks
function LocationMarker({ onLocationSelect }: LocationPickerProps) {
  const [position, setPosition] = useState<{lat: number, lng: number} | null>({lat: 7.0700, lng: 125.6000});

  useMapEvents({
    click(e : any) {
      setPosition(e.latlng);
      onLocationSelect(e.latlng.lat, e.latlng.lng);
    },
  });

  return position === null ? null : (
    <Marker position={position}></Marker>
  );
}

export default function LocationPicker({ onLocationSelect }: LocationPickerProps) {
  const defaultCenter = [7.0700, 125.6000] as [number, number];
    
  const mapOptions = {
    center: defaultCenter,
    zoom: 13,
    maxZoom: 18,
    minZoom: 5,
  };
  return (
    <div className="w-full overflow-hidden rounded-lg border border-gray-300 shadow-sm z-0 relative">
      <MapContainer 
        {...mapOptions}
        style={{ height: '300px', width: '100%', zIndex: 0 }}
      >
        <TileLayer
          url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
        />
        <LocationMarker onLocationSelect={onLocationSelect} />
      </MapContainer>
    </div>
  );
}