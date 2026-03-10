import { useEffect, useState, type SubmitEventHandler } from "react";
import useBackendApi from "../hooks/useBackendApi";
import { useNavigate } from "react-router-dom";
import LocationPicker from "../components/LocationPicker";

const CREATE_INCIDENT_URL = "/incidents"

export default function CreateIncident() {
    const api = useBackendApi();
    const navigate = useNavigate();

    const [title, setTitle] = useState('');
    const [description, setDescription] = useState('');
    const [category, setCategory] = useState('');
    const [location, setLocation] = useState<{ lat: number, lon: number } | null>({lat: 7.0700, lon: 125.6000});
    const [image, setImage] = useState<File | null>(null);
    const [errMsg, setErrMsg] = useState('');

    useEffect(() => {
        setErrMsg('');
    }, [title, description, category, location, image]);

    const handleSubmit : SubmitEventHandler = async (e) => {
        e.preventDefault();
        if (!location) {
            setErrMsg("Please select a location");
            return;
        }

        try{
            const formData = new FormData();
            formData.append("title", title);
            formData.append("description", description);
            formData.append("category", category);
            formData.append("latitude", String(location.lat));
            formData.append("longitude", String(location.lon));
            if(image){
                formData.append("file", image);
            }

            await api.post(CREATE_INCIDENT_URL, formData, {
                headers: {"Content-Type": "multipart/form-data"}
            });

            setTitle('');
            setDescription('');
            setCategory('');
            setLocation({lat: 7.0700, lon: 125.6000});
            setImage(null);
            navigate('/incidents/feed');
        } catch (error : any){
            console.log(error);
            setErrMsg("Failed to Create Incident");
        }
    }
    
    return (
        <section className="mx-auto max-w-2xl p-4 min-h-screen">
            <h1 className="mb-6 text-2xl font-bold text-gray-900">Report an Incident</h1>
            
            {errMsg && (
                <div className="mb-4 rounded-lg bg-red-50 p-3 text-red-600 border border-red-200" aria-live="assertive">
                    {errMsg}
                </div>
            )}

            <form onSubmit={handleSubmit} className="space-y-5 rounded-2xl bg-white p-6 shadow-xl border border-gray-100">
                
                {/* Title Input */}
                <div>
                    <label htmlFor="title" className="mb-1 block text-sm font-medium text-gray-700">Title</label>
                    <input
                        type="text"
                        id="title"
                        value={title}
                        onChange={(e) => setTitle(e.target.value)}
                        required
                        placeholder="E.g., Broken Streetlight"
                        className="w-full rounded-lg border border-gray-300 px-4 py-2 text-gray-900 focus:border-blue-500 focus:outline-none focus:ring-2 focus:ring-blue-500/20"
                    />
                </div>

                {/* Category Dropdown */}
                <div>
                    <label htmlFor="category" className="mb-1 block text-sm font-medium text-gray-700">Category</label>
                    <select
                        id="category"
                        value={category}
                        onChange={(e) => setCategory(e.target.value)}
                        required
                        className="w-full rounded-lg border border-gray-300 px-4 py-2 text-gray-900 focus:border-blue-500 focus:outline-none focus:ring-2 focus:ring-blue-500/20 bg-white"
                    >
                        <option value="" disabled>Select a category</option>
                        <option value="Security">Security</option>
                        <option value="Maintenance">Maintenance</option>
                        <option value="Wildlife">Wildlife</option>
                        <option value="Suggestion">Suggestion</option>
                        <option value="Other">Other</option>
                    </select>
                </div>

                {/* Description Textarea */}
                <div>
                    <label htmlFor="description" className="mb-1 block text-sm font-medium text-gray-700">Description</label>
                    <textarea
                        id="description"
                        value={description}
                        onChange={(e) => setDescription(e.target.value)}
                        required
                        rows={4}
                        placeholder="Provide details about the incident..."
                        className="w-full rounded-lg border border-gray-300 px-4 py-2 text-gray-900 focus:border-blue-500 focus:outline-none focus:ring-2 focus:ring-blue-500/20 resize-y"
                    />
                </div>

                {/* The Map */}
                <div>
                    <label className="mb-1 block text-sm font-medium text-gray-700">Incident Location</label>
                    <p className="mb-3 text-xs text-gray-500">Click on the map to drop a pin at the incident location.</p>
                    <LocationPicker 
                        onLocationSelect={(lat, lon) => setLocation({ lat, lon })} 
                    />
                </div>

                {/* File Upload */}
                <div>
                    <label htmlFor="file-upload" className="mb-1 block text-sm font-medium text-gray-700">Photo Evidence (Optional)</label>
                    <input 
                        id="file-upload"
                        type="file" 
                        accept="image/*"
                        onChange={(e) => setImage(e.target.files ? e.target.files[0] : null)}
                        className="w-full rounded-lg border border-gray-300 px-4 py-2 text-sm text-gray-700 file:mr-4 file:rounded-full file:border-0 file:bg-blue-50 file:px-4 file:py-2 file:text-sm file:font-semibold file:text-blue-700 hover:file:bg-blue-100"
                    />
                </div>

                {/* Submit Button */}
                <button 
                    type="submit" 
                    className="mt-6 w-full rounded-lg bg-blue-600 px-4 py-2.5 text-sm font-semibold text-white shadow-sm hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 transition-all active:scale-[0.98]"
                >
                    Submit Report
                </button>
            </form>
        </section>
    );
}
