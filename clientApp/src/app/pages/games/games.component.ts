import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';

import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { first } from 'rxjs/operators';

import { GameService } from '../../../app/services/game.service';
import { GameInfo } from './games.model';


@Component({
  selector: 'app-customers',
  templateUrl: './games.component.html',
  styleUrls: ['./games.component.scss']
})

/**
* Ecomerce Customers component
*/
export class GamesComponent implements OnInit {

  // bread crumb items
  breadCrumbItems: Array<{}>;
  statusMessage: string; // message to show after delete the record
  gameInfo: GameInfo[] = [];
  term: any; // Variable to search the text in grid
  typesubmit: boolean;
  // page
  currentpage: number;
  totalPages: number;
  public tableArray = [];
  public tableHeader = [];
  HighlightRow: Number;
  HighlightCol: Number;
  highlightedColArray = [];
  highlightedRowArray = [];
  selectedCellForWinner = [];
  typeValidationForm: FormGroup; // type validation form
  constructor(private gameService: GameService, private modalService: NgbModal, public formBuilder: FormBuilder) {
    this.typesubmit = false;
  }

  ngOnInit() {
    // setup the top lables
    this.breadCrumbItems = [{
      label: 'Skote'
    }, {
      label: 'Games',
      active: true
    }];

    this.currentpage = 1;
    /**
     * Fetches the games data
     */
    this.loadAllGames();

    this.generateRondomNumber();
    /**
     * Type validation form
     */
    this.typeValidationForm = this.formBuilder.group({
      gameID: ['-1'],
      gameName: ['', [Validators.required]],
      // cell1Value: ['', [Validators.required]],
      // cell2Value: ['', [Validators.required]],
      // cell3Value: ['', [Validators.required]],
      // cell4Value: ['', [Validators.required]],
    });
  }

  /**
   * All employees data fetches
   */
  private loadAllGames() {
    this.gameService.getAll().pipe(first()).subscribe(games => {
      this.gameInfo = games;
      this.totalPages = 0;
    });
  }

  private generateRondomNumber() {

    for (var i = 1; i <= 300; i++) {
      var col = 'col' + i;
      this.tableHeader.push(col);
    }

    var arr = [];
    while (arr.length < 1800) {
      var r = Math.floor(Math.random() * 1800) + 1
      if (arr.indexOf(r) === -1) arr.push(r);
    }

    for (let i = 6; i > 0; i--) {
      var sliced = arr.splice(0, Math.ceil(arr.length / i));
      var jsonData = {};
      for (var j = 0; j < sliced.length; j++) {
        var k = j + 1;
        jsonData['col' + k] = sliced[j];
      }
      this.tableArray.push(jsonData);
    }
    console.log(this.tableArray)

  }
  private getPivotArray(dataArray, rowIndex, colIndex, dataIndex) {
    //Code from https://techbrij.com
    var result = {}, ret = [];
    var newCols = [];
    for (var i = 0; i < dataArray.length; i++) {

      if (!result[dataArray[i][rowIndex]]) {
        result[dataArray[i][rowIndex]] = {};
      }
      result[dataArray[i][rowIndex]][dataArray[i][colIndex]] = dataArray[i][dataIndex];

      //To get column names
      if (newCols.indexOf(dataArray[i][colIndex]) == -1) {
        newCols.push(dataArray[i][colIndex]);
      }
    }

    newCols.sort();
    var item = [];

    //Add Header Row
    item.push('Item');
    item.push.apply(item, newCols);
    ret.push(item);

    //Add content 
    for (var key in result) {
      item = [];
      item.push(key);
      for (var i = 0; i < newCols.length; i++) {
        item.push(result[key][newCols[i]] || "-");
      }
      ret.push(item);
    }
    return ret;
  }
  clickedEvent(row: any, col: any, eve: any) {
    this.highlightedColArray.push(col);
    this.highlightedRowArray.push(row);
    this.HighlightCol = col;
    this.HighlightRow = row;

    const index = this.selectedCellForWinner.indexOf(eve.currentTarget.innerText);
    if (index > -1) {
      eve.currentTarget.classList.remove("active");
      this.selectedCellForWinner.splice(index, 1);
    }
    else {
      eve.currentTarget.classList.add("active");
      this.selectedCellForWinner.push(eve.currentTarget.innerText);
    }

  }


  deleteGame(info: GameInfo) {
    this.gameService.deleteGame(info.gameID).subscribe((resp: Response) => {
      this.statusMessage = 'Record Deleted Successfully.',
        this.loadAllGames();
    });

  }

  openModal(largeDataModal: any, info: GameInfo) {
    this.typesubmit = false;
    this.modalService.open(largeDataModal, {
      backdrop: 'static',
      size: 'xl',
      scrollable: true
    });
    if (info) {
      this.typeValidationForm.patchValue({
        gameID: info.gameID,
        gameName: info.gameName
      });
    } else {
      this.typeValidationForm.patchValue({
        gameID: "-1",
        gameName: ""

      });
    }

  }
  /**
   * Returns the type validation form
   */
  get type() {
    return this.typeValidationForm.controls;
  }

  /**
   * Type validation form submit data
   */
  typeSubmit() {

    if (this.typeValidationForm.valid && this.selectedCellForWinner.length > 5) {
      this.typesubmit = false;

      var finalArray = [];
      for (var i = 0; i < 300; i++) {
        var j = i + 1;
        var gameDetails = {
          'gameID': 0,
          'gameDetailID': 0,
          'valueOfRow1': this.tableArray[0]['col' + j],
          'valueOfRow2': this.tableArray[1]['col' + j],
          'valueOfRow3': this.tableArray[2]['col' + j],
          'valueOfRow4': this.tableArray[3]['col' + j],
          'valueOfRow5': this.tableArray[4]['col' + j],
          'valueOfRow6': this.tableArray[5]['col' + j],
        };
        finalArray.push(gameDetails);
      }


      var objGameInfo = {
        'gameID': Number(this.typeValidationForm.get('gameID').value),
        'gameName': this.typeValidationForm.get('gameName').value,
        'winValue1': Number(this.selectedCellForWinner[0]),
        'winValue2': Number(this.selectedCellForWinner[1]),
        'winValue3': Number(this.selectedCellForWinner[2]),
        'winValue4': Number(this.selectedCellForWinner[3]),
        'winValue5': Number(this.selectedCellForWinner[4]),
        'winValue6': Number(this.selectedCellForWinner[5]),
        'IsDeleted': false
      };

      var objFinal = {
        'gameInfo': objGameInfo,
        'gameDetail': finalArray
      };

      if (this.typeValidationForm.get('gameID').value === "-1") {

        this.gameService.postGame(objFinal).subscribe(response => {
          this.statusMessage = 'Game is created Successfully.'
          this.loadAllGames();
        }, (error) => {
          console.log('error during post is ', error)
        });
      } else {
        this.gameService.updateGame(objFinal, this.typeValidationForm.get('gameID').value).subscribe(response => {
          this.statusMessage = 'Game is updated Successfully.'
          this.loadAllGames();
        }, (error) => {
          console.log('error during post is ', error)
        });
      }
      this.modalService.dismissAll();

    } else {
      // validate all form fields
      this.typesubmit = true;
    }

  }
  reset() {
    this.typeValidationForm.reset();
    this.typesubmit = false;
  }
}