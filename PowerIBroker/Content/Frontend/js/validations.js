var invalidlogin = "";
var signUpMessage = "";
var enquiryMessage = "";
//----------------frontent  -----------------//

//saving contact us content
$(document).ready(function () {
    $('#btnContactUs').click(function (e) {     
        var isValid = true;
        $('#ContactUsName,#ContactUsPhone,#ContactUsEmail,#Query').each(function () {
            if ($.trim($(this).val()) == '') {
                isValid = false;
                $(this).removeClass('custom-input-success').addClass('custom-input-error');
                $(this).keypress(function () {
                    $(this).removeClass('custom-input-error').addClass('custom-input-success');
                });
            }
            else {
                var emailReg = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;
                var emailaddressVal = $("#ContactUsEmail").val();
                if (!emailReg.test(emailaddressVal)) {
                    $("#ContactUsEmail").removeClass('custom-input-success').addClass('custom-input-error');
                    isValid = false;
                }

            }
        });

        if (isValid) {
            var response = grecaptcha.getResponse();
            if (response.length == 0) {
                isValid = false;
                //alert('Please click on reCAPTCHA image');
                alert('Please check the checkbox on reCAPTCHA');
            }
            else {
                $("#CaptchaContactUs").click();
            }

        }

        if (isValid == false)
            e.preventDefault();
    });

    $('#btn_ContactUs').click(function (e) {

        var isValid = true;
        $('#ContactUs_Name,#ContactUs_Phone,#ContactUs_Email,#ContactUs_Query').each(function () {
            if ($.trim($(this).val()) == '') {
                isValid = false;
                $(this).removeClass('custom-input-success').addClass('custom-input-error');
                $(this).keypress(function () {
                    $(this).removeClass('custom-input-error').addClass('custom-input-success');
                });
            }
            else {
                var emailReg = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;
                var emailaddressVal = $("#ContactUs_Email").val();
                if (!emailReg.test(emailaddressVal)) {
                    $("#ContactUs_Email").removeClass('custom-input-success').addClass('custom-input-error');
                    isValid = false;
                }

            }
        });


        if (isValid == false)
            e.preventDefault();
    });


    var curr = $('#CultureCode').val();
    var phoneFormat = getmaskingformat(curr);
    $("#ContactUsPhone").mask(phoneFormat);
    // add 13 dec Login Validation check start
    $('#btnUserLogin').click(function (e) {
     
        var isValid = true;
        $('#LoginEmail,#LoginPassword').each(function () {
            if ($.trim($(this).val()) == '') {
                isValid = false;
                $(this).removeClass('custom-input-success').addClass('custom-input-error');
                $(this).keypress(function () {
                    $(this).removeClass('custom-input-error').addClass('custom-input-success');
                });
            }

        });
        //if (isValid) {
        //    $("#CaptchaLogin").click();
        //}
        if (isValid) {
            var response = grecaptcha.getResponse();
            if (response.length == 0) {
                isValid = false;
                alert('Please check the checkbox on reCAPTCHA');
            }
            else {
                //$("#CaptchaAdminLogin").click();
                $("#Loginformsubmit").submit();
            }
        }

        ////
        //if (isValid == true) {
        //    var response = grecaptcha.getResponse();
        //    if (response.length == 0) {
        //        isValid = false;
        //        sweetAlert({
        //            title: "",
        //            text: "Please select reCAPTCHA",
        //            type: "error"
        //        });
        //    }
        //}

        if (isValid == false)
            e.preventDefault();
        else {

            var culturename = window.navigator.languages[0];
            $('#CultureName').val(culturename);
        }

    });
    // add 13 dec Login Validation check End
});



$(function () {
    $('#ContactUsPhone').keydown(function (e) {
        if (e.shiftKey || e.ctrlKey || e.altKey) {
            e.preventDefault();
        } else {
            var key = e.keyCode;
            if (!((key == 8) || (key == 9) || (key == 46) || (key >= 35 && key <= 40) || (key >= 48 && key <= 57) || (key >= 96 && key <= 105))) {
                e.preventDefault();
            }
        }
    });
});

function getmaskingformat(curr) {

    var phoneFormat = "";
    switch (curr) {
        case "en-IN":
            phoneFormat = "(999) 999-9999";
            break;
        case "en-GB":
            phoneFormat = "(999) 999-9999";
            break;
        case "es-ES":
            phoneFormat = "(999) 999-9999";
            break;
        case "fr-CA":
            phoneFormat = "99 99 99 99 99";
            break;
        case "de-CH":
            phoneFormat = "999 999-9999";
            break;

        case "zh-CN":
            phoneFormat = "99-9999-9999";
            break;
        case "fr-FR":
            phoneFormat = "99 99 99 99 99";
            break;

        case "pl-PL":
            phoneFormat = "999-999-9999";
            break;
        case "zh-SG":
            phoneFormat = "99-9999-9999";
            break;

        case "th-TH":
            phoneFormat = "(999) 999-9999";
            break;

        default:
            phoneFormat = "(999) 999-9999";
            break;
    }
    return phoneFormat;


}


