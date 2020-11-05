var createBtn = $('#create');
var idnumber = $("#idnumber");
var cardnumber = $("#cardnumber");
var spinner = $("#spinner");
spinner.hide();
var form = $("#form");
var error = $("#error");
error.hide();
var created = $("#created");
created.hide();

createBtn.on('click', function (ev) {
    var jsonObj = {
        IdNumber: idnumber.val(),
        CardNumber: cardnumber.val()
    }
    form.hide();
    spinner.show();
    $.ajax({
        method: "POST",
        url: "/Vote/CreateAccount",
        data: JSON.stringify(jsonObj),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            spinner.hide();
            
            var accountId = data.accountId;
            var voteTokenBalance = data.voteTokenBalance;
            if (accountId && accountId.length > 0) {
                created.show();
                $("#account-details").html(`Your accound id is <strong>${accountId}</strong> and you have <strong>${voteTokenBalance}</strong> Vote token.`);
            } else {
                $("#invalidtext").html(data);
                $("#invalid").append('<div><button class="btn-default btn-info" onClick="window.location.href=window.location.href">Try again!</button></div>');
            }            
        },
        error: function (err) {
            spinner.hide();
            error.show();
            console.log(err);
        }
    });
})