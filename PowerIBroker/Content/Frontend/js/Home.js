$(document).ready(function () {
    $('#btnUserSignUpHome').click(function (e) {

        var isValid = true;
        $('#ClientName,#Contact,#ComName,#ComEmail,#CNOE,#CMessage').each(function () {
            if ($.trim($(this).val()) == '') {
                isValid = false;
                $(this).removeClass('custom-input-success').addClass('custom-input-error');
                $(this).keypress(function () {
                    $(this).removeClass('custom-input-error').addClass('custom-input-success');
                });
            }

        });
        if (isValid == false)
            e.preventDefault();
    });
});

$(function () {
    $('#Contact,#CNOE').keydown(function (e) {
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
