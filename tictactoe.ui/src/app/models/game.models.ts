export type Player = 'X' | 'O';

export type GameMode = 'TwoPlayer' | 'Computer';

export type GameStatus = 'InProgress' | 'Won' | 'Draw';

export interface Move {
  moveNumber: number;
  player: Player;
  row: number;
  column: number;
  cellIndex: number;
}

export interface GameState {
  gameId: string;
  board: (Player | null)[];
  currentPlayer: Player;
  mode: GameMode;
  status: GameStatus;
  winner: Player | null;
  winningCells: number[];
  moveHistory: Move[];
  canUndo: boolean;
}

export interface Scoreboard {
  xWins: number;
  oWins: number;
  draws: number;
}

export interface CreateGameRequest {
  mode: GameMode;
}

export interface MakeMoveRequest {
  player: Player;
  row: number;
  column: number;
}
