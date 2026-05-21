# Community Watch

## What the project does
Community Watch is a responsive web application designed for subdivision residents to report and view local incidents in real-time. It provides a focused platform where verified neighbors can pin specific issues—such as security concerns, maintenance needs, or wildlife sightings—directly onto a local map and chronological feed.

## Why it is useful
Unlike generic neighborhood social media groups that can become cluttered, Community Watch is strictly focused on actionable incidents. It ensures high-fidelity local data through:
* **Privacy & Verification:** Access is strictly invite-only, ensuring all data comes from actual residents.
* **Granular Location Data:** Users can pin specific local issues on an interactive map.
* **Focused Utility:** Streamlined for reporting, tracking status, and commenting on specific community issues without unrelated social noise.

## Tech Stack
* **Frontend:** React, TypeScript, Tailwind CSS, Leaflet (React-Leaflet)
* **Backend:** .NET 9 Minimal API, C#
* **Database:** PostgreSQL, Dapper (ORM)
* **File Storage:** MinIO (Local) / Amazon S3 (Production)
* **Infrastructure:** Docker, AWS (Amplify, EC2, RDS)

## How to get started

### Prerequisites
* Docker Desktop
* .NET 9 SDK
* Node.js (v20+)

### Local Setup

1.  **Clone the repository**
    ```bash
    git clone https://github.com/jerarddahuyag-code/CommunityIncidentReportApp.git
    cd CommunityWatch
    ```
2.  **Set up Environment Variables**
    At the root folder, create a .env file with the following keys (Provide your own values):
    ```txt
    POSTGRES_USER=
    POSTGRES_PASSWORD=
    POSTGRES_DB=
    MINIO_ROOT_USER=
    MINIO_ROOT_PASSWORD=
    ClientUrl=
    ConnectionStrings__Postgres=
    JWT__ValidAudience=
    JWT__ValidIssuer=
    JWT__SecretKey=
    Minio__Endpoint=
    Minio__AccessKey=
    Minio__SecretKey=
    ```
4.  **Start Infrastructure**
    Run the local PostgreSQL database, MinIO storage, and Backend API containers.
    ```bash
    docker-compose up -d

5.  **Run the Frontend**
    Navigate to the frontend project, install dependencies, and start the Vite development server.
    ```bash
    cd ../client
    npm install
    npm run dev
    ```

## Who maintains and contributes
* **Project Lead & Maintainer:** Jerard Kent Dahuyag
