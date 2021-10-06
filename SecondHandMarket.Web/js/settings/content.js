function showDeleteFileForm(fileName) {
    $('#file-name').text(fileName);
    $('#file-name-hidden').val(fileName);
    $('#delete-file-dialog').show();
}

function hideDeleteFileForm() {
    $('#delete-file-dialog').hide();
}