
function showSubmittedItems(phone) {
    var url = host + "admin/_SubmittedItems?phone=" + phone;
    url = encodeURI(url);
    $.ajax({
        url: url,
        cache: false,
        async: true
    }).done(function (html) {
        $('#submitted-items').html(html);
        $('#save-returned').prop("disabled", false);
    });
}

$(document).ready(function () {

    $('#salesman-phone').change(function(){
        showSubmittedItems($('#salesman-phone').val());
    });

    $('#salesman-phone').focus();
    
});