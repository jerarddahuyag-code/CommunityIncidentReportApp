CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- 1. USERS TABLE
CREATE TABLE IF NOT EXISTS users (
    Id UUID PRIMARY KEY,
    Username VARCHAR(50) UNIQUE NOT NULL,
    DisplayName VARCHAR(100),
    PasswordHash TEXT NOT NULL,
    Role VARCHAR(20) NOT NULL,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 2. INVITES TABLE
CREATE TABLE IF NOT EXISTS invites (
    Id SERIAL PRIMARY KEY,
    Code VARCHAR(20) UNIQUE NOT NULL,
    IsUsed BOOLEAN DEFAULT FALSE,
    CreatedBy UUID NOT NULL,
    CONSTRAINT fk_invites_user
        FOREIGN KEY (CreatedBy) 
        REFERENCES users(Id)
);

-- 3. INCIDENTS TABLE
CREATE TABLE IF NOT EXISTS incidents (
    Id UUID PRIMARY KEY,
    UserId UUID NOT NULL,
    Title VARCHAR(150) NOT NULL,
    Description TEXT,
    Category VARCHAR(50) NOT NULL,
    Latitude DECIMAL(9,6),
    Longitude DECIMAL(9,6),
    Status VARCHAR(20) DEFAULT 'Reported',
    ImageUrl TEXT,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_incidents_user
        FOREIGN KEY (UserId) 
        REFERENCES users(Id)
);

-- 4. COMMENTS TABLE
CREATE TABLE IF NOT EXISTS comments (
    Id UUID PRIMARY KEY,
    IncidentId UUID NOT NULL,
    UserId UUID NOT NULL,
    Content TEXT NOT NULL,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_comments_user
        FOREIGN KEY (UserId) 
        REFERENCES users(Id),
    CONSTRAINT fk_comments_incident
        FOREIGN KEY (IncidentId) 
        REFERENCES incidents(Id) 
        ON DELETE CASCADE
);

-- Refresh TOKENS
CREATE TABLE RefreshTokens (
    Id SERIAL PRIMARY KEY,
    UserId UUID NOT NULL,
    Token VARCHAR(255) NOT NULL UNIQUE,
    ExpiresOn TIMESTAMP WITH TIME ZONE NOT NULL,
    CreatedOn TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    RevokedOn TIMESTAMP WITH TIME ZONE NULL
);

-- Adding an index makes looking up the token super fast during the /refresh call
CREATE INDEX IX_RefreshTokens_Token ON RefreshTokens(Token);