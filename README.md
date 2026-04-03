# Multiplayer Blackjack - Client/Server Application

## Overview
A fully functional, multiplayer Blackjack game built from scratch using **C#** and **Windows Forms (.NET)**. This project demonstrates a complete Client-Server architecture, handling real-time communication between multiple players and a central server. 

Initially developed as a university project for the Object-Oriented Programming II course, this application showcases my fast-learning capabilities. It highlights my transition from zero programming knowledge to implementing advanced OOP concepts, multithreading, and socket-based networking in a very short amount of time.

## Key Features
* **Client-Server Architecture:** Utilizes `TcpListener` and `TcpClient` for robust, real-time bidirectional communication.
* **Custom Network Protocol:** Implemented a custom text-based messaging protocol (handling states like `HIT`, `STAND`, `NEW_GAME`, and synchronizing player balances/stats).
* **Advanced OOP Principles:**
  * Encapsulation, Inheritance, and Polymorphism.
  * Abstract classes for dynamic betting systems (`Real Money` vs `W-L-D Stats`).
* **Multithreading & Concurrency:** The server runs on a separate background thread, utilizing safe `InvokeRequired` delegates to update the WinForms UI without freezing.
* **Dynamic UI:** Real-time rendering of card images, player states, and scores.

## Tech Stack
* **Language:** C#
* **Framework:** .NET / Windows Forms (WinForms)
* **Networking:** TCP/IP Sockets
* **Concepts:** Object-Oriented Programming (OOP), Multithreading, Client-Server Sync

## How to Run
1. Clone the repository.
2. Open the solution in **Visual Studio**.
3. Build and run the project.
4. **Start the Server:** Click "Pornește Server" to initialize the host on port `8888`.
5. **Connect Clients:** Open multiple instances of the app, enter a Player Name, leave IP as `127.0.0.1` (for local testing), and click "Conectează".
6. Place your bets and enjoy!

## Screenshots
<img width="878" height="579" alt="gameplay1" src="https://github.com/user-attachments/assets/b13c7af4-4635-4b2b-a9ab-06addceb8a14" />
<img width="1760" height="591" alt="gameplay2" src="https://github.com/user-attachments/assets/fe4a0534-5d7b-40f4-9180-4a2bb145cfbb" />
