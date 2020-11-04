﻿var ctx = document.getElementById('myChart').getContext('2d');
$.ajax({
    method: "GET",
    url: '/Vote/GetResults',
    contentType: 'application/json',
    success: function (data) {
        $("#spinner").hide();
        var names = data.map(x => x.name);
        var votes = data.map(x => x.votes);
        var chart = new Chart(ctx, {
            type: 'bar',

            data: {
                labels: names,
                datasets: [{
                    label: 'Election Results',
                    backgroundColor: "rgba(0, 123, 255, 0.5)",
                    borderColor: "rgba(0, 123, 255, 0.9)",
                    data: votes
                }]
            },
            options: {
                scales: {
                    yAxes: [{
                        ticks: {
                            beginAtZero: true
                        }
                    }]
                }
            }
        });
    },
    error: function (err) {
        console.log(err);
    }
});