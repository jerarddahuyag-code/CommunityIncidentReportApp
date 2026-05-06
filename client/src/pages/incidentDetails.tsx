import { useEffect, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import useBackendApi from "../hooks/useBackendApi";
import type { Incident } from "../types/incident";
import type { Comment as CommentType } from "../types/comment";
import IncidentCard from "../components/IncidentCard";
import Comment from "../components/Comment";
import { useUpdateIncidentStatus } from "../hooks/useUpdateIncidentStatus";

export default function IncidentDetails() {
  const api = useBackendApi();
  const location = useLocation();
  const navigate = useNavigate();
  
  const [incident, setIncident] = useState(location.state?.incident as Incident);

  const [comments, setComments] = useState<CommentType[]>([]);
  const [errMsg, setErrMsg] = useState("");
  const [isLoading, setIsLoading] = useState(true);

  // New states for the comment form
  const [newComment, setNewComment] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [postError, setPostError] = useState("");

  const { updateStatus } = useUpdateIncidentStatus();

  useEffect(() => {
      if (!incident) {
          navigate('/incidents/feed');
      }
  }, [incident, navigate]);

  // I extracted the fetch logic into a reusable function so we can call it after posting
  const fetchComments = async () => {
    try {
      const response = await api.get(`/incidents/${incident.id}/comments`);
      setComments(response?.data?.items || response?.data); 
    } catch (error: any) {
      console.log(error);
      setErrMsg("There was an error fetching the comments.");
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    if (!incident) return;
    setIsLoading(true);
    fetchComments();
  }, [api, incident]);

  const handleStatusChange = async(incidentId:string, newStatus:string) => {
    const success = await updateStatus(incidentId, newStatus)
    if (success && incident) {
      setIncident({...incident, status: newStatus})
    }
  }

  // The function to handle submitting a new comment
  const handlePostComment = async (e: React.SubmitEvent) => {
    e.preventDefault();
    if (!newComment.trim()) return;

    setIsSubmitting(true);
    setPostError("");

    try {
      // Make sure 'content' matches your C# CreateCommentRequest record!
      await api.post(`/incidents/${incident.id}/comments`, {
        content: newComment 
      });

      setNewComment(""); // Clear the input field
      await fetchComments(); // Refresh the comment list to show the new one
    } catch (error: any) {
      console.error(error);
      setPostError("Failed to post comment. Please try again.");
    } finally {
      setIsSubmitting(false);
    }
  };

  if (!incident) return null;

  return (
    <section className="mx-auto max-w-2xl p-4 min-h-screen">
        <button 
            onClick={() => navigate(-1)} 
            className="mb-4 text-sm font-medium text-gray-500 hover:text-gray-800"
        >
            ← Back to Feed
        </button>

        <IncidentCard incident={incident} hideCommentButton={true} onStatusChange={handleStatusChange}/>

        <div className="rounded-xl border border-gray-200 bg-white shadow-sm mt-4 overflow-hidden">
            <div className="bg-gray-50 border-b border-gray-200 p-4">
                <h3 className="font-bold text-gray-900">Comments ({comments.length})</h3>
            </div>

            {/* Comment List States */}
            {isLoading && (
                <div className="p-8 text-center text-gray-500 animate-pulse">Loading comments...</div>
            )}

            {errMsg && (
                <div className="p-4 text-red-600 bg-red-50">{errMsg}</div>
            )}

            {!isLoading && comments.length === 0 && !errMsg && (
                <div className="p-8 text-center text-gray-500 text-sm">No comments yet. Be the first to share!</div>
            )}

            {/* The Comments */}
            <div className="flex flex-col">
                {comments.map((comment) => (
                    <Comment key={comment.id} comment={comment} />
                ))}
            </div>

            {/* NEW: The Comment Input Form */}
            <div className="p-4 border-b border-gray-100 bg-white">
                <form onSubmit={handlePostComment}>
                    <textarea
                        value={newComment}
                        onChange={(e) => setNewComment(e.target.value)}
                        placeholder="Write a comment..."
                        rows={3}
                        disabled={isSubmitting}
                        className="w-full rounded-lg border border-gray-300 p-3 text-sm text-gray-900 focus:border-blue-500 focus:outline-none focus:ring-2 focus:ring-blue-500/20 resize-none disabled:bg-gray-50 disabled:text-gray-500"
                    />
                    <div className="mt-2 flex items-center justify-between">
                        <span className="text-xs text-red-500 font-medium">{postError}</span>
                        <button
                            type="submit"
                            disabled={!newComment.trim() || isSubmitting}
                            className="rounded-lg bg-blue-600 px-5 py-2 text-sm font-semibold text-white shadow-sm hover:bg-blue-700 focus:outline-none disabled:bg-blue-300 disabled:cursor-not-allowed transition-all"
                        >
                            {isSubmitting ? "Posting..." : "Post Comment"}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    </section>
  );
}