import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { SudokuGridComponent, SelectorDialog, TileCardComponent } from './sudoku-grid/sudoku-grid.component';
import { DemoMaterialModule } from './material-module';

@NgModule({
  declarations: [
    AppComponent,
    SudokuGridComponent,
    SelectorDialog,
    TileCardComponent,
  ],
  imports: [
    BrowserModule,
    DemoMaterialModule,
    AppRoutingModule,
    BrowserAnimationsModule,
  ],
  providers: [],
  bootstrap: [AppComponent],
  entryComponents: [SelectorDialog]
})
export class AppModule { }
