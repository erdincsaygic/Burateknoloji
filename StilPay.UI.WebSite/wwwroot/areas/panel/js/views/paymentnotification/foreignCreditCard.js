var urlRedirect = '/panel/paymentnotification/validation';
var input = document.querySelector("#phone");
var isValid = false;

$(document).ready(function () {
    $("#creditCardForm").validate({
        errorElement: 'span',
        errorClass: 'text-danger',
        rules: {
            "CreditCardModel.SenderName": {
                required: true,
            },
            "CreditCardModel.CardNumber": {
                required: true,
                minlength: 16
            },
            "CreditCardModel.ExpirationDate": {
                required: true,
            },
            "CreditCardModel.SecurityCode": {
                required: true,
                minlength: 3
            },
            //"CreditCardModel.PhoneNumber": {
            //    intTelInput: true
            //},
            "CreditCardModel.CountryCode": {
                required: true,
            },
        },
        messages: {
            "CreditCardModel.SenderName": {
                required: "Ad-Soyad giriniz"
            },
            "CreditCardModel.CardNumber": {
                required: "Kredi kartı numarası giriniz.",
                minlength: "Kredi Kartı 16 haneli olmalıdır."
            },
            "CreditCardModel.ExpirationDate": {
                required: "Son kullanma tarihi giriniz",
            },
            "CreditCardModel.SecurityCode": {
                required: "CVV giriniz",
                minlength: "CVV 3 rakamdan oluşmaktadır"
            },
            "CreditCardModel.CountryCode": {
                required: "Ülke kodu boş bırakılamaz",
            },
        }
    });

    //$.validator.addMethod('intTelInput', function (value, element, param) {
    //    window.iti = iti;
    //    var iti = intlTelInput(input);

    //    if (!iti.isValidNumber()) {
    //        return false;
    //    }

    //    return true;
    //}, 'Telefon Numaranızı Doğru Ülke Kodu İle ve Doğru Formatta Giriniz.');

    var onlyLettersRule = document.getElementById('name');
    onlyLettersRule.addEventListener("keyup", function () {
        if (onlyLettersRule.value.match(/[^a-zA-Z' 'wığüşöçĞÜŞÖÇİ]/g)) {
            onlyLettersRule.value = this.value.replace(/[^a-zA-Z' 'wığüşöçĞÜŞÖÇİ]/g, '');
        }
    });

    $('#phone').val("");
    $('#cardnumber').val("");
    //$('#phone').keyup( function (e) {
    //    if (this.value[0] == 0)
    //        this.value = "";
    //    if (this.value.length > this.maxLength)
    //        this.value = this.value.slice(0, this.maxLength);       
    //});


    $("#box3").prop('checked', true);
    $("#btnSubmit").addClass("hovers");
    $("#btnSubmit").removeAttr("disabled");

    $("#box3").click(function () {
        if ($(this).prop("checked") == true) {
            $("#btnSubmit").addClass("hovers");
            $("#btnSubmit").removeAttr("disabled");

        }
        else if ($(this).prop("checked") == false) {
            alertify.notify('Lütfen Mesafeli Satış Sözleşmesini Onaylayınız', 'error', 5, function () { }).dismissOthers();
            $("#btnSubmit").removeClass("hovers");
            $("#btnSubmit").attr("disabled", "disabled");
        }
    })

    $('table').on("change", "input[type='radio']", function (e) {
        if ($(this).is(':checked')) {
            e.target.value = e.target.id;
            
            $('#installmentAmount').val($(this).parent().next().next().text());
        }
    });

    $("#cardnumber").bind('cut', function () {
        document.getElementById("personel").innerHTML = "";
    });

    $("#cardnumber").bind("paste", function (e) {
        document.getElementById("cardnumber").value = "";
    });

    setTimeout(function () {
        history.back();
    }, 600000);

    history.pushState(null, document.title, location.href);

    $("#creditCardForm").on('submit', function () {
        window.iti = iti;
        var iti = intlTelInput(input);
        iti.destroy();
        
        if (!isValid && $("#creditCardForm").valid()) {
            $(input).addClass('form-control is-invalid');
            alertify.notify('Telefon Numaranızı Doğru Ülke Kodu İle ve Doğru Formatta Giriniz.', 'error', 3, function () { }).dismissOthers();
            return false;
        }
    });
});

