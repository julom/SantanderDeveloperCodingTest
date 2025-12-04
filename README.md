# SantanderDeveloperCodingTest - HackerNews Best Stories API

A simple RESTful API built with ASP.NET Core 9 Minimal API, which returns the top N stories from Hacker News sorted by score — designed to be efficient, resilient and easy to run.

## 🧠 What this project does

- Fetches list of top stories IDs from Hacker News (`beststories.json`)
- Fetches details for each story (`item/{id}.json`)
- Returns JSON response with the following fields for each story:
  - `title`, `uri`, `postedBy`, `time` (ISO 8601), `score`, `commentCount`  
- Supports a query parameter `n` to return top **n** stories  
- Uses in-memory caching + output caching + retry & timeout policies to prevent overloading Hacker News API  
- Provides health checks and rate-limiting  

## 🚀 Quickstart

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)  
- (Optional) Docker  

### Run locally

```bash
git clone https://github.com/julom/SantanderDeveloperCodingTest.git
cd SantanderDeveloperCodingTest
dotnet run --project src/SantanderDeveloperCodingTest
```

The API will be available at https://localhost:7273

Example request:

GET /bestStories?n=5

## 📬 API Endpoints

GET /bestStories?n={n}

Query parameter: n (integer, >= 1) — number of top stories to return

Success response: HTTP 200 OK

Response body: JSON array of story objects sorted by score desc

### Story object example:

{
  "title": "Example story",
  "uri": "https://example.com/story",
  "postedBy": "username",
  "time": "2025-12-04T13:45:00+00:00",
  "score": 1234,
  "commentCount": 56
}

### Error responses:

400 Bad Request — invalid query parameter (e.g. n <= 0) → returns ProblemDetails JSON

500 Internal Server Error — when external API fails and no cached data available → returns ProblemDetails

## ✅ Assumptions & Constraints

- Hacker News data changes data mostly by adding new stories infrequently — caching with short TTL (e.g. 30–60 s) is acceptable

- Hacker News stories should should not be modified too often - caching stories content with TTL e.g. 30 mins is acceptable

- No authentication required — API is public read-only proxy

- For simplicity, the number n is not globally limited, but very large n may cause performance issues

- Uses in-memory cache — not suitable for multi-instance deployments

## 🔧 What I would improve with more time / in production

- Switch to distributed cache (e.g. Redis) for scalability & multi-instance support

- Add request rate-limiting *per client* to avoid abuse, add circuit breaker for Hacker News API when it's unaccessible

- Add logging & metrics (e.g. Prometheus) to monitor upstream calls and cache efficiency

- Add more integration tests (testing cache policies)

- Add more robust error handling and fallback logic (e.g. stale-while-revalidate caching)

- Expand and refine CORS configuration and header rules

- Further structure and clean up Program.cs - The Program.cs file is intentionally compact for clarity, but it contains multiple responsibilities.