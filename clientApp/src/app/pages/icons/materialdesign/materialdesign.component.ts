import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-materialdesign',
  templateUrl: './materialdesign.component.html',
  styleUrls: ['./materialdesign.component.scss']
})

/**
 * Material design component
 */
export class MaterialdesignComponent implements OnInit {
  // bread crumb items
  breadCrumbItems: Array<{}>;

  constructor() { }

  ngOnInit() {
    this.breadCrumbItems = [{ label: 'Icons' }, { label: 'MDI', active: true }];
  }

}
