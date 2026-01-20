var urlRedirect = 'https://bayi.burateknoloji.com/';

$(document).ready(function () {
    $("#frmDef").validate({
        errorElement: 'div',
        errorClass: 'text-danger',
        rules: {
            "Name": {
                required: true
            },
            "Phone": {
                required: true,
                digits: true,
                minlength: 11
            },
            "Title": {
                required: true
            },
            "TaxNr": {
                required: true,
                digits: true,
                minlength: 10
            },
            "TaxOffice": {
                required: true
            },
            "Address": {
                required: true
            },
            "Website": {
                required: true
            },
            "MonthlyGiro": {
                required: true
            },
            "Email": {
                required: true,
                email: true
            },
            "Password": {
                required: true,
                minlength: 4
            },
            "Confirm": {
                required: true,
                equalTo: "#password"
            }
        },
        messages: {
            "Name": {
                required: "Lütfen ad soyad giriniz"
            },
            "Phone": {
                required: "Lütfen gsm numarası giriniz",
                digits: "Rakamlardan oluşmalıdır",
                minlength: "Uzunluk 11 haneli olmalıdır"
            },
            "Title": {
                required: "Lütfen ünvan giriniz"
            },
            "TaxNr": {
                required: "Lütfen vergi no giriniz",
                digits: "Rakamlardan oluşmalıdır",
                minlength: "Uzunluk en az 10 haneli olmalıdır"
            },
            "TaxOffice": {
                required: "Lütfen vergi dairesi giriniz"
            },
            "Address": {
                required: "Lütfen adres giriniz"
            },
            "Website": {
                required: "Lütfen website adresinizi giriniz"
            },
            "MonthlyGiro": {
                required: "Lütfen ortalama aylık ciro seçiniz"
            },
            "Email": {
                required: "Lütfen email giriniz",
                email: "Geçerli bir email giriniz"
            },
            "Password": {
                required: "Lütfen şifre giriniz",
                minlength: "Uzunluk en az 4 haneli olmalıdır"
            },
            "Confirm": {
                required: "Lütfen şifre tekrarı giriniz",
                equalTo: "Şifreler uyumlu değil"
            }
        }
    })
});

function onSuccessForm(response) {
    if (response.status === "ERROR") {
        alertify.set('notifier', 'position', 'top-center');
        alertify
            .notify(response.message, 'error', 5, function () { window.parent.location.href = "https://burateknoloji.com/kurumsalbasvuru"; })
            .dismissOthers();
    }
    else {
        alertify.dialog('alert').set({ transition: 'zoom', message: 'Transition effect: zoom' });
        alertify.alert('Closable: false').set('closable', false).set('label', 'TAMAM');;
        alertify.alert(
            'TEBRİKLER',
            '<p class="text-dark">Başvurunuz Alındı.<br>Giriş ekranına yönlendirileceksiniz.<br><span class="text-custom fw-bold">Kurumsal</span> giriş yaparak devam edebilirsiniz.<br>İyi Günler.</p>',
            function () { window.parent.location.href = "https://burateknoloji.com/giris"; }
        );
    }
}