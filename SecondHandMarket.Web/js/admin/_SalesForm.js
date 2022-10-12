var itemIds = [];

$('#save-button').click(function () {
    for (i = 0; i < itemIds.length; i++) {
        var url = host + "admin/updateItemStatus?id=" + itemIds[i]  + "&statusId=" + 3;
        url = encodeURI(url);
        $.ajax({
            method: 'GET',
            url: url,
            cache: false,
            async: false
        });
    }

    $('.modal').modal('toggle');
});


$('#add-item').click(function () {
    if ($('#item').val() != '') {
        itemId = Number($('#item').val().split(" | ")[0]);
        if (contains(itemIds, itemId) == false) {
            itemIds.push(itemId);
            $('#save-button').prop("disabled", false);
        }

        loadBody();
    }
});

$('#item').keypress(function (e) {
    if (e.key === 'Enter') {
        if ($('#item').val() != '') {
            itemId = Number($('#item').val().split(" | ")[0]);
            if (contains(itemIds, itemId) == false) {
                itemIds.push(itemId);
                $('#save-button').prop("disabled", false);
            }

            loadBody();
        }
    }
});

function removeItem(itemId) {
    for (i = 0; i < itemIds.length; i++){
        if (itemIds[i] == itemId) {
            itemIds.splice(i, 1);
            loadBody();
            if (itemIds.length == 0) {
                $('#save-button').prop("disabled", true);
            }
            return;
        }
    }
}

function loadBody() {

    $.ajax({
        method: 'POST',
        url: host + 'admin/_SalesFormBody',
        data: {
            itemIds: itemIds
        }
    }).done(function (html) {
        $('#body').html(html);
        $('#item').val('');
        $('#item').focus();
    });
}

function contains(array, obj) {
    var i = array.length;
    while (i--) {
        if (array[i] == obj) {
            return true;
        }
    }
    return false;
}