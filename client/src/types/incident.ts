export interface Incident{
    id: string,
    userId: string,
    username: string,
    title: string,
    description: string,
    category: string,
    latitude?: number | null,
    longitude?: number | null,
    status: string,
    imageUrl?: string | null,
    createdAt: string
}