import { Component, OnInit, Input } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { trigger, state, style, transition, animate } from '@angular/animations';

export interface DialogData {
  value: number;
}

export interface Tile {
  color: string;
  text: string;
  row: number;
  col: number;
  backgroundColor: string;
}

@Component({
  selector: 'app-sudoku-grid',
  templateUrl: './sudoku-grid.component.html',
  styleUrls: ['./sudoku-grid.component.styl'],
})
export class SudokuGridComponent implements OnInit {
  static defaultPuzzleString: string = "540021070009784012700000608002006100005407000496500837280905700904078000000263489";
  
  boardState: number[][] = [
    [0, 0, 0, 0, 0, 0, 0, 0, 0], 
    [0, 0, 0, 0, 0, 0, 0, 0, 0], 
    [0, 0, 0, 0, 0, 0, 0, 0, 0], 
    [0, 0, 0, 0, 0, 0, 0, 0, 0], 
    [0, 0, 0, 0, 0, 0, 0, 0, 0], 
    [0, 0, 0, 0, 0, 0, 0, 0, 0], 
    [0, 0, 0, 0, 0, 0, 0, 0, 0], 
    [0, 0, 0, 0, 0, 0, 0, 0, 0], 
    [0, 0, 0, 0, 0, 0, 0, 0, 0]
  ];
  solvedState: string[][] = [
    ['unsolved', 'unsolved', 'unsolved', 'unsolved', 'unsolved', 'unsolved', 'unsolved', 'unsolved', 'unsolved', ],
    ['unsolved', 'unsolved', 'unsolved', 'unsolved', 'unsolved', 'unsolved', 'unsolved', 'unsolved', 'unsolved', ],
    ['unsolved', 'unsolved', 'unsolved', 'unsolved', 'unsolved', 'unsolved', 'unsolved', 'unsolved', 'unsolved', ],
    ['unsolved', 'unsolved', 'unsolved', 'unsolved', 'unsolved', 'unsolved', 'unsolved', 'unsolved', 'unsolved', ],
    ['unsolved', 'unsolved', 'unsolved', 'unsolved', 'unsolved', 'unsolved', 'unsolved', 'unsolved', 'unsolved', ],
    ['unsolved', 'unsolved', 'unsolved', 'unsolved', 'unsolved', 'unsolved', 'unsolved', 'unsolved', 'unsolved', ],
    ['unsolved', 'unsolved', 'unsolved', 'unsolved', 'unsolved', 'unsolved', 'unsolved', 'unsolved', 'unsolved', ],
    ['unsolved', 'unsolved', 'unsolved', 'unsolved', 'unsolved', 'unsolved', 'unsolved', 'unsolved', 'unsolved', ],
    ['unsolved', 'unsolved', 'unsolved', 'unsolved', 'unsolved', 'unsolved', 'unsolved', 'unsolved', 'unsolved', ],
  ];

  offsets: number[][] = [
    [0, 0], [0, 3], [0, 6],
    [3, 0], [3, 3], [3, 6],
    [6, 0], [6, 3], [6, 6],
  ];
  
  constructor(public dialog: MatDialog) { 
    if (!SudokuGridComponent.defaultPuzzleString) return;

    for (let index = 0; index < 81; index++) {
      if (SudokuGridComponent.defaultPuzzleString[index] != "0")
      {
        this.boardState[Math.floor(index / 9)][index % 9] = +SudokuGridComponent.defaultPuzzleString[index];
        this.solvedState[Math.floor(index / 9)][index % 9] = 'solved';
      }
    }
  }

  ngOnInit() {    
  }

  getTileSubset(offset:number[]) {
    var result: Tile[] = new Array<Tile>(9);

    for (let i = 0; i < 9; i++) {
      var rowIdx = Math.floor(i / 3) + offset[0];
      var colIdx = (i % 3) + offset[1];
      var currentNum = this.boardState[rowIdx][colIdx];
      result[i] = { 
        text: currentNum.toString(), 
        color: (currentNum == 0 ? "red" : "green"), 
        backgroundColor: (currentNum == 0 ? "pink" : "palegreen"),
        row: rowIdx,
        col: colIdx,
      };
    }

    return result;
  }

  openDialog(tile:Tile): void {
    const dialogRef = this.dialog.open(SelectorDialog, {
      width: "150px",
      data: {value: +tile.text}
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.boardState[tile.row][tile.col] = result.toString();
        this.solvedState[tile.row][tile.col] = 'solved';
      }
    })
  }

}

@Component({
  selector: 'app-selector-popup',
  templateUrl: 'sudoku-grid-selector.component.html',
  styleUrls: ['sudoku-grid.component.styl']
})
export class SelectorDialog {
  choices: number[] = [1, 2, 3, 4, 5, 6, 7, 8, 9];

  constructor () { }

}

@Component({
  selector: 'app-tile-card',
  template: '<mat-grid-tile [@solvedUnsolved]="solvedState" class="flexTile"><span class="su-number">{{ this.tile.text }}</span></mat-grid-tile>',
  styleUrls: ['./sudoku-grid.component.styl'],
  animations: [
    trigger('solvedUnsolved', [
      state('solved', style({
        backgroundColor: 'palegreen',
        color: 'green',
        borderColor: 'green',
      })),
      state('unsolved', style({
        backgroundColor: 'pink',
        color: 'red',
        borderColor: 'red',
      })),
      transition('solved => unsolved', [
        animate('2s 1s ease-in')
      ]),
      transition('unsolved => solved', [
        animate('1s 1s ease-out')
      ])
    ])
  ]
})
export class TileCardComponent {
  solvedState = 'unsolved';
  @Input() tile: Tile;

  constructor( ) { 
    if (this.tile
      && this.tile.text != "0") this.solvedState = 'solved';
  }

}
