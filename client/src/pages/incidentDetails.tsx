import { useEffect, useState } from "react";
import { useLocation, useNavigate, useParams } from "react-router-dom";
import useBackendApi from "../hooks/useBackendApi";
import type { Incident } from "../types/incident";
import type { Comment as CommentType } from "../types/comment";
import IncidentCard from "../components/IncidentCard";
import Comment from "../components/Comment";
import { useUpdateIncidentStatus } from "../hooks/useUpdateIncidentStatus";
import MapFeed from "../components/MapFeed";

export default function IncidentDetails() {
  const api = useBackendApi();
  const location = useLocation();
  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>(); 
  
  const [incident, setIncident] = useState<Incident | null>(location.state?.incident || null);
  const [comments, setComments] = useState<CommentType[]>([]);
  const [errMsg, setErrMsg] = useState("");
  const [isLoading, setIsLoading] = useState(true);

  // NEW: State to handle mobile view toggle, defaulting to 'details'
  const [mobileView, setMobileView] = useState<'details' | 'map'>('details');

  const [newComment, setNewComment] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [postError, setPostError] = useState("");

  const { updateStatus } = useUpdateIncidentStatus();

  useEffect(() => {
    const fetchIncidentData = async () => {
      setIsLoading(true);
      try {
        if (!incident && id) {
          const incResponse = await api.get(`/incidents/${id}`);
          setIncident(incResponse.data);
        }
        const comResponse = await api.get(`/incidents/${id}/comments`);
        setComments(comResponse?.data?.items || comResponse?.data || []);
      } catch (error: any) {
        console.error(error);
        setErrMsg("Failed to load incident details or comments.");
      } finally {
        setIsLoading(false);
      }
    };

    if (id) {
      fetchIncidentData();
    } else {
      navigate('/incidents/feed');
    }
  }, [id, api, navigate]); 

  const handleStatusChange = async (incidentId: string, newStatus: string) => {
    const success = await updateStatus(incidentId, newStatus);
    if (success && incident) {
      setIncident({ ...incident, status: newStatus });
    }
  };

  const handlePostComment = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!newComment.trim() || !incident) return;

    setIsSubmitting(true);
    setPostError("");

    try {
      await api.post(`/incidents/${incident.id}/comments`, { content: newComment });
      setNewComment(""); 
      const response = await api.get(`/incidents/${incident.id}/comments`);
      setComments(response?.data?.items || response?.data); 
    } catch (error: any) {
      console.error(error);
      setPostError("Failed to post comment. Please try again.");
    } finally {
      setIsSubmitting(false);
    }
  };

  if (!incident && isLoading) {
    return <div className="flex h-screen items-center justify-center animate-pulse text-gray-500">Loading details...</div>;
  }

  if (!incident) return null;

  return (
    <div className="flex flex-col md:flex-row bg-gray-50 overflow-hidden">
        
      <div className="md:hidden fixed bottom-6 right-5 z-1000">
        <button 
          onClick={() => setMobileView(mobileView === 'details' ? 'map' : 'details')}
          className="w-full max-w-xs rounded bg-blue-100 px-4 py-2 font-bold text-blue-800 transition-colors hover:bg-blue-200"
        >
          View {mobileView === 'details' ? 'Map' : 'Details'}
        </button>
      </div>

        {/* Left Column: Details & Comments (Toggles on Mobile) */}
        <section 
            className={`w-full md:w-1/2 lg:w-2/5 flex-col overflow-y-auto px-4 md:px-8 py-6 h-full border-r border-gray-200 
            ${mobileView === 'map' ? 'hidden md:flex' : 'flex'}`}
        >
            <button 
                onClick={() => navigate('/incidents/feed')} 
                className="mb-4 text-sm font-medium text-gray-500 hover:text-gray-800 flex items-center gap-2"
            >
                ← Back to Feed
            </button>

            <IncidentCard 
              incident={incident} 
              hideCommentButton={true} 
              onStatusChange={handleStatusChange} 
            />

            <div className="rounded-xl border border-gray-200 bg-white shadow-sm mt-4 mb-8">
                <div className="bg-gray-50 border-b border-gray-200 p-4">
                    <h3 className="font-bold text-gray-900">Comments ({comments.length})</h3>
                </div>

                {isLoading && <div className="p-8 text-center text-gray-500 animate-pulse">Loading comments...</div>}
                {errMsg && <div className="p-4 text-red-600 bg-red-50">{errMsg}</div>}
                {!isLoading && comments.length === 0 && !errMsg && (
                    <div className="p-8 text-center text-gray-500 text-sm">No comments yet. Be the first to share!</div>
                )}

                <div className="flex flex-col max-h-96 overflow-y-auto">
                    {comments.map((comment) => (
                        <Comment key={comment.id} comment={comment} />
                    ))}
                </div>

                <div className="p-4 border-t border-gray-100 bg-white">
                    <form onSubmit={handlePostComment}>
                        <textarea
                            value={newComment}
                            onChange={(e) => setNewComment(e.target.value)}
                            placeholder="Write a comment..."
                            rows={3}
                            disabled={isSubmitting}
                            className="w-full rounded-lg border border-gray-300 p-3 text-sm text-gray-900 focus:border-blue-500 focus:outline-none focus:ring-2 focus:ring-blue-500/20 resize-none disabled:bg-gray-50"
                        />
                        <div className="mt-2 flex items-center justify-between">
                            <span className="text-xs text-red-500 font-medium">{postError}</span>
                            <button
                                type="submit"
                                disabled={!newComment.trim() || isSubmitting}
                                className="rounded-lg bg-blue-600 px-5 py-2 text-sm font-semibold text-white shadow-sm hover:bg-blue-700 disabled:bg-blue-300 transition-all"
                            >
                                {isSubmitting ? "Posting..." : "Post Comment"}
                            </button>
                        </div>
                    </form>
                </div>
            </div>
        </section>

        {/* Right Column: Sticky Map (Toggles on Mobile) */}
        <section 
            // Calculated height accounts for the ~76px mobile toggle bar so the map doesn't overflow
            className={`w-full md:w-1/2 lg:w-3/5 max-h-[calc(100vh-76px)] h-screen bg-gray-200 
            ${mobileView === 'details' ? 'hidden md:block' : 'block'}`}
        >
             <MapFeed incidents={[incident]}/>
        </section>
    </div>
  );
}