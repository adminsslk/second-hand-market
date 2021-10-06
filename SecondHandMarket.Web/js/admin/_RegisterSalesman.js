function removeItem(e) {
    $(e).closest("tr").remove();
    $('#item-description').focus();
}

$('#add-item').click(function () {

    $('#error-message').text('');
    $('#error-message').addClass('hidden');

    //Validation
    if ($('#salesman-first-name').val() == '') {
        $('#error-message').removeClass('hidden');
        $('#error-message').text('Du måste fylla i ett förnamn');
        $('#salesman-first-name').focus();
        return;
    }

    if ($('#salesman-last-name').val() == '') {
        $('#error-message').removeClass('hidden');
        $('#error-message').text('Du måste fylla i ett efternamn');
        $('#salesman-last-name').focus();
        return;
    }

    if ($('#salesman-phone').val() == '') {
        $('#error-message').removeClass('hidden');
        $('#error-message').text('Du måste fylla i ett mobilnummer');
        $('#salesman-phone').focus();
        return;
    }

    if ($('#item-description').val() == '') {
        $('#error-message').removeClass('hidden');
        $('#error-message').text('Du måste fylla i en beskrivning');
        $('#item-description').focus();
        return;
    }

    if ($('#item-price').val() == '') {
        $('#error-message').removeClass('hidden');
        $('#error-message').text('Du måste fylla i ett pris');
        $('#item-price').focus();
        return;
    }

    row = '<tr>';
    row += '<td>' + $('#item-description').val().toUpperCase() + '</td>';
    row += '<td>' + $('#item-price').val() + '</td>';
    row += '<td></td>';
    row += '<td><span class="glyphicon glyphicon-trash pull-right" onclick="removeItem(this)" style="cursor: pointer;"></span></td>';
    row += '</tr>';

    $('#items-body').append(row);
    $('#items').removeClass('hidden');

    $('#item-price').val('');
    $("#item-price").prop("readonly", false);
    $('#item-description').val('');
    $('#item-price').val('');
    $('#item-description').focus();

});


$('#save').click(function () {
    
    //PREVENT DOUBLE SAVE
    if ($('#next-spinner').hasClass('hidden') == false)
        return;

    $('#error-message').text('');
    $('#error-message').addClass('hidden');

    //CREATE ITEMS ARRAY
    items = [];
    var rowCount = 0;

    $('#items-body > tr').each(function () {
        items[rowCount] = {
            description: $(this).find('td:nth-child(1)').text(),
            price: $(this).find('td:nth-child(2)').text()
        }

        rowCount++;
    });

    //VALIDATION
    if (items.length == 0) {
        $('#error-message').text('Du måste lägga till minst en vara');
        $('#error-message').removeClass('hidden');
        return;
    }

    if ($('#salesman-first-name').val() == '') {
        $('#error-message').removeClass('hidden');
        $('#error-message').text('Du måste fylla i ett förnamn');
        $('#salesman-first-name').focus();
        return;
    }

    if ($('#salesman-last-name').val() == '') {
        $('#error-message').removeClass('hidden');
        $('#error-message').text('Du måste fylla i ett efternamn');
        $('#salesman-last-name').focus();
        return;
    }

    if ($('#salesman-phone').val() == '') {
        $('#error-message').removeClass('hidden');
        $('#error-message').text('Du måste fylla i ett mobilnummer');
        $('#salesman-phone').focus();
        return;
    }

    $('#next-spinner').removeClass('hidden');
    $('#next-text').addClass('hidden');
    
    //SAVE ITEMS
    var url = host + "admin/_RegisteredItems";
    url = encodeURI(url);
    $.ajax({
        method: 'POST',
        url: url,
        data: {
            salesman: {
                phone: $('#salesman-phone').val(),
                firstname: $('#salesman-first-name').val(),
                lastname: $('#salesman-last-name').val(),
                isMember: $('#salesman-ismember-hidden').val()
            },
            items: items            
        },
        cache: false,
        async: false
    }).done(function (html) {
        $('#item-dialog').html(html);
    });


});

$(".numeric-textbox").keydown(function (e) {
    //Do search if enter
    //if (e.keyCode == 13) {
    //    $('#add-item').click();
    //}

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

$("#salesman-phone").change(function () {
    var url = host + "admin/getuser?phone=" + $("#salesman-phone").val();
    url = encodeURI(url);
    $.ajax({
        url: url,
        cache: false
    }).done(function (user) {
        if (user.Phone != null) {
            $('#salesman-first-name').val(user.FirstName);
            $('#salesman-last-name').val(user.LastName);
            $('#salesman-ismember').prop('checked', user.IsMember);
            $('#item-description').focus();
        }
        
    });  
})

$("#salesman-ismember").change(function (e) {
    if($("#salesman-ismember").is(':checked'))
        $("#salesman-ismember-hidden").val(true);
    else
        $("#salesman-ismember-hidden").val(false);
})

$("#item-isexchangeitem").change(function (e) {
    if ($("#item-isexchangeitem").is(':checked')) {
        $("#item-isexchangeitem-hidden").val(true);
        $('#item-price').val('0');
        $("#item-price").prop("readonly", true);
        $('#add-item').focus();
    }
    else {
        $("#item-isexchangeitem-hidden").val(false);
        $('#item-price').val('');
        $("#item-price").prop("readonly", false);
        $("#item-price").focus();
    }
})