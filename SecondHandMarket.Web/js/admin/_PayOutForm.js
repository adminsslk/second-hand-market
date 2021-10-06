
function showSoldItems(phone) {
    var url = host + "admin/_SoldItems?phone=" + phone;
    url = encodeURI(url);
    $.ajax({
        url: url,
        cache: false,
        async: true
    }).done(function (html) {
        $('#sold-items').html(html);
        $('#save-payed-out').prop("disabled", false);
    });
}

$(document).ready(function () {

    $('#salesman-phone').change(function(){
        showSoldItems($('#salesman-phone').val());
    });

    $('#salesman-phone').focus();

    if ($('#salesman-phone').val().length > 0) {
        showSoldItems($('#salesman-phone').val());
    }
    
});