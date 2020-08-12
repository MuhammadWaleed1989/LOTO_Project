import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-fontawesome',
  templateUrl: './fontawesome.component.html',
  styleUrls: ['./fontawesome.component.scss']
})

/**
 * Font awesome component
 */
export class FontawesomeComponent implements OnInit {
  // bread crumb items
  breadCrumbItems: Array<{}>;

  constructor() { }

  ngOnInit() {
    this.breadCrumbItems = [{ label: 'Icons' }, { label: 'Font awesome', active: true }];
  }
}
