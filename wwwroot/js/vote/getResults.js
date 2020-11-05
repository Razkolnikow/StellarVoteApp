var ctx = document.getElementById('myChart').getContext('2d');
$.ajax({
    method: "GET",
    url: '/Vote/GetResults',
    contentType: 'application/json',
    success: function (data) {
        $("#spinner").hide();
        var names = data.map(x => x.name);
        var votes = data.map(x => x.votes);

        var sumVotes = votes.reduce((a, b) => a + b, 0);
        var votesPercentages = votes.map(i => Number((i / sumVotes) * 100).toFixed(2));

        var resultsDiv = $("#results");
        for (let i = 0; i < names.length; i++) {
            let name = names[i];
            let vote = votes[i];
            let percentage = votesPercentages[i];
            resultsDiv.append(`<h4>Candidate: <strong>${name}</strong> --- Votes: <strong>${vote}</strong> --- Percentage: <strong>${percentage}</strong></h4>`)
        }
        
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

        var ctxPie = document.getElementById("pieChart").getContext('2d');
        var myChart = new Chart(ctxPie, {
            type: 'pie',
            data: {
                datasets: [{
                    data: votesPercentages,
                    backgroundColor: [
                        "rgba(0, 123, 255,0.9)",
                        "rgba(0, 123, 255,0.7)",
                        "rgba(0, 123, 255,0.5)",
                        "rgba(0,0,0,0.07)"
                    ],
                    hoverBackgroundColor: [
                        "rgba(0, 123, 255,0.9)",
                        "rgba(0, 123, 255,0.7)",
                        "rgba(0, 123, 255,0.5)",
                        "rgba(0,0,0,0.07)"
                    ]

                }],
                labels: names
            },
            options: {
                responsive: true
            }
        });
    },
    error: function (err) {
        console.log(err);
    }
});