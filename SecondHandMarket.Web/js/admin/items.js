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

function PrintLabel(phone, itemId) {
    var url = host + "admin/label?phone=" + phone + "&itemId=" + itemId;
    url = encodeURI(url);
    $.ajax({
        url: url,
        cache: false
    }).done(function (file) {
        printPdf(host + file);
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

function showEditItemForm(id) {
    $('#item-dialog').data('action', 'nothing');
    var url = host + "admin/_edititemform?id=" + id;
    url = encodeURI(url);
    $.ajax({
        url: url,
        cache: false,
        async: true
    }).done(function (html) {
        $('#item-dialog').html(html);
        $('#item-dialog').modal('show');
        $("#item-status-id").focus();
    });
}

function showDeleteForm(id) {
    var url = host + "admin/_deleteitemform?id=" + id;
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

function submitForm(pageIndex) {
    $('#page-index').val(pageIndex);
    $('#search-form').submit();
}

$(document).ready(function () {

    if (isTouchDevice == false) {
        $("#search-item-phone").focus();
        $("#search-item-phone")[0].setSelectionRange($("#search-item-phone").val().length, $("#search-item-phone").val().length);
    }

    $('[data-toggle="tooltip"]').tooltip()

     $("#search-item-id").keydown(function (e) {
        //Do search if enter
         if (e.keyCode == 13) {
             reloadPage($('#search-item-phone').val(), $('#search-item-id').val(), $('#search-item-status').val(), $('#page-index').val(), $('#page-size').val(), $('#search-item-year').val());
        }
        // Allow: backspace, delete, tab, escape and .
        if ($.inArray(e.keyCode, [46, 8, 9, 27, 110, 190]) !== -1 ||
             // Allow: Ctrl+A
            (e.keyCode == 65 && e.ctrlKey === true) || 
             // Allow: home, end, left, right
            (e.keyCode >= 35 && e.keyCode <= 39)) {
                 // let it happen, don't do anything
                 return;
        }
        // Ensure that it is a number and stop the keypress
        if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
            e.preventDefault();
        }
     });
    
     $("#search-item-phone").keydown(function (e) {
         //Do search if enter
         if (e.keyCode == 13) {
             reloadPage($('#search-item-phone').val(), $('#search-item-id').val(), $('#search-item-status').val(), $('#page-index').val(), $('#page-size').val(), $('#search-item-year').val());
         }
     });
});