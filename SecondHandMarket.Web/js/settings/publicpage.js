function saveFrontpageSection(e) {
    textArea = $('#' + e.id);
    year = textArea.data('year');
    sectionName = textArea.data('section');
    var html = tinyMCE.get(e.id).getContent();
    var url = host + "settings/SavePublicPageSection";
    url = encodeURI(url);

    $.ajax({
        type: 'POST',
        url: url,
        data: { year: year, sectionName: sectionName, html: html },
        cache: false,
        async: true,
        error: function (xhr, ajaxOptions, thrownError) { alert(xhr.status + ": " + thrownError); }
    });
}

$(document).ready(function () {
    tinymce.init({
        selector: '.frontpage-section',
        plugins: 'code image save',
        toolbar: 'save | styleselect | bold italic | link image | code',
        menubar: false,
        height: 500,
        content_css: host + 'css/bootstrap.css',
        save_onsavecallback: function () { saveFrontpageSection(this); },
        language: 'sv_SE',
        relative_urls: false,
    });
});