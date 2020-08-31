import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';

import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { first } from 'rxjs/operators';

import { Users } from '../usermanagement/users.model';

import { GameCompleteInfo, GameValues, GameDetailInfo, GameInfo } from '../games/games.model';
import { environment } from '../../../environments/environment';
import { AlertService } from '../../../app/services/alert.service';

import { GameService } from '../../../app/services/game.service';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@aspnet/signalr';
import { Subscription, Observable, timer, of } from 'rxjs';
import * as moment from 'moment';

@Component({
  selector: 'app-customers',
  templateUrl: './gamestart.component.html',
  styleUrls: ['./gamestart.component.scss']
})

/**
* Ecomerce Customers component
*/
export class GameStartComponent implements OnInit {

  // bread crumb items
  breadCrumbItems: Array<{}>;
  statusMessage: string; // message to show after delete the record
  term: any; // Variable to search the text in grid
  typesubmit: boolean;
  gameCompleteInfo: GameCompleteInfo[] = [];
  // page
  currentpage: number;
  totalPages: number;
  typeValidationForm: FormGroup; // type validation form
  public _hubConnection: HubConnection;
  public _state: HubConnectionState;
  public tableArray = [];
  public tableHeader = [];
  public tableFooter = [];

  public resultTableHeader = [];
  public resultTable = [];
  public resultTableFooter = []

  public selectedCellForWinner = [];
  public AllConfirmedValues = [];
  public AllRowsConfirmedValues = [];
  public AllNotConfirmedValues = [];
  public AllConfirmedValuesByUser = [];
  public AllNotConfirmedValuesByUser = [];
  public AllRowsNotConfirmedValuesByUser = [];
  public AllRowsNotConfirmedValues = [];

  public WinListArray = [];

  public resultColHeader = [];
  public resultColBody = [];
  public resultColFooter = [];
  public resultColTable = [];
  public ColValuesArray = [];

  currentUserID: number;
  public gameID: number;
  public isGameStart: boolean = false;
  public isGamePause: boolean = false;
  public isGameFinish: boolean = false;

  public coinsCost: number = 0;
  public usedCoins: number = 0;
  public remainingCoins: number = 0;

  users: Users[] = [];
  gameValues: GameValues[] = [];
  intervalId: number = 0;
  message: string = '';
  seconds: number = 121;
  public isAdmin: boolean = false;
  public gameData: any;
  public error = '';
  constructor(private gameService: GameService, private modalService: NgbModal, public formBuilder: FormBuilder, private actRoute: ActivatedRoute,
    private alertService: AlertService) {
    this.typesubmit = false;
  }

