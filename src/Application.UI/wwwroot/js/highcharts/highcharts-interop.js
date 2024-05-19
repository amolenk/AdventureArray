
function RenderChart(renderTo, chartOptions) {

    console.log('RenderChart');

    Highcharts.setOptions({
        credits: {
            enabled: false
        }
    });

    try {

        const chart = Highcharts.chart(renderTo, chartOptions);
    }
    catch (e) {
        console.log(e);
    }
}