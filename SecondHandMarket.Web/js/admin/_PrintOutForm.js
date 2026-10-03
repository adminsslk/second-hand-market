

function PrintReceipt(phone) {
    var url = host + "admin/receipt?phone=" + phone;
    url = encodeURI(url);
    var mywindow = window.open(url, 'Kvitto', 'height=768,width=1024');
    mywindow.print();
    setTimeout(function () { mywindow.close() }, 1000);
    return true;
}

function ClearNumberOfLabels() {
    $('#items-body input[type=number]').each(function () {
        $(this).val(0);
        updateNumberOfLabels(this);
    });
    updatePrintLabelsButton();
}

function updatePrintLabelsButton() {
    var hasLabels = $('#items-body input[type=number]').filter(function () {
        return parseInt($(this).val(), 10) > 0;
    }).length > 0;
    $('#print-labels').prop('disabled', !hasLabels);
}

$('#items-body input[type=number]').on('input change', updatePrintLabelsButton);



