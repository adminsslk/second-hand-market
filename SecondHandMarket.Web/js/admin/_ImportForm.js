// Samma regler som ItemImport.Validate på servern, som kontrollerar igen när varorna sparas
function validateImportRow(tr) {
    var table = $('#import-table');
    var description = $.trim(tr.find('.import-description').val());
    var price = $.trim(tr.find('.import-price').val());
    var quantity = $.trim(tr.find('.import-quantity').val());
    var errors = [];
    var warnings = [];

    if (description === '')
        errors.push('Beskrivning saknas');
    else if (description.length > table.data('description-max-length'))
        errors.push('Beskrivningen är för lång');
    else if (description.length > table.data('label-max-length'))
        warnings.push('Beskrivningen får inte plats på etiketten (max ' + table.data('label-max-length') + ' tecken)');

    if (!/^\d{1,9}$/.test(price) || parseInt(price, 10) <= 0)
        errors.push('Priset måste vara ett heltal större än 0');

    if (!/^\d{1,9}$/.test(quantity) || parseInt(quantity, 10) < 1 || parseInt(quantity, 10) > table.data('max-quantity'))
        errors.push('Antal måste vara 1–' + table.data('max-quantity'));

    var status = tr.find('.import-status');
    var include = tr.find('.import-include');
    tr.removeClass('danger warning');
    status.empty();

    if (errors.length > 0) {
        tr.addClass('danger');
        status.text(errors.join('. '));
        include.prop('checked', false).prop('disabled', true);
    }
    else {
        if (include.prop('disabled'))
            include.prop('checked', true);
        include.prop('disabled', false);
        if (warnings.length > 0) {
            tr.addClass('warning');
            status.text(warnings.join('. '));
        }
        else {
            status.html('<span class="fa fa-check text-success"></span>');
        }
    }
}

function updateImportSummary() {
    var rows = 0, items = 0, total = 0, skipped = 0;

    $('#import-table .import-row').each(function () {
        var tr = $(this);
        if (!tr.find('.import-include').prop('checked')) {
            skipped++;
            return;
        }
        var quantity = parseInt(tr.find('.import-quantity').val(), 10);
        rows++;
        items += quantity;
        total += quantity * parseInt(tr.find('.import-price').val(), 10);
    });

    var text = items + ' varor från ' + rows + ' rader, totalt ' + total.toLocaleString('sv-SE') + ' kr.';
    if (skipped > 0)
        text += ' ' + skipped + ' rader hoppas över.';

    $('#import-summary').text(text);
    $('#import-save').prop('disabled', items === 0);
    $('#import-save-text').text(items > 0 ? 'IMPORTERA ' + items + ' VAROR' : 'IMPORTERA');
}

function showImportError(message) {
    $('#import-error').text(message).removeClass('hidden');
}

$('#import-read').click(function () {
    $('#import-error').addClass('hidden');
    $('#import-preview').empty();
    $('#import-save').prop('disabled', true);
    $('#import-save-text').text('IMPORTERA');

    if ($('#import-salesman-id').val() === '') {
        showImportError('Välj en återförsäljare.');
        return;
    }

    if ($('#import-file')[0].files.length === 0) {
        showImportError('Välj en CSV-fil.');
        return;
    }

    $('#import-read-spinner').removeClass('hidden');

    $.ajax({
        url: host + 'admin/_ImportPreview',
        type: 'POST',
        data: new FormData($('#import-form')[0]),
        processData: false,
        contentType: false,
        cache: false
    }).done(function (html) {
        $('#import-preview').html(html);
        $('#import-table .import-row').each(function () {
            validateImportRow($(this));
        });
        updateImportSummary();
    }).fail(function (xhr) {
        showImportError('Det gick inte att läsa filen (' + xhr.status + ' ' + xhr.statusText + ').');
    }).always(function () {
        $('#import-read-spinner').addClass('hidden');
    });
});

// Byter man återförsäljare, tagg eller fil måste filen läsas in igen
$('#import-salesman-id, #import-file, #import-tag').on('input change', function () {
    $('#import-preview').empty();
    $('#import-save').prop('disabled', true);
    $('#import-save-text').text('IMPORTERA');
});

$('#import-preview').on('input change', '.import-description, .import-price, .import-quantity', function () {
    validateImportRow($(this).closest('tr'));
    updateImportSummary();
});

$('#import-preview').on('change', '.import-include', function () {
    updateImportSummary();
});

$('#import-save').click(function () {
    //PREVENT DOUBLE SAVE
    if ($('#import-save-spinner').hasClass('hidden') == false)
        return;

    $('#import-error').addClass('hidden');

    var rows = [];
    $('#import-table .import-row').each(function () {
        var tr = $(this);
        if (!tr.find('.import-include').prop('checked'))
            return;
        rows.push({
            RowNumber: tr.data('row-number'),
            Description: $.trim(tr.find('.import-description').val()),
            Price: $.trim(tr.find('.import-price').val()),
            Quantity: $.trim(tr.find('.import-quantity').val())
        });
    });

    if (rows.length === 0) {
        showImportError('Det finns inga varor att importera.');
        return;
    }

    $('#import-save-spinner').removeClass('hidden');
    $('#import-save').prop('disabled', true);

    $.ajax({
        url: host + 'admin/ImportItems',
        type: 'POST',
        data: JSON.stringify({
            salesmanId: $('#import-table').data('salesman-id'),
            rows: rows
        }),
        contentType: 'application/json; charset=utf-8',
        cache: false
    }).done(function (html) {
        $('#item-dialog').html(html);
    }).fail(function (xhr) {
        $('#import-save-spinner').addClass('hidden');
        $('#import-save').prop('disabled', false);
        showImportError(xhr.status == 400 && xhr.responseText ? xhr.responseText : 'Importen misslyckades (' + xhr.status + ' ' + xhr.statusText + ').');
    });
});
