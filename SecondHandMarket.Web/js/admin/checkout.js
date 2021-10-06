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

function printPdf (url) {
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

function showSalesForm() {
    var url = host + "admin/_SalesForm";
    url = encodeURI(url);
    $.ajax({
        url: url,
        cache: false,
        async: true
    }).done(function (html) {
        $('#item-dialog').html(html);
        $('#item-dialog').modal('show');
        $('#sales-form-item-id').focus();
    });
}

function showRegisterSalesmanForm() {
    var url = host + "admin/_RegisterSalesmanForm";
    url = encodeURI(url);
    $.ajax({
        url: url,
        cache: false,
        async: true
    }).done(function (html) {
        $('#item-dialog').html(html);
        $('#item-dialog').modal('show');
        $('#salesman-phone').focus();
    });
}

function showSubmissionForm() {
    var url = host + "admin/_SubmissionForm";
    url = encodeURI(url);
    $.ajax({
        url: url,
        cache: false,
        async: true
    }).done(function (html) {
        $('#item-dialog').html(html);
        $('#item-dialog').modal('show');
        $('#modal-salesman-phone').focus();
    });
}

function showPayOutForm() {
    var url = host + "admin/_PayOutForm";
    url = encodeURI(url);
    $.ajax({
        url: url,
        cache: false,
        async: true
    }).done(function (html) {
        $('#item-dialog').html(html);
        $('#item-dialog').modal('show');
        $('#modal-salesman-phone').focus();
    });
}

function showReturnForm() {
    var url = host + "admin/_ReturnForm";
    url = encodeURI(url);
    $.ajax({
        url: url,
        cache: false,
        async: true
    }).done(function (html) {
        $('#item-dialog').html(html);
        $('#item-dialog').modal('show');
        $('#modal-salesman-phone').focus();
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

function updateItemStatus(id, statusId) {
    var url = host + "admin/updateItemStatus?id=" + id + "&statusId=" + statusId;
    url = encodeURI(url);
    $.ajax({
        url: url,
        cache: false,
        async: false
    }).done(function (html) {
        updateItemRow(id);
    });
}

function updateItemRow(id) {
    var url = host + "admin/_ItemRow?id=" + id;
    url = encodeURI(url);
    $.ajax({
        url: url,
        cache: false,
        async: true
    }).done(function (html) {
        $('#tr-' + id).replaceWith(html);
    });
}

$(document).ready(function () {

    $('[data-toggle="tooltip"]').tooltip();

    $('.new-salesman-button').click(function () {
        showRegisterSalesmanForm();
    });

    $('.sales-button').click(function () {
        showSalesForm();
    });
    
    $('.submission-button').click(function () {
        showSubmissionForm();
    });

    $('.pay-out-button').click(function () {
        showPayOutForm();
    });

    $('.return-button').click(function () {
        showReturnForm();
    });

});