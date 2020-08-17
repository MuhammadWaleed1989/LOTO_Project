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
      this.totalPages = games.length;
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
        // ,
        // cell1Value: info.cell1Value,
        // cell2Value: info.cell2Value,
        // cell3Value: info.cell3Value,
        // cell4Value: info.cell4Value
      });
    } else {
      this.typeValidationForm.patchValue({
        gameID: "-1",
        gameName: ""
        // ,
        // cell1Value: "",
        // cell2Value: "",
        // cell3Value: "",
        // cell4Value: "",
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


      var object = {
        'gameID': Number(this.typeValidationForm.get('gameID').value),
        'gameName': this.typeValidationForm.get('gameName').value,
        // 'cell1Value': Number(this.typeValidationForm.get('cell1Value').value),
        // 'cell2Value': Number(this.typeValidationForm.get('cell2Value').value),
        // 'cell3Value': Number(this.typeValidationForm.get('cell3Value').value),
        // 'cell4Value': Number(this.typeValidationForm.get('cell4Value').value),
        'IsDeleted': false
      };

      if (this.typeValidationForm.get('gameID').value === "-1") {

        this.gameService.postGame(object).subscribe(response => {
          this.statusMessage = 'Game is created Successfully.'
          this.loadAllGames();
        }, (error) => {
          console.log('error during post is ', error)
        });
      } else {
        this.gameService.updateGame(object, this.typeValidationForm.get('gameID').value).subscribe(response => {
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