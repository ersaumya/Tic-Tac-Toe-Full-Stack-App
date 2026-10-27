import { Component, computed, inject, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Game } from './services/game';
import { GameMode, GameState, Scoreboard } from './models/game.models';

@Component({
  imports: [],
  selector: 'app-root',
  styleUrl: './app.css',
  templateUrl: './app.html',
})
export class App {
  protected readonly title = signal('tictactoe.ui');

  private readonly gameApi = inject(Game);

  readonly game = signal<GameState | null>(null);
  readonly gameMode = signal<GameMode>('TwoPlayer');

  readonly scoreboard = signal<Scoreboard>({
    xWins: 0,
    oWins: 0,
    draws: 0,
  });

  readonly loading = signal(false);

  readonly errorMessage = signal<string | null>(null);

  readonly canUndo = computed(() => this.game()?.canUndo ?? false);

  constructor() {
    this.createGame('TwoPlayer');
    this.loadScoreboard();
  }

  createGame(mode: GameMode): void {
    this.gameMode.set(mode);
    this.loading.set(true);
    this.errorMessage.set(null);

    this.gameApi.createGame({ mode }).subscribe({
      next: (state) => {
        this.game.set(state);
        this.loading.set(false);
      },
      error: () => {
        this.errorMessage.set('Unable to create a new game.');
        this.loading.set(false);
      },
    });
  }

  makeMove(index: number): void {
    const currentGame = this.game();

    if (
      !currentGame ||
      currentGame.status !== 'InProgress' ||
      currentGame.board[index] !== null ||
      this.loading()
    ) {
      return;
    }

    const row = Math.floor(index / 3);
    const column = index % 3;

    this.loading.set(true);
    this.errorMessage.set(null);

    this.gameApi
      .makeMove(currentGame.gameId, {
        player: currentGame.currentPlayer,
        row,
        column,
      })
      .subscribe({
        next: (state) => {
          this.game.set(state);
          this.loading.set(false);

          if (state.status !== 'InProgress') {
            this.loadScoreboard();
          }
        },
        error: () => {
          this.errorMessage.set('Unable to make the move.');
          this.loading.set(false);
        },
      });
  }

  resetGame(): void {
    const currentGame = this.game();

    if (!currentGame) {
      this.createGame('TwoPlayer');
      return;
    }

    this.loading.set(true);

    this.gameApi.resetGame(currentGame.gameId).subscribe({
      next: (state) => {
        this.game.set(state);
        this.loading.set(false);
      },
      error: () => {
        this.errorMessage.set('Unable to reset the game.');
        this.loading.set(false);
      },
    });
  }

  undoMove(): void {
    const currentGame = this.game();

    if (!currentGame || !currentGame.canUndo || this.loading()) {
      return;
    }

    this.loading.set(true);

    this.gameApi.undo(currentGame.gameId).subscribe({
      next: (state) => {
        this.game.set(state);
        this.loading.set(false);
      },
      error: () => {
        this.errorMessage.set('Unable to undo the move.');
        this.loading.set(false);
      },
    });
  }

  loadScoreboard(): void {
    this.gameApi.getScoreboard().subscribe({
      next: (scores) => this.scoreboard.set(scores),
      error: () => {
        this.errorMessage.set('Unable to load the scoreboard.');
      },
    });
  }

  resetScoreboard(): void {
    this.loading.set(true);
    this.errorMessage.set(null);

    this.gameApi.resetScoreboard().subscribe({
      next: () => {
        this.scoreboard.set({ xWins: 0, oWins: 0, draws: 0 });
        this.loading.set(false);
      },
      error: () => {
        this.errorMessage.set('Unable to reset the scoreboard.');
        this.loading.set(false);
      },
    });
  }

  isWinningCell(index: number): boolean {
    return this.game()?.winningCells.includes(index) ?? false;
  }

  getCellLabel(index: number): string {
    const row = Math.floor(index / 3) + 1;
    const column = (index % 3) + 1;

    return `Row ${row}, Column ${column}`;
  }
}
