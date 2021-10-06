/// <reference path="_references.js" />

$(document).ready(function () {

    //Resize section heights to fit screen
    $(".full-screen-section").each(function () {
        if($(this).outerHeight() < $(window).height()){
            $(this).outerHeight($(window).height());
        }
    });

    // Closes the sidebar menu
    $("#menu-close").click(function (e) {
        e.preventDefault();
        $("#sidebar-wrapper").toggleClass("active");
    });

    // Opens the sidebar menu
    $("#menu-toggle").click(function (e) {
        e.preventDefault();
        $("#sidebar-wrapper").toggleClass("active");
    });

    // Scrolls to the selected menu item on the page
    $(function () {
        $('a[href*=#]:not([href=#])').click(function () {
            if (location.pathname.replace(/^\//, '') == this.pathname.replace(/^\//, '') || location.hostname == this.hostname) {

                var target = $(this.hash);
                target = target.length ? target : $('[name=' + this.hash.slice(1) + ']');
                if (target.length) {
                    $('html,body').animate({
                        scrollTop: target.offset().top,
                        options: { done: scrollDone() },
                    }, 1000);
                    return false;
                }
            }

        });
    });

    // Closes sidebar menu after selection
    function scrollDone() {
        $("#sidebar-wrapper").removeClass("active");
    }

    $('.edit-user').click(function () {
        var url = host + "settings/_edituserform?id=" + $(this).data('id');
        url = encodeURI(url);
        $.ajax({
            url: url,
            cache: false,
            async: true
        }).done(function (html) {
            $('#item-dialog').html(html);
            $('#item-dialog').modal('show');
        });
    });
});

function changePassword(id) {
    var url = host + "settings/_changepasswordform?id=" + id;
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

