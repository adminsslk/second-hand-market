function reload(e) {
    var url = host + "admin/stats?year=" + $(e).val();
    window.location = url;
}

$(document).ready(function () {

    $('#year-tabs a').click(function (e) {
        e.preventDefault()
        $(this).tab('show')
    })

    $('.edit-user').click(function () {
        var url = host + "admin/_edituserform?id=" + $(this).data('id');
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