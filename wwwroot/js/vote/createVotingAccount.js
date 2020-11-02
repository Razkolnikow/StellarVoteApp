var createBtn = $('#create');
var idnumber = $("#idnumber");
var cardnumber = $("#cardnumber");

createBtn.on('click', function (ev) {
    var jsonObj = {
        idnumber: idnumber,
        cardnumber: cardnumber
    }
    $.ajax({
        method: "POST",
        url: "/Vote/CreateAccount",
        data: JSON.stringify(jsonObj),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            console.log(data);
        },
        error: function (err) {
            console.log(err);
        }
    });
})