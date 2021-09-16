function isNumber(evt) {
    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        return false;
    }
    return true;
}

$(document).ready(function () {
    $('#btnLogin').click(function (e) {
        var isValid = true;
        $('#loginEmail,#loginPassword').each(function () {
            if ($.trim($(this).val()) == '') {
                isValid = false;
                $(this).removeClass('custom-input-success').addClass('custom-input-error');
                $(this).keypress(function () {
                    $(this).removeClass('custom-input-error').addClass('custom-input-success');
                });
            }
            else {
                var emailReg = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;
                var emailaddressVal = $("#loginEmail").val();
                if (!emailReg.test(emailaddressVal)) {
                    $("#loginEmail").removeClass('custom-input-success').addClass('custom-input-error');
                    isValid = false;
                }


            }
        });
        if (isValid == false)
            e.preventDefault();
    });

    $('#btnUserSignUp').click(function (e) {
        
        
        var isValid = true;
        if ($("#AccountType").val() == "0") {
            isValid = false;
            $(".regTab a").removeClass("active").addClass('custom-input-error');
        }
        $('#UserName,#UserEmail,#UserPassword,#ConfirmPassword').each(function () {
            if ($.trim($(this).val()) == '') {
                isValid = false;
                $(this).removeClass('custom-input-success').addClass('custom-input-error');
                $(this).keypress(function () {
                    $(this).removeClass('custom-input-error').addClass('custom-input-success');
                });
            }
            else {
                var emailReg = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;
                var emailaddressVal = $("#UserEmail").val();
                if (!emailReg.test(emailaddressVal)) {
                    $("#UserEmail").removeClass('custom-input-success').addClass('custom-input-error');
                    isValid = false;
                }


            }
        });
        if ($("#UserPassword").val() != "") {
            $("#passError").text("");
            if ($("#UserPassword").val().length >= 8 && $("#UserPassword").val().length <= 20) {
                if ($("#UserPassword").val() != $("#ConfirmPasword").val()) {
                    isValid = false;
                    $("#UserPassword,#ConfirmPasword").removeClass('custom-input-success').addClass('custom-input-error');
                    $("#passError").text('Passwords do not match');
                    //$("#passError").text('Password must contain minimum 8 characters');
                }
                else {
                  
                    $("#UserPassword,#ConfirmPasword").removeClass('custom-input-error custom-input-success')
                }
            }
            else {
                isValid = false;
               
                $("#UserPassword").removeClass('custom-input-success').addClass('custom-input-error');
            }
        }
        if (!$("#IsAgree").is(":checked")) {
            isValid = false;
            $("#IsAgree").parent("div").removeClass('custom-input-success').addClass('custom-input-error iAgree')
            $("#IsAgree").change(function () {
                if ($(this).is(":checked")) {

                    $("#IsAgree").parent("div").removeClass('custom-input-error custom-input-success').addClass('custom-input-success');
                    isValid = true;
                }
            });
        }

        if (isValid == false)
            e.preventDefault();
    });

    //$('#btnFrgtPass').click(function (e) {
    //    
    //    var isValid = true;
    //    $('#forgotPassEmail').each(function () {
    //        if ($.trim($(this).val()) == '') {
    //            isValid = false;
    //            $(this).removeClass('custom-input-success').addClass('custom-input-error');
    //            $(this).keypress(function () {
    //                $(this).removeClass('custom-input-error').addClass('custom-input-success');
    //            });

    //        } else {

    //            var emailReg = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;
    //            var emailaddressVal = $("#forgotPassEmail").val();
    //            if (!emailReg.test(emailaddressVal)) {
    //                $("#forgotPassEmail").removeClass('custom-input-success').addClass('custom-input-error');
    //                isValid = false;
    //            }
    //            if (isValid) {
    //                $("#btnFrgtPass").val("Please wait...")
    //                $.ajax({
    //                    url: '/Home/CheckUserExistence',
    //                    data: '{"email":"' + $.trim($(this).val()) + '"}',
    //                    contentType: "application/json;charset=utf-8",
    //                    dataType: 'string',
    //                    type: "POST",
    //                    success: function (response) {
    //                        
    //                        $("#btnFrgtPass").val("Reset Password")
    //                        if (response.responseText == "success")
    //                            toastr['success']('We have sent a password reset link on your email');
    //                        else if (response.responseText == "not_found")
    //                            toastr['error']('Sorry, we don\'t have this email in our system');
    //                        else
    //                            toastr['error']('Some error occurred while processing the request. Please try again');
    //                    },
    //                    error: function (response) {
    //                        
    //                        $("#btnFrgtPass").val("Reset Password")
    //                        if (response.responseText == "success")
    //                            toastr['success']('We have sent a password reset link on your email');
    //                        else if (response.responseText == "not_found")
    //                            toastr['error']('Sorry, we don\'t have this email in our system');
    //                        else
    //                            toastr['error']('Some error occurred while processing the request. Please try again');
    //                    }
    //                })
    //            }
              
    //        }
    //    });


    //    if (isValid == false)
    //        e.preventDefault();
    //});

    $('#btnResetPassword').click(function (e) {
        var isValid = true;
        $('#resetPassword,#resetConfirmPassword').each(function () {
            if ($.trim($(this).val()) == '') {
                isValid = false;
                $(this).removeClass('custom-input-success').addClass('custom-input-error');
                $(this).keypress(function () {
                    $(this).removeClass('custom-input-error').addClass('custom-input-success');
                });
            }
            else {
                if ($("#resetPassword").val().length >= 8 && $("#resetPassword").val().length <= 20) {
                    if ($("#resetPassword").val() != $("#resetConfirmPassword").val()) {
                        isValid = false;
                        $("#resetPassword,#resetConfirmPassword").removeClass('custom-input-success').addClass('custom-input-error');
                    }
                }
                else {
                    isValid = false;
                    $("#resetPassword").removeClass('custom-input-success').addClass('custom-input-error');
                }
            }
        });


        if (isValid == false)
            e.preventDefault();
    });


    function calculate_time_zone() {
        var minutes;
        var rightNow = new Date();
        var jan1 = new Date(rightNow.getFullYear(), 0, 1, 0, 0, 0, 0);  // jan 1st
        var june1 = new Date(rightNow.getFullYear(), 6, 1, 0, 0, 0, 0); // june 1st
        var temp = jan1.toGMTString();
        var jan2 = new Date(temp.substring(0, temp.lastIndexOf(" ") - 1));
        temp = june1.toGMTString();
        var june2 = new Date(temp.substring(0, temp.lastIndexOf(" ") - 1));
        var std_time_offset = (jan1 - jan2) / (1000 * 60 * 60);
        var daylight_time_offset = (june1 - june2) / (1000 * 60 * 60);
        var dst;
        if (std_time_offset == daylight_time_offset) {
            dst = "0"; // daylight savings time is NOT observed
        } else {
            // positive is southern, negative is northern hemisphere
            var hemisphere = std_time_offset - daylight_time_offset;
            if (hemisphere >= 0)
                std_time_offset = daylight_time_offset;
            dst = "1"; // daylight savings time is observed
        }
        var i;
        // Here set the value of hidden field to the ClientTimeZone.
        minutes = convert(std_time_offset);
        // Setting TimeZone to cookie
        document.cookie = "TimeZone=TimeZone=" + minutes + ";path=/";
        return minutes;
    }
    // This function is to convert the timezoneoffset to Standard format
    function convert(value) {
        var hours = parseInt(value);
        value -= parseInt(value);
        value *= 60;
        var mins = parseInt(value);
        value -= parseInt(value);
        value *= 60;
        var secs = parseInt(value);
        var display_hours = hours;
        // handle GMT case (00:00)
        if (hours == 0) {
            display_hours = "00";
        } else if (hours > 0) {
            // add a plus sign and perhaps an extra 0
            display_hours = (hours < 10) ? "+0" + hours : "+" + hours;
        } else {
            // add an extra 0 if needed
            display_hours = (hours > -10) ? "-0" + Math.abs(hours) : hours;
        }
        mins = (mins < 10) ? "0" + mins : mins;
        return display_hours + ":" + mins;
    }
    // Adding the funtion to onload event of document object
    onload = calculate_time_zone;
    //loginplaceholder
})

