import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';

import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { first } from 'rxjs/operators';

import { GameCompleteInfo, GameDetailInfo, GameInfo } from '../games/games.model';
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

    this.currentpage = 1;
    this.currentUserID = Number(localStorage.getItem('id'));
    this._hubConnection = new HubConnectionBuilder().withUrl(`${environment.apiUrl}/echo`).build();

    this._hubConnection.on('GameAllValues', (data: any) => {
      this.AllConfirmedValues = [];
      if (data) {
        for (var i = 0; i < data.length; i++) {
          if (data[i]['isConfirmed'] === true) {
            this.AllConfirmedValues.push(data[i]['value']);
          }
          if (data[i]['isConfirmed'] === false) {
            this.AllNotConfirmedValues.push(data[i]['value']);
          }
          if (data[i]['isConfirmed'] === false && data[i]['userID'] === this.currentUserID) {
            this.AllNotConfirmedValuesByUser.push(data[i]['value']);
          }
          if (data[i]['isConfirmed'] === true && data[i]['userID'] === this.currentUserID) {
            this.AllConfirmedValuesByUser.push(data[i]['value']);
          }

        }
      }
    });
    // this._hubConnection.on('GetNotConfirmedValue', (data: any) => {
    //   this.AllNotConfirmedValuesByUser = [];
    //   if (data) {
    //     for (var i = 0; i < data.length; i++) {
    //       this.AllNotConfirmedValuesByUser.push(data[i]);
    //     }
    //   }
    // });

    this._hubConnection.start()
      .then(() => {
        this.actRoute.paramMap.subscribe(params => {
          this._hubConnection.invoke('GameStart', Number(params.get('id')));
          //this._hubConnection.invoke('StartGetNotConfirmedValue', Number(params.get('id')), this.currentUserID);
        })
        console.log('Hub connection started')
      })
      .catch(err => {
        console.log('Error while establishing connection')
      });

    this.actRoute.paramMap.subscribe(params => {
      this.getGameDetails(params.get('id'));
    })
  }


  private getGameDetails(gameID: any) {
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


    const index = this.AllNotConfirmedValuesByUser.indexOf(eve.currentTarget.innerText);
    if (index > -1) {
      eve.currentTarget.classList.remove("current");
      this.AllNotConfirmedValuesByUser.splice(index, 1);
    }
    else {
      eve.currentTarget.classList.add("current");
      this.AllNotConfirmedValuesByUser.push(eve.currentTarget.innerText);
    }
    this.actRoute.paramMap.subscribe(params => {
      this.StartGame(Number(eve.currentTarget.innerText), Number(params.get('id')));
    })


  }

  ConfirmValues() {
    this.actRoute.paramMap.subscribe(params => {
      this.gameService.ConfirmValue(Number(params.get('id')), this.AllNotConfirmedValuesByUser).subscribe((resp: Response) => {
        for (var i = 0; i < this.AllNotConfirmedValuesByUser.length; i++) {
          const index: number = this.AllNotConfirmedValuesByUser.indexOf(this.AllNotConfirmedValuesByUser[i]);
          if (index !== -1) {
            this.AllNotConfirmedValuesByUser.splice(index, 1);
            this.AllConfirmedValues.push(this.AllNotConfirmedValuesByUser[i]);
          }
        }
        this._hubConnection.invoke('GameStart', Number(params.get('id')));
      });
    })

  }

  StartGame(value: number, gameID: number) {
    this.gameService.StartGame(gameID, value, false).subscribe((resp: Response) => {
      this._hubConnection.invoke('GameStart', gameID);
    });
  }

  ReturnNotConfirmedValues() {

  }

}