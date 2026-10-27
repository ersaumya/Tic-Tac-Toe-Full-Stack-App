import { HttpClient } from '@angular/common/http';
import { inject, Service } from '@angular/core';
import { CreateGameRequest, GameState, MakeMoveRequest, Scoreboard } from '../models/game.models';
import { Observable } from 'rxjs';

@Service()
export class Game {
  private readonly http = inject(HttpClient);

  private readonly apiUrl = 'https://localhost:7265/api';

  createGame(request: CreateGameRequest): Observable<GameState> {
    return this.http.post<GameState>(`${this.apiUrl}/games`, request);
  }

  getGame(gameId: string): Observable<GameState> {
    return this.http.get<GameState>(`${this.apiUrl}/games/${gameId}`);
  }

  makeMove(gameId: string, request: MakeMoveRequest): Observable<GameState> {
    return this.http.post<GameState>(`${this.apiUrl}/games/${gameId}/moves`, request);
  }

  undo(gameId: string): Observable<GameState> {
    return this.http.post<GameState>(`${this.apiUrl}/games/${gameId}/undo`, {});
  }

  resetGame(gameId: string): Observable<GameState> {
    return this.http.post<GameState>(`${this.apiUrl}/games/${gameId}/reset`, {});
  }

  getScoreboard(): Observable<Scoreboard> {
    return this.http.get<Scoreboard>(`${this.apiUrl}/scoreboard`);
  }

  resetScoreboard(): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/scoreboard/reset`, null);
  }
}
