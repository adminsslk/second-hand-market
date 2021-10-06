

function PrintReceipt(phone) {
    var url = host + "admin/receipt?phone=" + phone;
    url = encodeURI(url);
    var mywindow = window.open(url, 'Kvitto', 'height=768,width=1024');
    mywindow.print();
    setTimeout(function () { mywindow.close() }, 1000);
    return true;
}



