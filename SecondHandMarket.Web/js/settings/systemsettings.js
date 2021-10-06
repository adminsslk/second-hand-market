function getYear(year) {
    result = null;
    url = host + "settings/GetYear?key=" + year;

    $.ajax({
        type: 'GET',
        url: url,
        dataType: 'json',
        success: function (data) {
            result = data;
        },
        data: {},
        async: false
    });
    return result;
}

function updateActiveYear(year, salesCost, revenueShare) {

    $.ajax({
        url: host + "settings/UpdateActiveYear",
        type: "POST",
        data: JSON.stringify({
            Value: year,
            SalesCost: salesCost,
            RevenueShare: revenueShare
        }),
        contentType: "application/json; charset=utf-8",
        cache: false,
        async: false
    });
}


$(document).ready(function () {
    $('#edit-general-settings').click(function (e) {
        $('.edit').removeClass('hidden');
        $('.read').addClass('hidden');
    })

    $('#read-general-settings').click(function (e) {
        $('.edit').addClass('hidden');
        $('.read').removeClass('hidden');
    })

    $('.year-dropdown-item').click(function (e) {
        e.preventDefault();
        var year = getYear(e.target.innerText);
        $('#dropdownMenu1').html(year.Value + '<span class="caret"></span>');
        $('#year-revenue-share').val(year.RevenueShare * 100);
        $('#year-sales-cost').val(year.SalesCost);
    })

    $("#save-general-settings").click(function () {
        year = $('#dropdownMenu1').text();
        salesCost = $('#year-sales-cost').val();
        revenueShare = $('#year-revenue-share').val() / 100;

        updateActiveYear(year, salesCost, revenueShare);

        window.location.reload();
    });
});