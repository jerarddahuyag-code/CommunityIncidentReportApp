import type { Comment as CommentType } from "../types/comment"; // Aliased to prevent naming collision with the component

interface CommentProps {
    comment: CommentType;
}

export default function Comment({ comment }: CommentProps) {
    return (
        <div className="flex gap-4 border-b border-gray-100 p-4 last:border-0 hover:bg-gray-50 transition-colors">
            {/* Avatar */}
            <div className="shrink-0">
                <img
                    src={`https://ui-avatars.com/api/?name=${comment.username}&background=random&color=fff`}
                    alt={comment.username}
                    className="h-8 w-8 rounded-full"
                />
            </div>
            
            {/* Comment Content */}
            <div className="flex-1">
                <div className="flex items-center gap-2">
                    <span className="text-sm font-bold text-gray-900">{comment.username}</span>
                    <span className="text-xs text-gray-400">
                        {new Date(comment.createdAt).toLocaleDateString()} at {new Date(comment.createdAt).toLocaleTimeString([], {hour: '2-digit', minute:'2-digit'})}
                    </span>
                </div>
                <p className="mt-1 text-sm text-gray-700 whitespace-pre-wrap">{comment.content}</p>
            </div>
        </div>
    );
}