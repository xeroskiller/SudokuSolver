import { Component } from '@angular/core';
import { SudokuGridComponent } from './sudoku-grid/sudoku-grid.component'

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.styl']
})
export class AppComponent {
  title = 'SudokuSolverApp';
}
