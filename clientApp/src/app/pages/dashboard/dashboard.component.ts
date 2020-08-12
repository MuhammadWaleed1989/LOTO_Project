import { Component, OnInit } from '@angular/core';

import { emailSentBarChart, monthlyEarningChart, transactions, statData } from './data';

import { ChartType } from './dashboard.model';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})

/**
 * Dashboard component
 */
export class DashboardComponent implements OnInit {

  emailSentBarChart: ChartType;
  monthlyEarningChart: ChartType;
  transactions;
  statData;

  constructor() { }

  ngOnInit() {

    /**
     * Fetches the data
     */
    this.fetchData();
  }

  /**
   * Fetches the data
   */
  private fetchData() {
    this.emailSentBarChart = emailSentBarChart;
    this.monthlyEarningChart = monthlyEarningChart;
    this.transactions = transactions;
    this.statData = statData;
  }

}
