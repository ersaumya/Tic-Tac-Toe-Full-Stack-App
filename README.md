# Tic Tac Toe
# Project overview 
The Tic Tac Toe application is a full-stack web application developed using Angular 22 for the frontend and ASP.NET Core Web API (.NET 10) for the backend. It allows users to play Tic Tac Toe in two modes: 

Two-Player Mode, where two players take turns on the same board.

Computer Mode, where a player competes against a computer opponent.
# Tech stack 

o	Frontend: Angular 22,TypeScript,Bootstrap 

o	Backend: .NET 10 WEB API,C#,OpenAPI/Scalar

o	API Style: REST API 

o	Storage: In-memory storage

<img width="1120" height="674" alt="image" src="https://github.com/user-attachments/assets/8d5724da-b1bc-44fb-a269-8b615806976c" />


# Features implemented 

Select a game mode: two-player or computer opponent.

Create a new game.

Retrieve a game's current state.

Display the 3 × 3 board.

Submit a move by row and column.

Track the current player and game status.

Detect a winner or a draw and expose winning cells.

Keep move history.

Undo a move.

Reset a game.

Retrieve the scoreboard.

Reset the scoreboard.


# How to run the backend locally 

Install .NET 10 sdk

```
cd tictactoe.api
dotnet restore
dotnet run
```
# How to run the frontend locally 
Install 

Angular CLI       : 22.2.4

Node.js           : 24.19.0
```
cd tictactoe.ui
npm install
npm start
```
# API endpoint summary 

https://localhost:7265/scalar/

<img width="260" height="293" alt="image" src="https://github.com/user-attachments/assets/7a5b6121-cc0e-4178-8535-a1c3b7927519" />

| Method	| Endpoint | Purpose |
| -------- |---------|--------|
| POST	|/api/games	|Create a new game session|
|GET	|/api/games/{id}	|Get current game state|
|POST	|/api/games/{id}/moves	|Submit a player move|
|POST	|/api/games/{id}/undo	|Undo last move|
|POST	|/api/games/{id}/reset	|Reset the current game|
|GET	|/api/scoreboard	|Get scoreboard|
|POST	|/api/scoreboard/reset	|Reset scoreboard|

