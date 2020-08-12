import { ChartType } from './echart.model';
import { graphic } from 'echarts';

const lineChart: ChartType = {
    xAxis: {
        type: 'category',
        data: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'],
        axisLabel: {
            textStyle: {
                color: '#999'
            }
        },
        axisTick: {
            show: false
        },
        axisLine: {
            show: false
        },
    },
    yAxis: {
        type: 'value',
        axisLine: {
            show: false
        },
        axisTick: {
            show: false
        },
        axisLabel: {
            textStyle: {
                color: '#999'
            }
        }
    },
    series: [{
        data: [120, 132, 101, 134, 90, 230, 210],
        type: 'line'
    },
    {
        data: [420, 500, 611, 454, 540, 450, 330],
        type: 'line'
    },
    {
        data: [820, 932, 901, 934, 1290, 1330, 1320],
        type: 'line'
    }],
    color: ['#556ee6', '#34c38f', '#f1b44c']
};

const barChart: ChartType = {
    color: ['#50a5f1'],
    tooltip: {
        trigger: 'axis',
        axisPointer: {
            type: 'shadow'
        }
    },
    grid: {
        left: '3%',
        right: '4%',
        bottom: '3%',
        containLabel: true
    },
    xAxis: [
        {
            type: 'category',
            data: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'],
            axisTick: {
                show: false
            },
            axisLabel: {
                textStyle: {
                    color: '#999'
                }
            },
            axisLine: {
                show: false
            },
        },
    ],
    yAxis: [{
        type: 'value',
        axisTick: {
            show: false
        },
        axisLine: {
            show: false
        },
        axisLabel: {
            textStyle: {
                color: '#999'
            }
        }
    }],
    series: [{
        name: 'Counters',
        type: 'bar',
        barWidth: '60%',
        data: [10, 52, 200, 334, 390, 330, 220]
    }]
};


const pieChart: ChartType = {
    series: [{
        data: [
            { value: 45, name: 'Crome' },
            { value: 20, name: 'IE' }, { value: 17, name: 'Firefox' }, { value: 5, name: 'Safari' }, { value: 10, name: 'Etc' }],
        type: 'pie',
    }],
    color: ['#556ee6', '#34c38f', '#f1b44c', '#50a5f1', '#f46a6a'],
    legend: {
        x: 'center',
        y: 'bottom',
        data: ['Crome', 'IE', 'Firefox', 'Safari', 'Etc'],
    },
};

const customPieChart: ChartType = {
    series: [{
        data: [
            { value: 25, name: 'Crome' },
            { value: 10, name: 'IE' }, { value: 15, name: 'Firefox' }, { value: 30, name: 'Safari' }, { value: 10, name: 'Etc' }],
        type: 'pie',
        roseType: 'radius',
    }],
    color: ['#556ee6', '#34c38f', '#f1b44c', '#50a5f1', '#f46a6a'],
    legend: {
        x: 'center',
        y: 'bottom',
        data: ['Crome', 'IE', 'Firefox', 'Safari', 'Etc']
    },
};

const dataAxis = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T'];
const data = [220, 182, 191, 234, 290, 330, 310, 123, 442, 321, 90, 149, 210, 122, 133, 334, 198, 123, 125, 220];
const dataShadow = [];
const yMax = 500;

// tslint:disable-next-line: prefer-for-of
for (let i = 0; i < data.length; i++) {
    dataShadow.push(yMax);
}
const gradientBarChart: ChartType = {
    xAxis: {
        data: dataAxis,
        axisLabel: {
            inside: true,
            textStyle: {
                color: '#fff'
            }
        },
        axisTick: {
            show: false
        },
        axisLine: {
            show: false
        },
        z: 10
    },
    yAxis: {
        axisLine: {
            show: false
        },
        axisTick: {
            show: false
        },
        axisLabel: {
            textStyle: {
                color: '#999'
            }
        }
    },
    dataZoom: [
        {
            type: 'inside'
        }
    ],
    series: [
        { // For shadow
            type: 'bar',
            itemStyle: {
                normal: { color: 'rgba(0,0,0,0.05)' }
            },
            barGap: '-100%',
            barCategoryGap: '40%',
            data: dataShadow,
            animation: false
        },
        {
            type: 'bar',
            itemStyle: {
                normal: {
                    color: new graphic.LinearGradient(
                        0, 0, 0, 1,
                        [
                            { offset: 0, color: '#83bff6' },
                            { offset: 0.5, color: '#188df0' },
                            { offset: 1, color: '#188df0' }
                        ]
                    )
                },
                emphasis: {
                    color: new graphic.LinearGradient(
                        0, 0, 0, 1,
                        [
                            { offset: 0, color: '#2378f7' },
                            { offset: 0.7, color: '#2378f7' },
                            { offset: 1, color: '#83bff6' }
                        ]
                    )
                }
            },
            data
        }
    ]

};

export { lineChart, barChart, pieChart, customPieChart, gradientBarChart };