  private getBoolean(value) {
    switch (value) {
      case true:
      case "true":
      case 1:
      case "1":
      case "on":
      case "yes":
        return true;
      default:
        return false;
    }
  }
  ngOnInit() {
    this.error = '';
    // reset alerts on submit
    this.alertService.clear();

    this.typeValidationForm = this.formBuilder.group({
      winValue1: ['', [Validators.required, Validators.pattern('[0-9]+')]],
      winValue2: ['', [Validators.required, Validators.pattern('[0-9]+')]],
      winValue3: ['', [Validators.required, Validators.pattern('[0-9]+')]],
      winValue4: ['', [Validators.required, Validators.pattern('[0-9]+')]],
      winValue5: ['', [Validators.required, Validators.pattern('[0-9]+')]],
      winValue6: ['', [Validators.required, Validators.pattern('[0-9]+')]],
    });
    // setup the top lables
    this.breadCrumbItems = [{
      label: 'LOTO Game'
    }, {
      label: 'Game Details',
      active: true
    }];

    this.currentpage = 1;
    this.currentUserID = Number(localStorage.getItem('id'));
    this.isAdmin = this.getBoolean(localStorage.getItem('isAdmin'));
    this._hubConnection = new HubConnectionBuilder().withUrl(`${environment.apiUrl}/echo`).build();
    this.actRoute.paramMap.subscribe(params => {
      this.gameID = Number(params.get('id'));
    })
    this._hubConnection.on('GetGameInfo', (data: any) => {

      this.AllConfirmedValues = [];
      this.AllNotConfirmedValues = [];
      this.AllRowsNotConfirmedValues = [];
      this.AllNotConfirmedValuesByUser = [];
      this.AllConfirmedValuesByUser = [];
      this.AllRowsNotConfirmedValuesByUser = [];
      this.AllRowsConfirmedValues = [];
      this.WinListArray = [];
      this.isGameStart = false;
      this.isGamePause = false;
      this.isGameFinish = false;

      this.coinsCost = 0;
      this.usedCoins = 0;
      this.remainingCoins = 0;

      if (data) {

        var gameInfo = data.gameInfo;
        var userInfo = data.userInfo;
        if (userInfo) {
          this.coinsCost = userInfo.coinsCost;
          this.usedCoins = userInfo.usedCoins;
          this.remainingCoins = userInfo.remainingCoins;

        }
        if (gameInfo && gameInfo.length > 0) {

          this.gameData = gameInfo[0];
          this.isGameStart = gameInfo[0].isGameStart;
          this.isGamePause = gameInfo[0].isGamePause;
          this.isGameFinish = gameInfo[0].isGameFinish;

          this.WinListArray.push(gameInfo[0].winValue1, gameInfo[0].winValue2, gameInfo[0].winValue3, gameInfo[0].winValue4, gameInfo[0].winValue5, gameInfo[0].winValue6);
        }
        var gameValuesList = data.gameValues;
        if (gameValuesList) {
          for (var i = 0; i < gameValuesList.length; i++) {
            if (gameValuesList[i]['isConfirmed'] === true) {

              this.AllConfirmedValues.push(gameValuesList[i]['gameValueID']);

              var tempObj = [];
              tempObj.push(gameValuesList[i]['rowNum1'], gameValuesList[i]['rowNum2'], gameValuesList[i]['rowNum3'], gameValuesList[i]['rowNum4'], gameValuesList[i]['rowNum5'], gameValuesList[i]['rowNum6']);

              var tempCol = [];
              tempCol.push(gameValuesList[i]['gameValueID']);

              const elementsIndex = this.AllRowsConfirmedValues.findIndex(element => element.userID == gameValuesList[i]['userID']);

              if (elementsIndex < 0) {

                var userDetail = {
                  userName: gameValuesList[i]['userName'],
                  userID: gameValuesList[i]['userID'],
                  valuesList: tempObj,
                  machedResult: 0,
                  userCols: tempCol
                };

                this.AllRowsConfirmedValues.push(userDetail);

              }
              else {


                var newArray = this.AllRowsConfirmedValues[elementsIndex]['valuesList'].concat(tempObj);
                this.AllRowsConfirmedValues[elementsIndex].valuesList = newArray;

                var currentUserCols = this.AllRowsConfirmedValues[elementsIndex]['userCols'].concat(tempCol);
                this.AllRowsConfirmedValues[elementsIndex].userCols = currentUserCols;

              }
            }
            if (gameValuesList[i]['isConfirmed'] === false) {
              this.AllNotConfirmedValues.push(gameValuesList[i]['gameValueID']);

              var tempObj = [];
              tempObj.push(gameValuesList[i]['rowNum1'], gameValuesList[i]['rowNum2'], gameValuesList[i]['rowNum3'], gameValuesList[i]['rowNum4'], gameValuesList[i]['rowNum5'], gameValuesList[i]['rowNum6']);
              this.AllRowsNotConfirmedValues.push(tempObj);
            }
            if (gameValuesList[i]['isConfirmed'] === false && gameValuesList[i]['userID'] === this.currentUserID) {
              this.AllNotConfirmedValuesByUser.push(gameValuesList[i]['gameValueID']);
              var tempObj = [];
              tempObj.push(gameValuesList[i]['rowNum1'], gameValuesList[i]['rowNum2'], gameValuesList[i]['rowNum3'], gameValuesList[i]['rowNum4'], gameValuesList[i]['rowNum5'], gameValuesList[i]['rowNum6']);
              this.AllRowsNotConfirmedValuesByUser.push(tempObj);

            }
            if (gameValuesList[i]['isConfirmed'] === true && gameValuesList[i]['userID'] === this.currentUserID) {
              this.AllConfirmedValuesByUser.push(gameValuesList[i]['gameValueID']);
            }
          }
        }
        this.CalculateResult();
      }
    });
    this._hubConnection.start()
      .then(() => {

        this._hubConnection.invoke('GameStart', this.gameID, this.currentUserID);

        console.log('Hub connection started')
      })
      .catch(err => {
        console.log('Error while establishing connection')
      });
    this.getGameValues();
  }
  CalculateResult() {
    /**Make a header for resultant table */
    this.resultTableHeader = [];
    this.resultTable = [];
    for (var i = 0; i < this.AllRowsConfirmedValues.length; i++) {
      var col = 'col' + i;
      this.resultTableHeader.push(col);
    }

    /**Make a body for resultant table */
    var maxLength = 0;

    for (var i = 0; i < this.AllRowsConfirmedValues.length; i++) {

      var lengthOfList = this.AllRowsConfirmedValues[i]['valuesList'].length;

      if (maxLength < lengthOfList) {

        maxLength = lengthOfList;
      }

    }

    /**Push values in temp table */

    for (var i = 0; i < maxLength; i++) {
      var jsonDataOfRow = {};
      for (var j = 0; j < this.AllRowsConfirmedValues.length; j++) {
        jsonDataOfRow['col' + j] = -1;
      }
      this.resultTable.push(jsonDataOfRow);
    }

    /**Update a table body with values for each table */
    for (var i = 0; i < this.AllRowsConfirmedValues.length; i++) {
      var lengthOfList = this.AllRowsConfirmedValues[i]['valuesList'].length;
      var valuesList = this.AllRowsConfirmedValues[i]['valuesList'];
      for (var j = 0; j < lengthOfList; j++) {
        this.resultTable[j]['col' + i] = valuesList[j];
      }
      var intersection = valuesList.filter(element => this.WinListArray.includes(element));
      var resultLenght = intersection.length;
      this.AllRowsConfirmedValues[i]['machedResult'] = resultLenght;
    }

    this.tableFooter = [];
    console.log(this.tableArray)
    /**Start-  Logic for column footer */
    for (var i = 0; i < this.gameValues.length; i++) {
      var arrayGameDetail = [];

      for (let key in this.gameValues[i]) {
        if (this.gameValues[i].hasOwnProperty(key)) {
          arrayGameDetail.push(this.gameValues[i][key]);
        }
      }
      var matched = arrayGameDetail.filter(element => this.WinListArray.includes(element));
      this.tableFooter.push(matched.length);
    }
    /**End-  Logic for column footer */

    /*Make table of numbers and columns*/
    this.resultColHeader = [];
    this.resultColHeader.push(0, 1, 2, 3, 4, 5, 6);
    var lengthOfResultTable = 0;
    for (var i = 0; i < this.resultColHeader.length; i++) {
      let count = this.GetTheNumberCount(this.tableFooter, this.resultColHeader[i]);
      if (lengthOfResultTable < count) {

        lengthOfResultTable = count;
      }
    }

    /**Push values in temp table */
    this.resultColTable = [];
    this.resultColBody = [];
    this.ColValuesArray = [];
    for (var i = 0; i < this.resultColHeader.length; i++) {
      const indexes = this.tableFooter.reduce((r, n, k) => {
        n === this.resultColHeader[i] && r.push(k);

        return r;
      }, []);
      var col = 'col' + i;
      this.resultColTable.push(col);
      this.ColValuesArray.push({ colNumber: i, colValues: indexes, countOfMatchedCol: indexes.length });
    }
    for (var i = 0; i < lengthOfResultTable; i++) {
      var jsonDataOfRow = {};
      for (var j = 0; j < this.resultColHeader.length; j++) {
        jsonDataOfRow['col' + j] = -1;
      }
      this.resultColBody.push(jsonDataOfRow);
    }
    /** */
    /**Update a resultant table body with values for each table */
    for (var i = 0; i < this.ColValuesArray.length; i++) {
      var lengthOfList = this.ColValuesArray[i]['colValues'].length;
      var valuesList = this.ColValuesArray[i]['colValues'];
      for (var j = 0; j < lengthOfList; j++) {
        this.resultColBody[j]['col' + i] = valuesList[j] + 1;
      }
    }
    console.log(this.resultColBody);
  }
  GetTheNumberCount(arr: any, num: number) {
    return arr.reduce((n, x) => n + (x === num), 0)
  }
  GetNotConfirmedList() {
    const filteredUserMenus = this.tableArray.filter((item) => {
      return item.roles.find((role) => role === '2');
    });
  }
  ChangeGameStatus(status: string) {
    var isStart = false;
    var isStop = false;
    var isFinish = false;
    if (status == 'start') {
      isStart = true;
      isStop = false;
      isFinish = false;
    }
    if (status == 'pause') {
      isStart = false;
      isStop = true;
      isFinish = false;
    }
    if (status == 'finish') {
      isStart = false;
      isStop = false;
      isFinish = true;
    }

    var objGameInfo = {
      'gameID': this.gameID,
      'gameName': '',
      'winValue1': 0,
      'winValue2': 0,
      'winValue3': 0,
      'winValue4': 0,
      'winValue5': 0,
      'winValue6': 0,
      'isDeleted': false,
      'isGameStart': isStart,
      'isGamePause': isStop,
      'isGameFinish': isFinish,
    };
    this.gameService.UpdateGameStatus(objGameInfo).pipe(first()).subscribe(data => {
      this._hubConnection.invoke('GameStart', this.gameID, this.currentUserID);
    });

  }
  private getGameValues() {
    this.gameService.getGameValues().pipe(first()).subscribe(data => {

      for (var i = 1; i <= 300; i++) {
        var col = 'Col ' + i;
        this.tableHeader.push(col);
      }
      this.gameValues = data;
      let arrayGameDetail = [];

      for (let key in this.gameValues) {
        if (this.gameValues.hasOwnProperty(key)) {
          arrayGameDetail.push(this.gameValues[key]);
        }
      }
      var jsonDataOfRow1 = {};
      var jsonDataOfRow2 = {};
      var jsonDataOfRow3 = {};
      var jsonDataOfRow4 = {};
      var jsonDataOfRow5 = {};
      var jsonDataOfRow6 = {};
      for (var j = 0; j < arrayGameDetail.length; j++) {
        var k = j + 1;
        jsonDataOfRow1['Col ' + k] = arrayGameDetail[j]['rowNum1'];
        jsonDataOfRow2['Col ' + k] = arrayGameDetail[j]['rowNum2'];
        jsonDataOfRow3['Col ' + k] = arrayGameDetail[j]['rowNum3'];
        jsonDataOfRow4['Col ' + k] = arrayGameDetail[j]['rowNum4'];
        jsonDataOfRow5['Col ' + k] = arrayGameDetail[j]['rowNum5'];
        jsonDataOfRow6['Col ' + k] = arrayGameDetail[j]['rowNum6'];
      }
      this.tableArray.push(jsonDataOfRow1);
      this.tableArray.push(jsonDataOfRow2);
      this.tableArray.push(jsonDataOfRow3);
      this.tableArray.push(jsonDataOfRow4);
      this.tableArray.push(jsonDataOfRow5);
      this.tableArray.push(jsonDataOfRow6);

    });
  }
  clickedEvent(eve: any) {
    if (this.remainingCoins == 0 || this.remainingCoins < 100) {
      this.error = 'You can not select further columns as your coins are less than $100. Please contact your administrator for further coins';
      return;
    }
    this.error = '';
    if (!this.isGameStart) {
      return;
    }
    if (this.isGamePause || this.isGameFinish) {
      return;
    }
    var columnNumber = Number(eve.currentTarget.innerText.replace("Col", "").trim());
    var allIndex = this.AllConfirmedValues.indexOf(columnNumber);
    if (allIndex > -1) {
      return;
    }
    eve.currentTarget.classList.add("current");
    var row1 = document.getElementById("td_0_Col " + columnNumber);
    var row2 = document.getElementById("td_1_Col " + columnNumber);
    var row3 = document.getElementById("td_2_Col " + columnNumber);
    var row4 = document.getElementById("td_3_Col " + columnNumber);
    var row5 = document.getElementById("td_4_Col " + columnNumber);
    var row6 = document.getElementById("td_5_Col " + columnNumber);


    var rowValue1 = Number(row1.innerText.trim());
    var rowValue2 = Number(row2.innerText.trim());
    var rowValue3 = Number(row3.innerText.trim());
    var rowValue4 = Number(row4.innerText.trim());
    var rowValue5 = Number(row5.innerText.trim());
    var rowValue6 = Number(row6.innerText.trim());


    var rowValuesArray = [];
    rowValuesArray.push(rowValue1);
    rowValuesArray.push(rowValue2);
    rowValuesArray.push(rowValue3);
    rowValuesArray.push(rowValue4);
    rowValuesArray.push(rowValue5);
    rowValuesArray.push(rowValue6);


    var objColDetailWithRows = {
      gameValueID: columnNumber,
      gameID: this.gameID,
      userID: this.currentUserID,
      isConfirmed: false,
      isDeleted: false
    };
    var notByUserindex = this.AllNotConfirmedValuesByUser.indexOf(columnNumber);
    var allNotIndex = this.AllNotConfirmedValues.indexOf(columnNumber);
    if (notByUserindex < 0 && allNotIndex < 0) {
      row1.classList.add("borderRightAndLeft");
      row2.classList.add("borderRightAndLeft");
      row3.classList.add("borderRightAndLeft");
      row4.classList.add("borderRightAndLeft");
      row5.classList.add("borderRightAndLeft");
      row6.classList.add("borderRightAndLeft");
      eve.currentTarget.classList.add("current");
      //this.AllNotConfirmedValuesByUser.splice(notByUserindex, 1);
      this.AllNotConfirmedValues.push(columnNumber);
      this.InsertSelectedColValue(objColDetailWithRows);
    }
    else if (notByUserindex > -1) {
      eve.currentTarget.classList.remove("current");
      this.AllNotConfirmedValuesByUser.splice(notByUserindex, 1);

      this.removeValueFromConfirmValue(columnNumber, this.gameID);
    }
    else {
      return;
    }

    if (this.AllNotConfirmedValuesByUser.length > 0) { this.start() }
    else { this.stop() }
  }
  removeValueFromConfirmValue(value: number, gameID: number) {
    var valuesToRemove = [];
    valuesToRemove.push(value);
    this.gameService.bulkRemoveValueFromGame(gameID, valuesToRemove).subscribe((resp: Response) => {
      if (this.AllNotConfirmedValuesByUser.length > 0) { this.start() }
      else { this.stop() }
      this._hubConnection.invoke('GameStart', gameID, this.currentUserID);
    });
  }
  private InsertSelectedColValue(objColDetailWithRows: any) {
    this.gameService.InsertSelectedColValue(objColDetailWithRows).subscribe((resp: Response) => {
      // if (this.AllNotConfirmedValuesByUser.length > 0) { this.start() }
      // else { this.stop() }
      this._hubConnection.invoke('GameStart', this.gameID, this.currentUserID);
    });
  }
  ConfirmValues() {

    this.gameService.confirmValue(this.gameID, this.AllNotConfirmedValuesByUser).subscribe((resp: Response) => {
      var tempArray = JSON.parse(JSON.stringify(this.AllNotConfirmedValuesByUser));
      for (var i = 0; i < tempArray.length; i++) {
        var index = tempArray.indexOf(tempArray[i]);
        if (index > -1) {
          this.AllConfirmedValues.push(tempArray[i]);
          this.AllConfirmedValuesByUser.push(tempArray[i]);
          this.AllNotConfirmedValuesByUser.splice(index, 1);
        }
      }
      // if (this.AllNotConfirmedValuesByUser.length > 0) { this.start() }
      // else { this.stop() }
      this._hubConnection.invoke('GameStart', this.gameID, this.currentUserID);
    });
  }
  ngOnDestroy() { this.clearTimer(); }

