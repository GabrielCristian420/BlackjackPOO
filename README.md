# Multiplayer Blackjack - Distributed Client/Server & REST API 🃏

## Overview
A fully functional, multiplayer Blackjack game built from scratch. This project has evolved into a **Distributed System**, combining a real-time **C# (.NET) Client/Server architecture** with a modern **Python (FastAPI) RESTful backend**. 

> **🔗 Backend API Repository:** This game connects to a production-grade leaderboard backend built with FastAPI (Python): [Blackjack-Leaderboard-API](https://github.com/GabrielCristian420/Blackjack-Leaderboard-API)

Initially developed to master Object-Oriented Programming and TCP socket communication, the application now demonstrates full-stack integration capabilities by asynchronously communicating with a dedicated database-driven leaderboard microservice.

## Key Features
* **Real-Time Client-Server Architecture:** Utilizes `TcpListener` and `TcpClient` for robust, bidirectional communication and game state synchronization.
* **REST API Integration:** Asynchronously sends post-game results to a Python/FastAPI backend using `HttpClient`, preventing UI thread-blocking.
* **JWT Authentication:** Implemented automated OAuth2/JWT token fetching directly from the WinForms client to securely post scores.
* **Global Leaderboard:** Fetches and parses live JSON data to display the Top 10 players globally directly in the desktop app.
* **Custom Network Protocol:** Implemented a custom text-based messaging protocol handling states like `HIT`, `STAND`, and `NEW_GAME`.
* **Advanced OOP Principles:** Features deep encapsulation, inheritance, and polymorphism (e.g., dynamic betting systems for `Real Money` vs `W-L-D Stats`).
* **Multithreading & Concurrency:** The game server runs on a background thread, safely updating the WinForms UI via `InvokeRequired` delegates.

## Tech Stack
**Frontend & Game Server:**
* **Language:** C#
* **Framework:** .NET / Windows Forms (WinForms)
* **Networking:** TCP/IP Sockets, `HttpClient` for HTTP requests

**Leaderboard Backend (Microservice):**
* **Language:** Python 3.10+
* **Framework:** FastAPI
* **Database:** PostgreSQL / SQLite (managed via SQLAlchemy & Alembic)
* **Security:** JWT (JSON Web Tokens) Bearer Auth

## How to Run

### 1. Start the Leaderboard API (Python)
1. Clone the API repository: [Blackjack-Leaderboard-API](https://github.com/GabrielCristian420/Blackjack-Leaderboard-API)
2. Install dependencies: `pip install -r requirements.txt`
3. Run the FastAPI server: `uvicorn main:app --reload`
4. *Note: Ensure a test user (e.g., `GameServer`) is registered in the API via `http://127.0.0.1:8000/docs` for JWT generation.*

### 2. Start the Game (C#)
1. Clone this repository and open the solution in **Visual Studio**.
2. Build and run the project.
3. **Start the Game Server:** Click "Pornește Server" to initialize the host on port `8888`.
4. **Connect Clients:** Open multiple instances of the app, enter a Player Name, and click "Conectează".
5. **View Global Leaderboard:** Click the "GLOBAL TOP 10" button to fetch live data from the FastAPI backend!

## Screenshots
<img width="878" height="579" alt="gameplay1" src="https://github.com/user-attachments/assets/b13c7af4-4635-4b2b-a9ab-06addceb8a14" />
<img width="1760" height="591" alt="gameplay2" src="https://github.com/user-attachments/assets/fe4a0534-5d7b-40f4-9180-4a2bb145cfbb" />
