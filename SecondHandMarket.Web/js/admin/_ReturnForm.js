
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
        $('#return-item-id').focus();
    });
}

function checkReturnedItem() {
    var input = $('#return-item-id');
    var id = $.trim(input.val());
    if (id === '') {
        return;
    }

    var checkbox = $('.item-input').filter(function () {
        return /^\d+$/.test(id) && parseInt($(this).data('id'), 10) === parseInt(id, 10);
    });

    if (checkbox.length === 0) {
        $('#return-item-error')
            .text('Varunummer ' + id + ' finns inte bland säljarens inlämnade varor! Kontrollera att det är rätt vara som återlämnas.')
            .removeClass('hidden');
        input.select();
        return;
    }

    checkbox.prop('checked', true);
    $('#return-item-error').addClass('hidden');
    $('#error-message').addClass('hidden');
    input.val('').focus();
}

$(document).ready(function () {

    $('#salesman-phone').change(function(){
        showSubmittedItems($('#salesman-phone').val());
    });

    $('#submitted-items').on('keydown', '#return-item-id', function (e) {
        if (e.which === 13) {
            e.preventDefault();
            checkReturnedItem();
        }
    });

    $('#salesman-phone').focus();
    
});