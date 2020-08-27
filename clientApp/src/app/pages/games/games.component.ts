import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';

import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { first } from 'rxjs/operators';

import { GameService } from '../../../app/services/game.service';
import { GameInfo, UserGame } from './games.model';


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
  public isAdmin: boolean = false;
  public currentUserID: number;
  constructor(private gameService: GameService, private modalService: NgbModal, public formBuilder: FormBuilder) {
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
    // setup the top lables
    this.breadCrumbItems = [{
      label: 'LOTO Game'
    }, {
      label: 'Games',
      active: true
    }];

    this.currentpage = 1;
    /**
     * Fetches the games data
     */
    this.loadAllGames();
    this.isAdmin = this.getBoolean(localStorage.getItem('isAdmin'));
    this.currentUserID = Number(localStorage.getItem('id'));
    /**
     * Type validation form
     */
    this.typeValidationForm = this.formBuilder.group({
      gameID: ['-1'],
      gameName: ['', [Validators.required]],
      // winValue1: ['', [Validators.required, Validators.pattern('[0-9]+')]],
      // winValue2: ['', [Validators.required, Validators.pattern('[0-9]+')]],
      // winValue3: ['', [Validators.required, Validators.pattern('[0-9]+')]],
      // winValue4: ['', [Validators.required, Validators.pattern('[0-9]+')]],
      // winValue5: ['', [Validators.required, Validators.pattern('[0-9]+')]],
      // winValue6: ['', [Validators.required, Validators.pattern('[0-9]+')]],
    });
  }

  /**
   * Load All Games
   */
  private loadAllGames() {
    this.gameService.getAll().pipe(first()).subscribe(games => {
      if (games) {
        this.gameInfo = games;
        this.totalPages = games.length;
      }
      else {
        this.gameInfo = [];
        this.totalPages = 0;
      }

    });
  }

  deleteGame(info: GameInfo) {
    this.gameService.deleteGame(info.gameID).subscribe((resp: Response) => {
      this.statusMessage = 'Record Deleted Successfully.',
        this.loadAllGames();
    });
  }
  InsertUserOfGame(gameID: any) {
    var obj = {
      gameID: gameID,
      GameUserID: -1,
      isDeleted: false,
      userID: this.currentUserID

    };
    this.gameService.InsertUserOfGame(obj).subscribe((resp: Response) => { });
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
        gameName: info.gameName,
        // winValue1: info.winValue1,
        // winValue2: info.winValue2,
        // winValue3: info.winValue3,
        // winValue4: info.winValue4,
        // winValue5: info.winValue5,
        // winValue6: info.winValue6,
      });
    } else {

      this.typeValidationForm.patchValue({
        gameID: "-1",
        gameName: "",
        // winValue1: "",
        // winValue2: "",
        // winValue3: "",
        // winValue4: "",
        // winValue5: "",
        // winValue6: "",
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

    if (this.typeValidationForm.valid) {
      this.typesubmit = false;

      var objGameInfo = {
        'gameID': Number(this.typeValidationForm.get('gameID').value),
        'gameName': this.typeValidationForm.get('gameName').value,
        // 'winValue1': Number(this.typeValidationForm.get('winValue1').value),
        // 'winValue2': Number(this.typeValidationForm.get('winValue2').value),
        // 'winValue3': Number(this.typeValidationForm.get('winValue3').value),
        // 'winValue4': Number(this.typeValidationForm.get('winValue4').value),
        // 'winValue5': Number(this.typeValidationForm.get('winValue5').value),
        // 'winValue6': Number(this.typeValidationForm.get('winValue6').value),
        'winValue1': -1,
        'winValue2': -1,
        'winValue3': -1,
        'winValue4': -1,
        'winValue5': -1,
        'winValue6': -1,
        'isDeleted': false,
        'isGameStart': false,
        'isGamePause': false,
        'isGameFinish': false,
      };

      if (this.typeValidationForm.get('gameID').value === "-1") {

        this.gameService.postGame(objGameInfo).subscribe(response => {
          this.statusMessage = 'Game is created Successfully.'
          this.loadAllGames();
        }, (error) => {
          console.log('error during post is ', error)
        });
      } else {
        this.gameService.updateGame(objGameInfo, this.typeValidationForm.get('gameID').value).subscribe(response => {
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
      this.greaterThan();
    }

  }
  greaterThan() {
    return this.selectedCellForWinner.length < 6 || this.selectedCellForWinner.length > 6;
  }
  reset() {
    this.typeValidationForm.reset();
    this.typesubmit = false;
  }
}