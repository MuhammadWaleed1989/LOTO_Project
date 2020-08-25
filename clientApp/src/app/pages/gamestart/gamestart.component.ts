import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';

import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { first } from 'rxjs/operators';

import { Users } from '../usermanagement/users.model';

import { GameCompleteInfo, GameValues, GameDetailInfo, GameInfo } from '../games/games.model';
import { environment } from '../../../environments/environment';

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
  public selectedCellForWinner = [];
  public AllConfirmedValues = [];
  public AllNotConfirmedValues = [];
  public AllConfirmedValuesByUser = [];
  public AllNotConfirmedValuesByUser = [];
  currentUserID: number;
  public winnerName: string;
  users: Users[] = [];
  gameValues: GameValues[] = [];
  intervalId: number = 0;
  message: string = '';
  seconds: number = 121;
  constructor(private gameService: GameService, private modalService: NgbModal, public formBuilder: FormBuilder, private actRoute: ActivatedRoute) {
    this.typesubmit = false;
  }

  ngOnInit() {

    // setup the top lables
    this.breadCrumbItems = [{
      label: 'Skote'
    }, {
      label: 'Game Details',
      active: true
    }];
    this.winnerName = "";
    this.currentpage = 1;
    this.currentUserID = Number(localStorage.getItem('id'));
    this._hubConnection = new HubConnectionBuilder().withUrl(`${environment.apiUrl}/echo`).build();

    // this._hubConnection.on('GameAllValues', (data: any) => {
    //   this.AllConfirmedValues = [];
    //   this.AllNotConfirmedValues = [];
    //   this.AllNotConfirmedValuesByUser = [];
    //   this.AllConfirmedValuesByUser = [];
    //   if (data) {
    //     if (data.gameValues) {
    //       var gameValuesList = data.gameValues
    //       for (var i = 0; i < gameValuesList.length; i++) {
    //         if (gameValuesList[i]['isConfirmed'] === true) {
    //           this.AllConfirmedValues.push(gameValuesList[i]['value']);
    //         }
    //         if (gameValuesList[i]['isConfirmed'] === false) {
    //           this.AllNotConfirmedValues.push(gameValuesList[i]['value']);
    //         }
    //         if (gameValuesList[i]['isConfirmed'] === false && gameValuesList[i]['userID'] === this.currentUserID) {
    //           this.AllNotConfirmedValuesByUser.push(gameValuesList[i]['value']);
    //         }
    //         if (gameValuesList[i]['isConfirmed'] === true && gameValuesList[i]['userID'] === this.currentUserID) {
    //           this.AllConfirmedValuesByUser.push(gameValuesList[i]['value']);
    //         }
    //       }
    //     }
    //     if (data.gameWinner) {
    //       this.winnerName = data.gameWinner;
    //     }
    //   }
    // });
    // this._hubConnection.on('UserList', (data: any) => {
    //   this.users = data;
    // });

    // this._hubConnection.start()
    //   .then(() => {
    //     this.actRoute.paramMap.subscribe(params => {
    //       // this.winnerOfTheGame(Number(params.get('id')));
    //       this._hubConnection.invoke('GameStart', Number(params.get('id')));
    //       // this._hubConnection.invoke('Start');
    //     })
    //     console.log('Hub connection started')
    //   })
    //   .catch(err => {
    //     console.log('Error while establishing connection')
    //   });

    // this.actRoute.paramMap.subscribe(params => {
    //   this.getGameDetails(params.get('id'));
    // })
    this.getGameValues();
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
    //if (this.winnerName !== "") { return; }
    var columnNumber = Number(eve.currentTarget.innerText.replace("Col", "").trim());
    var allIndex = eve.currentTarget.innerText;
    eve.currentTarget.classList.add("current");
    var row1 = document.getElementById("td_0_Col " + columnNumber);
    var row2 = document.getElementById("td_1_Col " + columnNumber);
    var row3 = document.getElementById("td_2_Col " + columnNumber);
    var row4 = document.getElementById("td_3_Col " + columnNumber);
    var row5 = document.getElementById("td_4_Col " + columnNumber);
    var row6 = document.getElementById("td_5_Col " + columnNumber);
    row1.classList.add("borderRightAndLeft");
    row2.classList.add("borderRightAndLeft");
    row3.classList.add("borderRightAndLeft");
    row4.classList.add("borderRightAndLeft");
    row5.classList.add("borderRightAndLeft");
    row6.classList.add("borderRightAndLeft");
    // if (allIndex > -1) {
    //   return;
    // }

    // var notByUserindex = this.AllNotConfirmedValuesByUser.indexOf(Number(eve.currentTarget.innerText));
    // var allNotIndex = this.AllNotConfirmedValues.indexOf(Number(eve.currentTarget.innerText));

    // if (notByUserindex < 0 && allNotIndex < 0) {
    //   eve.currentTarget.classList.add("current");
    //   this.AllNotConfirmedValuesByUser.push(Number(eve.currentTarget.innerText));
    //   this.actRoute.paramMap.subscribe(params => {
    //     this.StartGame(Number(eve.currentTarget.innerText), Number(params.get('id')));
    //   })
    // }
    // else if (notByUserindex > -1) {
    //   eve.currentTarget.classList.remove("current");
    //   this.AllNotConfirmedValuesByUser.splice(notByUserindex, 1);
    //   this.actRoute.paramMap.subscribe(params => {
    //     this.removeValueFromConfirmValue(Number(eve.currentTarget.innerText), Number(params.get('id')));
    //   })
    // }
    // else {
    //   return;
    // }
    // if (this.AllNotConfirmedValuesByUser.length > 0) { this.start() }
    // else { this.stop() }
  }
  /*private getGameDetails(gameID: any) {
    this.gameService.getGameDetails(gameID).pipe(first()).subscribe(returnedData => {
      for (var i = 1; i <= 300; i++) {
        var col = 'col' + i;
        this.tableHeader.push(col);
      }
      var gameData = JSON.stringify(returnedData);
      const gameParsedJSON = JSON.parse(gameData);
      const gameCompleteInfoObj: GameCompleteInfo = gameParsedJSON as GameCompleteInfo;
      this.gameCompleteInfo = returnedData;


      //Start --  Get game info and store in array list
      var gameInfo = gameCompleteInfoObj.gameInfo;

      let arrayGameInfo = [];
      for (let key in gameInfo) {
        if (gameInfo.hasOwnProperty(key)) {
          arrayGameInfo.push(gameInfo[key]);
        }
      }

      this.selectedCellForWinner.push(gameInfo['winValue1']);
      this.selectedCellForWinner.push(gameInfo['winValue2']);
      this.selectedCellForWinner.push(gameInfo['winValue3']);
      this.selectedCellForWinner.push(gameInfo['winValue4']);
      this.selectedCellForWinner.push(gameInfo['winValue5']);
      this.selectedCellForWinner.push(gameInfo['winValue6']);
      //console.log(this.selectedCellForWinner)
      //End --  Get game info and store in array list


      //Start --  Get game details and convert columns to row
      var gameDetail = gameCompleteInfoObj.gameDetail;
      let arrayGameDetail = [];

      for (let key in gameDetail) {
        if (gameDetail.hasOwnProperty(key)) {
          arrayGameDetail.push(gameDetail[key]);
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
        jsonDataOfRow1['col' + k] = arrayGameDetail[j]['valueOfRow1'];
        jsonDataOfRow2['col' + k] = arrayGameDetail[j]['valueOfRow2'];
        jsonDataOfRow3['col' + k] = arrayGameDetail[j]['valueOfRow3'];
        jsonDataOfRow4['col' + k] = arrayGameDetail[j]['valueOfRow4'];
        jsonDataOfRow5['col' + k] = arrayGameDetail[j]['valueOfRow5'];
        jsonDataOfRow6['col' + k] = arrayGameDetail[j]['valueOfRow6'];
      }
      this.tableArray.push(jsonDataOfRow1);
      this.tableArray.push(jsonDataOfRow2);
      this.tableArray.push(jsonDataOfRow3);
      this.tableArray.push(jsonDataOfRow4);
      this.tableArray.push(jsonDataOfRow5);
      this.tableArray.push(jsonDataOfRow6);
      //End --  Get game details and convert columns to row

    });
  }
  clickedEvent(eve: any) {
    if (this.winnerName !== "") { return; }
    var allIndex = this.AllConfirmedValues.indexOf(Number(eve.currentTarget.innerText));
    if (allIndex > -1) {
      return;
    }

    var notByUserindex = this.AllNotConfirmedValuesByUser.indexOf(Number(eve.currentTarget.innerText));
    var allNotIndex = this.AllNotConfirmedValues.indexOf(Number(eve.currentTarget.innerText));

    if (notByUserindex < 0 && allNotIndex < 0) {
      eve.currentTarget.classList.add("current");
      this.AllNotConfirmedValuesByUser.push(Number(eve.currentTarget.innerText));
      this.actRoute.paramMap.subscribe(params => {
        this.StartGame(Number(eve.currentTarget.innerText), Number(params.get('id')));
      })
    }
    else if (notByUserindex > -1) {
      eve.currentTarget.classList.remove("current");
      this.AllNotConfirmedValuesByUser.splice(notByUserindex, 1);
      this.actRoute.paramMap.subscribe(params => {
        this.removeValueFromConfirmValue(Number(eve.currentTarget.innerText), Number(params.get('id')));
      })
    }
    else {
      return;
    }
    if (this.AllNotConfirmedValuesByUser.length > 0) { this.start() }
    else { this.stop() }
  }

  ConfirmValues() {
    this.actRoute.paramMap.subscribe(params => {
      this.gameService.confirmValue(Number(params.get('id')), this.AllNotConfirmedValuesByUser).subscribe((resp: Response) => {
        var tempArray = JSON.parse(JSON.stringify(this.AllNotConfirmedValuesByUser));
        //var tempArray = this.AllNotConfirmedValuesByUser;
        for (var i = 0; i < tempArray.length; i++) {
          var index = tempArray.indexOf(tempArray[i]);
          if (index > -1) {
            this.AllConfirmedValues.push(tempArray[i]);
            this.AllConfirmedValuesByUser.push(tempArray[i]);
            this.AllNotConfirmedValuesByUser.splice(index, 1);
          }
        }
        if (this.AllNotConfirmedValuesByUser.length > 0) { this.start() }
        else { this.stop() }
        this._hubConnection.invoke('GameStart', Number(params.get('id')));
        this.winnerOfTheGame(Number(params.get('id')));


      });
    })

  }
  removeValueFromConfirmValue(value: number, gameID: number) {
    this.gameService.removeValueFromConfirmValue(gameID, value).subscribe((resp: Response) => {
      if (this.AllNotConfirmedValuesByUser.length > 0) { this.start() }
      else { this.stop() }
      this._hubConnection.invoke('GameStart', gameID);
    });
  }
  StartGame(value: number, gameID: number) {
    this.gameService.StartGame(gameID, value, false).subscribe((resp: Response) => {
      if (this.AllNotConfirmedValuesByUser.length > 0) { this.start() }
      else { this.stop() }
      this._hubConnection.invoke('GameStart', gameID);
    });
  }

  winnerOfTheGame(gameID: number) {
    var isWinner = true;
    for (var i = 0; i < this.selectedCellForWinner.length; i++) {
      var index = this.AllConfirmedValuesByUser.indexOf(this.selectedCellForWinner[i]);
      if (index < 0) {
        isWinner = false;
      }
    }
    if (isWinner) {
      this.gameService.UpdateWinner(gameID).subscribe((resp: Response) => {
        if (this.AllNotConfirmedValuesByUser.length > 0) { this.start() }
        else { this.stop() }
        this._hubConnection.invoke('GameStart', gameID);
      });
    }

  }
  ngOnDestroy() { this.clearTimer(); }

  clearTimer(): void { clearInterval(this.intervalId); }

  start(): void { this.countDown(); }
  stop(): void {
    this.seconds = 121;
    this.clearTimer();
  }

  private countDown(): void {
    this.clearTimer();
    this.intervalId = window.setInterval(() => {
      this.seconds -= 1;
      if (this.seconds === 0) {
        if (this.AllNotConfirmedValuesByUser.length > 0) {
          this.actRoute.paramMap.subscribe(params => {
            this.gameService.bulkRemoveValueFromGame(Number(params.get('id')), this.AllNotConfirmedValuesByUser).subscribe((resp: Response) => {
              this._hubConnection.invoke('GameStart', Number(params.get('id')));
              if (this.AllNotConfirmedValuesByUser.length === 0) { this.stop(); }
            });
          })
        }

      } else {
        if (this.seconds < 0) { this.seconds = 120; } // reset
        this.message = `${this.seconds} seconds and counting`;
      }
    }, 1000);
  }*/
}