window.addEventListener("load", function () {

    var iti = window.intlTelInput(input, {
        hiddenInput: "full_phone",
        nationalMode: false,
        formatOnDisplay: true,
        separateDialCode: true,
        autoHideDialCode: false,
        autoPlaceholder: "aggressive",
        placeholderNumberType: "MOBILE",
        initialCountry: "auto",
        geoIpLookup: callback => {
            fetch("https://ipapi.co/json")
                .then(res => res.json())
                .then(data => callback(data.country_code))
                .catch(() => callback("us"));
        },
        utilsScript: "/areas/panel/js/inttelinput/utils.js?1687509211722"
    });


    input.addEventListener('keyup', formatIntlTelInput);
    input.addEventListener('change', formatIntlTelInput);

    function formatIntlTelInput() {
        if (typeof intlTelInputUtils !== 'undefined') { // utils are lazy loaded, so must check
            var currentText = iti.getNumber(intlTelInputUtils.numberFormat.E164);
            if (typeof currentText === 'string') { // sometimes the currentText is an object :)
                iti.setNumber(currentText); // will autoformat because of formatOnDisplay=true
            }
        }
    }

    input.addEventListener("countrychange", function () {
        input.value = '';
        reset();
    });


    input.addEventListener('keyup', function () {
        reset();
        if (input.value.trim()) {
            if (iti.isValidNumber()) {
                isValid = true;
                $(input).addClass('form-control is-valid');
                $("#countryCode").val($(".iti__selected-dial-code").html())             

            } else {
                $(input).addClass('form-control is-invalid');
                isValid = false;
            }
        }
    });

    input.addEventListener('change', reset);
    input.addEventListener('keyup', reset);

    var reset = function () {
        $(input).removeClass('form-control is-invalid');
    };
});


function isPhoneNumberKey(evt) {
    var charCode = (evt.which) ? evt.which : evt.keyCode
    if (charCode != 43 && charCode > 31 && (charCode < 48 || charCode > 57))
        return false;
    return true;
}


function getCreditCardNumber() {
    var creditCardNumber = document.getElementById("cardnumber").value.replace(/\D/g, "");
    document.getElementById("personel").innerHTML = "";
    if (creditCardNumber.length > 16) {
        creditCardNumber = creditCardNumber.slice(0, 16);    
    }
    if (creditCardNumber.length == 16) {
        $.ajax({ 
            type: 'GET',
            url: '/Panel/PaymentNotification/ForeignCreditCardInstallmentOptions',
            data: { creditCardNumber },
            success: function (data) {
                if (data.status == "ERROR") {
                    alertify.notify(data.message, 'error', 5, function () { }).dismissOthers();
                }
                else {
                    getInstallmentOptionsValues(data)
                }
            },
            error: function (data) {
                alertify.notify(data.message, 'error', 5, function () { }).dismissOthers();
                setTimeout(function () {
                    history.back();
                }, 5000)
            }
        })
    }
}

function getInstallmentOptionsValues(data) {
    showLoading();
    setTimeout(function () {
        $.each(data, function (key, value) {
            if (parseInt(key) == 1) {
                $('#personel')
                    .append(`<tr>`)
                    .append(`<th></th>`)
                    .append(`<th>Taksit Sayısı</th>`)
                    .append(`<th>Ödeme</th>`)
                    .append(`</tr>`)
                    .append(`<tr>`)
                    .append(`<td><input type="radio" id=${parseInt(key)} name="CreditCardModel.InstallmentMonth" value="1" checked></td>`)
                    .append(`<td><label for=${parseInt(key)}>Tek Çekim</label></td>`)
                    .append(`<td>${value}</td>`)
                    .append(`</tr>`)

                $('#installmentAmount').val(value);
            }
            else {
                $('#personel')
                    .append(`<tr>`)
                    .append(`<td><input type="radio" id=${parseInt(key)} name="CreditCardModel.InstallmentMonth"></td>`)
                    .append(`<td><label for=${parseInt(key)}>${key}</label></td>`)
                    .append(`<td>${value}</td>`)
                    .append(`</tr>`)
            }
        });
    }, 1000);
    hideLoading();
}

function showLoading() {
    Swal.fire({
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
}

function hideLoading() {
    setTimeout(function () {
        Swal.close();
    }, 500);
}


function onBegin() {
    $("#btnSubmit").removeClass("hovers");
    $("#btnSubmit").attr("disabled", "disabled");
}

function onFailure() {
    setTimeout(function () {
        Swal.fire({
            title: '<p class="fs-4 text-danger">Opps!</p>',
            showConfirmButton: true,
            confirmButtonText: "Tamam",
            confirmButtonColor: "#dc3545",
            allowOutsideClick: false,
            allowEscapeKey: false,
        })
    }, 1000)

    $("#btnSubmit").addClass("hovers");
    $("#btnSubmit").removeAttr("disabled");
}

function onSuccess(response) {
    setTimeout(function () {
        if (response.status === "ERROR") {
            alertify.notify(response.message, 'error', 5, function () { }).dismissOthers();
            setTimeout(function () {
                history.back();
            }, 5000)
        }
        else
            window.location.href = urlRedirect;
    }, 1000)
}

