var urlLIST = '/membertype/index';

$(document).ready(function () {
    $("#frmDef").validate({
        errorElement: 'div',
        errorClass: 'text-danger',
        rules: {
            "entity.Name": {
                required: true
            }
        },
        messages: {
            "entity.Name": {
                required: "Lütfen ad giriniz"
            }
        }
    })

    $("#entity_MinAmount").numeric_format({
        defaultvalue: "",
        thousandslimit: 7,
        centsLimit: 2,
        centsSeparator: ',',
        thousandsSeparator: '.',
        clearOnEmpty: true
    })

    $("#entity_MaxAmount").numeric_format({
        defaultvalue: "",
        thousandslimit: 7,
        centsLimit: 2,
        centsSeparator: ',',
        thousandsSeparator: '.',
        clearOnEmpty: true
    })

    $("#entity_Quantity").numeric_format({
        defaultvalue: "",
        thousandslimit: 5,
        centsLimit: 0,
        centsSeparator: ',',
        thousandsSeparator: '.',
        clearOnEmpty: true
    })
});
