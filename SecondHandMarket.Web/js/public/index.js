function showItems(phone) {

}


$(document).ready(function () {
    //if ($.cookie('section') == 'items') {
    //    $('html,body').animate({
    //        scrollTop: $('#items').offset().top,
    //        options: { done: $.cookie('section', '') }
    //    }, 0);
    //}

    $("#item-search-button").click(function () {
        //url = host + "public/index?id=" + $("#item-id").val();
        //$.cookie('section', 'items');
        //window.location.href = url;

        $('#search-form').submit();
    });

    $('.activate-item-search').click(function () {
        if (isTouchDevice == false) {
            $('#item-id').focus();
            $("#item-id")[0].setSelectionRange($("#item-id").val().length, $("#item-id").val().length);
        }
    });

    $("#search").keydown(function (e) {
        //Do search if enter
        if (e.keyCode == 13) {
            $('#search-form').submit();
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
});

function getParameterByName(name) {
    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
    return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}