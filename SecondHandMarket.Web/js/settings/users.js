function insertUserRow(email) {
    var url = host + "settings/_UserRow?email=" + email;
    url = encodeURI(url);
    $.ajax({
        url: url,
        cache: false,
        async: true
    }).done(function (html) {
        $('#user-table tr:last').after(html);
    });
}

function updateUserRow(id) {
    var url = host + "settings/_UserRow?id=" + id;
    url = encodeURI(url);
    $.ajax({
        url: url,
        cache: false,
        async: true
    }).done(function (html) {
        $('#tr-user-' + id).replaceWith(html);
    });
}

function updateUserRole(id, roleId) {
    var url = host + "settings/updateUserRole?id=" + id + "&roleId=" + roleId;
    url = encodeURI(url);
    $.ajax({
        url: url,
        cache: false,
        async: true
    }).done(function (html) {
        $('#tr-user-' + id).replaceWith(html);
    });
}

function updateMembership(id) {
    var isMember = $('#is-member-checkbox-' + id).is(":checked");

    var url = host + "settings/UpdateMembership?userId=" + id + "&isMember=" + isMember;
    url = encodeURI(url);
    $.ajax({
        url: url,
        cache: false,
        async: true
    });
}

function showDeleteUserForm(id) {
    $('#item-dialog').data('action', 'nothing');
    var url = host + "settings/_deleteuserform?id=" + id;
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

function showEditUserForm(id) {
    var url = host + "settings/_edituserform?id=" + id;
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

function showRegisterUserForm() {
    var url = host + "settings/_registeruserform";
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

function submitForm(pageIndex) {
    $('#page-index').val(pageIndex);
    $('#search-form').submit();
}
