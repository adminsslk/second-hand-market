function PrintReceipt(phone) {
    //PRINT OUT LABELS
    var url = host + "admin/receipt?phone=" + phone;
    url = encodeURI(url);
    var mywindow = window.open(url, 'Kvitto', 'height=768,width=1024');
    mywindow.print();
    setTimeout(function () { mywindow.close() }, 1000);
    $('#salesman-phone').focus();
    return true;
}

function showRegisteredItems(phone, showSubmitted) {
    var url = host + "admin/_RegisteredItems?phone=" + phone + "&showSubmitted=" + showSubmitted;
    url = encodeURI(url);
    $.ajax({
        url: url,
        cache: false,
        async: true
    }).done(function (html) {
        $('#registerd-items').html(html);
        $('#save-submitted').prop("disabled", false);
    });
}

$(document).ready(function () {

    $('#salesman-phone').change(function(){
        showRegisteredItems($('#salesman-phone').val(), false);
    });

    $('#salesman-phone').focus();
    
});