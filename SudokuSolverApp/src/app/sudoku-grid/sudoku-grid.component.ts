import { Component, OnInit } from '@angular/core';

export interface Tile {
  color: string;
  text: string;
  backgroundColor: string;
}

@Component({
  selector: 'app-sudoku-grid',
  templateUrl: './sudoku-grid.component.html',
  styleUrls: ['./sudoku-grid.component.styl']
})
export class SudokuGridComponent implements OnInit {
  defaultPuzzleString: string = "540021070009784012700000608002006100005407000496500837280905700904078000000263489";
  
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

  offsets: number[][] = [
    [0, 0], [0, 3], [0, 6],
    [3, 0], [3, 3], [3, 6],
    [6, 0], [6, 3], [6, 6],
  ];
  
  constructor() { 
    this.boardState = this.boardFromString(this.defaultPuzzleString);
  }

  ngOnInit() {    
  }

  private boardFromString(boardString:string) {
    if (!boardString) return;
    if (boardString.length != 81) return;

    var board:number[][] = [
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

    for (let index = 0; index < 81; index++) {
      //console.debug("x: "+Math.floor(index / 9).toString()+"   y: "+(index % 9).toString());
      board[Math.floor(index / 9)][index % 9] = +boardString[index];      
    }

    return board;
  }

  getTileSubset(offset:number[]) {
    var result: Tile[] = new Array<Tile>(9);

    for (let i = 0; i < 9; i++) {
      var currentNum = this.boardState[Math.floor(i / 3) + offset[0]][(i % 3) + offset[1]];
      result[i] = { text: currentNum.toString(), color: (currentNum == 0 ? "red" : "green"), backgroundColor: (currentNum == 0 ? "pink" : "palegreen")};
    }

    return result;
  }

  openDialog() {
    alert("works");
  }

}