  clearTimer(): void { clearInterval(this.intervalId); }

  start(): void { this.countDown(); }
  stop(): void {
    this.seconds = 121;
    this.clearTimer();
  }
  ShowStartButton() {
    return !this.isGameStart && !this.isGameFinish && this.isAdmin;
  }
  ShowPauseButton() {
    return !this.isGamePause && !this.isGameFinish && this.isAdmin;
  }
  ShowEndButton() {
    return !this.isGameFinish && this.isAdmin;
  }
  private countDown(): void {
    this.clearTimer();
    this.intervalId = window.setInterval(() => {
      this.seconds -= 1;
      if (this.seconds === 0) {
        if (this.AllNotConfirmedValuesByUser.length > 0) {

          this.gameService.bulkRemoveValueFromGame(this.gameID, this.AllNotConfirmedValuesByUser).subscribe((resp: Response) => {
            this._hubConnection.invoke('GameStart', this.gameID, this.currentUserID);
            if (this.AllNotConfirmedValuesByUser.length === 0) { this.stop(); }
          });

        }

      } else {
        if (this.seconds < 0) { this.seconds = 120; } // reset
        this.message = `${this.seconds} seconds and counting`;
      }
    }, 1000);
  }
  typeSubmit() {

    if (this.typeValidationForm.valid) {
      this.typesubmit = false;

      var objGameInfo = {
        'gameID': this.gameID,
        'gameName': '',
        'winValue1': Number(this.typeValidationForm.get('winValue1').value),
        'winValue2': Number(this.typeValidationForm.get('winValue2').value),
        'winValue3': Number(this.typeValidationForm.get('winValue3').value),
        'winValue4': Number(this.typeValidationForm.get('winValue4').value),
        'winValue5': Number(this.typeValidationForm.get('winValue5').value),
        'winValue6': Number(this.typeValidationForm.get('winValue6').value),
        'isDeleted': false,
        'isGameStart': false,
        'isGamePause': false,
        'isGameFinish': false,
      };

      if (this.gameID === -1) {

        this.gameService.postGame(objGameInfo).subscribe(response => {

          this._hubConnection.invoke('GameStart', this.gameID, this.currentUserID);
        });
      } else {
        this.gameService.UpdateWinnigValues(objGameInfo).subscribe(response => {
          this._hubConnection.invoke('GameStart', this.gameID, this.currentUserID);
        });
      }
      this.modalService.dismissAll();

    } else {
      // validate all form fields
      this.typesubmit = true;

    }

  }

  /**
 * Returns the type validation form
 */
  get type() {
    return this.typeValidationForm.controls;
  }
  openModal(largeDataModal: any) {
    this.typesubmit = false;
    this.modalService.open(largeDataModal, {
      backdrop: 'static',
      size: 'xl',
      scrollable: true
    });
    if (this.gameData) {
      this.typeValidationForm.patchValue({

        winValue1: this.gameData.winValue1 == -1 ? "" : this.gameData.winValue1,
        winValue2: this.gameData.winValue2 == -1 ? "" : this.gameData.winValue2,
        winValue3: this.gameData.winValue3 == -1 ? "" : this.gameData.winValue3,
        winValue4: this.gameData.winValue4 == -1 ? "" : this.gameData.winValue4,
        winValue5: this.gameData.winValue5 == -1 ? "" : this.gameData.winValue5,
        winValue6: this.gameData.winValue6 == -1 ? "" : this.gameData.winValue6,
      });
    } else {

      this.typeValidationForm.patchValue({
        winValue1: "",
        winValue2: "",
        winValue3: "",
        winValue4: "",
        winValue5: "",
        winValue6: "",
      });
    }
  }

}