var urlLIST = '';

function AddFile(name) {
    $("#file" + name).trigger('click')
}

function ChangeFile(e, name) {
    if (e.target.value)
        $("#txt" + name).val('Evrak yüklendi');
    else
        $("#txt" + name).val('');
}

function ClearFile(name) {
    $("#file" + name).val('');
    $("#" + name).val('');
    $("#txt" + name).val('');
}