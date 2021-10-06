var selectedPhone;

function PrintLabels(phone) {
    var url = host + "admin/labels?phone=" + phone;
    url = encodeURI(url);
    $.ajax({
        url: url,
        cache: false
    }).done(function (file) {
        printPdf(host + file);
    });
}

function printPdf(url) {
    var iframe = this._printIframe;
    if (!this._printIframe) {
        iframe = this._printIframe = document.createElement('iframe');
        document.body.appendChild(iframe);
        iframe.style.display = 'none';
        iframe.onload = function () {
            setTimeout(function () {
                iframe.focus();
                iframe.contentWindow.print();
            }, 1);
        };
    }

    iframe.src = url;
}

function updateNumberOfLabels(e) {
    var numberOfLabels = $(e).val();
    var itemId = $(e).data('item-id');

    $.ajax({
        url: host + "admin/UpdateNumberOfLabels",
        type: "POST",
        data: JSON.stringify({
            itemId: itemId,
            numberOfLabels: numberOfLabels
        }),
        contentType: "application/json; charset=utf-8",
        cache: false,
        async: false
    });
}

function showPrintOutForm(phone) {
    var url = host + "admin/_PrintOutForm?phone=" + phone;
    url = encodeURI(url);
    $.ajax({
        url: url,
        cache: false,
        async: true
    }).done(function (html) {
        $('#item-dialog').html(html);
        $('#item-dialog').modal('show');
    });
}

function showPayOutForm(phone) {
    var url = host + "admin/_PayOutForm?phone=" + phone;
    url = encodeURI(url);
    $.ajax({
        url: url,
        cache: false,
        async: true
    }).done(function (html) {
        selectedPhone = phone;
        $('#item-dialog').html(html);
        $('#item-dialog').modal('show');
        $('#modal-salesman-phone').focus();
    });
}

function updateItemStatus(id, statusId) {
    var url = host + "admin/updateItemStatus?id=" + id + "&statusId=" + statusId;
    url = encodeURI(url);
    $.ajax({
        url: url,
        cache: false,
        async: false
    }).done(function (html) {
        updateSalemanRow();
    });
}

function updateSalemanRow() {
    var url = host + "admin/_SalesmanRow?phone=" + selectedPhone + "&year=" + $('#selected-year').val();
    url = encodeURI(url);
    $.ajax({
        url: url,
        cache: false,
        async: true
    }).done(function (html) {
        $('#' + selectedPhone).replaceWith(html);
    });
}

function submitForm(pageIndex) {
    $('#page-index').val(pageIndex);
    $('#search-form').submit();
}s