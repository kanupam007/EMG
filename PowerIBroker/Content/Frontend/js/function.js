var ids;
function offsetAnchor() {
    if (location.hash.length !== 0) {
        ids = location.hash;
        var scrollAmnt = $(ids).offset().top;
        setTimeout(function () {
            $('html,body').animate({ scrollTop: scrollAmnt - 70 }, 2000);
        }, 500);

    }
}
$(window).on("hashchange", function () {
    offsetAnchor();
});
window.setTimeout(function () {
    offsetAnchor();
    //event.preventDefault();

}, 1);



$(document).ready(function () {
    setTimeout(function () { $('html,body').animate({ scrollTop: 00 }, 0000); }, 1);
});


$(document).ready(function () {
    setTimeout(function () { $('html,body').animate({ scrollTop: 00 }, 0000); }, 1);
    $(".btn-menu").click(function () {
        $(".btn-menu").toggleClass("btn-menu-open");
        $(".bdrForeResponsive").toggleClass("bdrs1");
        $(".navCon").toggleClass("navOpen");
        $(".MenuBack").toggleClass("MenuBack1");
        $("body ").toggleClass("hiddens");



    });

});

//tab section

$(document).ready(function () {
    $(".tabLink a").click(function () {
        $(".tabLink a").removeClass("tabActive");
        $(this).addClass("tabActive");
        var trgt = $(this).attr("trgt");
        $(".beniftsToclients").hide(0);
        $(trgt).show(0);



    });
});

//menu js

$(document).ready(function () {
    $(".navCon a").click(function () {
        $(".navCon a").parent().parent("li").removeClass("navActive");
        $(this).parent().parent("li").addClass("navActive");
        $(".navCon  a").removeClass("reqDemo1");
        $(this).addClass("reqDemo1");
        $(".bdrForeResponsive").removeClass("bdrs1");
        $(".navCon").removeClass("navOpen");
        $(".MenuBack").removeClass("MenuBack1");
        $("body ").removeClass("hiddens");
        $(".btn-menu").removeClass("btn-menu-open");


    });
});

//video play

$(document).ready(function () {
    $('.videoTag').click(function () {
        if ($(".mediaVideo").get(0).paused) {
            $(".mediaVideo").get(0).play();
            $(this).toggleClass("pause");

        } else {
            $(".mediaVideo").get(0).pause();
            $(this).toggleClass("pause");

        }
    });

});

//video play

//faq js
$(document).ready(function () {
    $(".faq strong").on("click", function (e) {
        if ($(this).parent().has("ul")) {
            e.preventDefault();
        }

        if (!$(this).hasClass("opens")) {
            // hide any open menus and remove all other classes
            $(".faq p").slideUp(350);
            $(".faq strong").removeClass("opens");

            // open our new menu and add the open class
            $(this).next("p").slideDown(350);
            $(this).addClass("opens");
        }

        else if ($(this).hasClass("opens")) {
            $(this).removeClass("opens");
            $(this).next("p").slideUp(350);
        }
    });
});
//faq js


//login pop js

$(document).ready(function () {

    $('.login').click(function () {
        $(".loginPop").addClass('fades');
        $(".backbg").fadeIn(300);
        $("body").addClass("hiddenss");
    });

    var curr = $('#CultureCode').val();
    var phoneFormat = getmaskingformat(curr);
    $("#Phone,#WorkPhone,#EmergancyPhone,#Contact,#CompanyContact,#HomePhone").mask(phoneFormat);
    var ssnFormat = getmaskingformatSSN(curr);
    $("#SSN,#SSNDependent,#ssn").mask(ssnFormat);

});
$('.login').click(function () {
    $.ajax({
        type: "GET",
        url: "/UserLogin/Index",
        //  data: { SponsoredVal: SponsoredVal, RangeStart: RangeStart, RangeEnd: RangeEnd, EmployeeValue: EmployeeValue, SpouseValue: SpouseValue, PlanId: PlanId, CoverageID: coverageID, PayPeriodID: PayPeriodID, IsSpouse: IsSpouse, MinRange: MinRange, MaxRange: MaxRange, CostDes: CostDes },
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            $('#LoginEmail').val(response.Email);
            $('#LoginPassword').val(response.Password);
            //rememberMe
            if (response.RememberMe == true) {
                $('#rememberMe').attr('checked', true);
            }
            else {
                $('#rememberMe').attr('checked', false);
            }
           
            $(".loginPop").addClass('fades');
            $(".backbg").fadeIn(300);
            $("body").addClass("hiddenss");

        }
    });


});

$(".close, .forgpass").click(function () {
    $(".loginPop").removeClass('fades');
    $("body").removeClass("hiddenss");
    $(".backbg").fadeOut(300);
    $("body").css("overflow", "auto")
});

$('.forgpass').click(function () {
    $("#forgotPassPopup").fadeIn(300);
    $(".backbg2").fadeIn(300);
    $("body").addClass("hiddenss");
});

$(".close, .forgClose").click(function () {
    $("#forgotPassPopup").fadeOut(300);
    $("body").removeClass("hiddenss");
    $(".backbg2").fadeOut(300);
    $("body").removeClass("hiddenss");
});

function RemoveCommas(value) {

    return value.replace(/\D/g, '');
}
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
function getmaskingformatSSN(curr) {

    var ssnFormat = "";
    switch (curr) {
        case "en-IN":
            ssnFormat = "999-99-9999";
            break;
        case "es-ES":
            ssnFormat = "999-99-9999";
            break;
        case "en-GB":
            ssnFormat = "999-99-9999";
            break;
        case "fr-CA":
            ssnFormat = "999 999 999";
            break;
        case "de-CH":
            ssnFormat = "99 999 9999";
            break;
        case "zh-CN":
            ssnFormat = "999 999 999";
            break;
        case "fr-FR":
            ssnFormat = "999 999 999";
            break;
        case "pl-PL":
            ssnFormat = "99-999/9999";
            break;
        case "zh-SG":
            ssnFormat = "999 999 999";
            break;
        case "th-TH":
            ssnFormat = "999-99-9999";
            break;
        default:
            ssnFormat = "999-99-9999";
            break;
    }
    return ssnFormat;


}
$(document).on('click', '.btnCurrSave', function (e) {
 
    $('.phonemask').each(function () {
        $(this).val(RemoveCommas($(this).val()));
    })
    $('.ssnmask').each(function () {
        $(this).val(RemoveCommas($(this).val()));
    })

});

$('#btnSavePasswordSSN').click(function (e) {

    var isValid = true;
    var resetPassword = $.trim($('#resetPassword').val());
    var resetConfirmPassword = $.trim($('#resetConfirmPassword').val());
    var Email = $.trim($('#Email').val());
    var Phone = $.trim($('#Phone').val());
    var SSN = $.trim($('#SSN').val());

    if (SSN == "") {
        isValid = false;
        $('#result').text('');
        $('#SSN').removeClass('custom-input-success').addClass('custom-input-error');
        $('#SSN').keypress(function () {
            $('#SSN').removeClass('custom-input-error').addClass('custom-input-success');
        });
    }
    if ((Email == "") && (Phone == "")) {
        isValid = false;
        $('#result').text('');
        $('#Email').removeClass('custom-input-success').addClass('custom-input-error');
        $('#Email').keypress(function () {
            $('#Email').removeClass('custom-input-error').addClass('custom-input-success');
            $('#Phone').removeClass('custom-input-error')
        });
        $('#Phone').removeClass('custom-input-success').addClass('custom-input-error');
        $('#Phone').keypress(function () {
            $('#Phone').removeClass('custom-input-error').addClass('custom-input-success');
            $('#Email').removeClass('custom-input-error')
        });
    }
    else {

        $('#Email').removeClass('custom-input-error');
        $('#Phone').removeClass('custom-input-error');

        if (Email != "") {
            var emailReg = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;
            var emailaddressVal = $("#Email").val();
            if (!emailReg.test(emailaddressVal)) {
                $("#Email").removeClass('custom-input-success').addClass('custom-input-error');
                isValid = false;
            }
        }

    }


    if (resetPassword == "" && resetConfirmPassword == "") {
        if (resetPassword == "") {
            isValid = false;
            $('#result').text('');
            $('#resetPassword').removeClass('custom-input-success').addClass('custom-input-error');
            $('#resetPassword').keypress(function () {
                $('#resetPassword').removeClass('custom-input-error').addClass('custom-input-success');
            });
        }
        if (resetConfirmPassword == "") {
            isValid = false;
            $('#result').text('');
            $('#resetConfirmPassword').removeClass('custom-input-success').addClass('custom-input-error');
            $('#resetConfirmPassword').keypress(function () {
                $('#resetConfirmPassword').removeClass('custom-input-error').addClass('custom-input-success');
            });
        }

    }

    else {

        if (resetPassword == "") {
            isValid = false;
            $('#result').text('');
            $('#resetPassword').removeClass('custom-input-success').addClass('custom-input-error');
            $('#resetPassword').keypress(function () {
                $('#resetPassword').removeClass('custom-input-error').addClass('custom-input-success');
            });
        }
        if (resetConfirmPassword == "") {
            isValid = false;
            $('#result').text('');
            $('#resetConfirmPassword').removeClass('custom-input-success').addClass('custom-input-error');
            $('#resetConfirmPassword').keypress(function () {
                $('#resetConfirmPassword').removeClass('custom-input-error').addClass('custom-input-success');
            });
        }
        var a = checkStrength(resetPassword)
        if (a == 'Weak' || a == 'Too short' || a == 'Good') {
            isValid = false
            $('#result').text(a);
        }
        else {

            $('#result').text(a);
            if ($("#resetPassword").val().length >= 8 && $("#resetPassword").val().length <= 20) {
                if ($("#resetPassword").val() != $("#resetConfirmPassword").val()) {
                    isValid = false;
                    $('#result').text("Password did not match");
                    $('#result').css('color', 'red');
                    $("#resetPassword,#resetConfirmPassword").removeClass('custom-input-success').addClass('custom-input-error');
                }
            }
            else {
                isValid = false;
                $("#resetPassword").removeClass('custom-input-success').addClass('custom-input-error');
            }
        }

    }

    if (isValid == false) {
        e.preventDefault();
    }
    else {
        var Phone = RemoveCommas($.trim($('#Phone').val()))
        var SSN = RemoveCommas($.trim($('#SSN').val()))
        $.trim($('#Phone').val(Phone))
        $.trim($('#SSN').val(SSN))
      
        $('#fsubmit').submit();
    }
});

$('#btnResetPassword').click(function (e) {
    var isValid = true;
    var resetPassword = $.trim($('#resetPassword').val());
    var resetConfirmPassword = $.trim($('#resetConfirmPassword').val());   
    if (resetPassword == "" && resetConfirmPassword == "") {
        if (resetPassword == "") {
            isValid = false;
            $('#result').text('');
            $('#resetPassword').removeClass('custom-input-success').addClass('custom-input-error');
            $('#resetPassword').keypress(function () {
                $('#resetPassword').removeClass('custom-input-error').addClass('custom-input-success');
            });
        }
        if (resetConfirmPassword == "") {
            isValid = false;
            $('#result').text('');
            $('#resetConfirmPassword').removeClass('custom-input-success').addClass('custom-input-error');
            $('#resetConfirmPassword').keypress(function () {
                $('#resetConfirmPassword').removeClass('custom-input-error').addClass('custom-input-success');
            });
        }
    }
    else {
        if (resetPassword == "") {
            isValid = false;
            $('#result').text('');
            $('#resetPassword').removeClass('custom-input-success').addClass('custom-input-error');
            $('#resetPassword').keypress(function () {
                $('#resetPassword').removeClass('custom-input-error').addClass('custom-input-success');
            });
        }
        if (resetConfirmPassword == "") {
            isValid = false;
            $('#result').text('');
            $('#resetConfirmPassword').removeClass('custom-input-success').addClass('custom-input-error');
            $('#resetConfirmPassword').keypress(function () {
                $('#resetConfirmPassword').removeClass('custom-input-error').addClass('custom-input-success');
            });
        }
        var a = checkStrength(resetPassword)
        if (a == 'Weak' || a == 'Too short' || a == 'Good') {
            isValid = false
            $('#result').text(a);
        }
        else {
            // isValid = true
            $('#result').text(a);
            if ($("#resetPassword").val().length >= 8 && $("#resetPassword").val().length <= 20) {
                if ($("#resetPassword").val() != $("#resetConfirmPassword").val()) {
                    isValid = false;                  
                    $('#result').text("Password did not match");
                    $('#result').removeClass('strong').addClass('short');                  
                    $("#resetPassword,#resetConfirmPassword").removeClass('custom-input-success').addClass('custom-input-error');
                }
            }
            else {
                isValid = false;
                $("#resetPassword").removeClass('custom-input-success').addClass('custom-input-error');
            }
        }

    }
    //});
    if (isValid == false)
        e.preventDefault();
});
function checkStrength(password) {

    var strength = 0
    if (password.length > 0 && password.length < 8) {
        $('#result').removeClass()
        $('#result').addClass('short')
        return 'Too short'
    }
    if (password.length >= 8) strength += 1
    // If password contains both lower and uppercase characters, increase strength value.
    if (password.match(/([a-z].*[A-Z])|([A-Z].*[a-z])/)) strength += 1
    // If it has numbers and characters, increase strength value.
    if (password.match(/([a-zA-Z])/) && password.match(/([0-9])/)) strength += 1
    // If it has one special character, increase strength value.
    if (password.match(/([!,%,&,@,#,$,^,*,?,_,~])/)) strength += 1
    // If it has two special characters, increase strength value.
    if (password.match(/(.*[!,%,&,@,#,$,^,*,?,_,~].*[!,%,&,@,#,$,^,*,?,_,~])/)) strength += 1
    // Calculated strength value, we can return messages
    // If value is less than 2
    if (strength < 2) {
        $('#result').removeClass()
        $('#result').addClass('weak')
        return 'Weak'
    } else if (strength == 2) {// for God now changed to weak
        $('#result').removeClass()
        $('#result').addClass('weak')
        return 'Weak'
    } else {
        $('#result').removeClass()
        $('#result').addClass('strong')
        return 'Strong'
    }
    if (isValid == false)
        e.preventDefault();
}
$(document).ready(function () {
    $('.parallax').scrolly({ bgParallax: true });
});


$("#newsContent").heliumSlider({
    paneFade: true,
    paneXOffset: [50, 50, 50],
    paneYOffset: [50, 50, 50],
    paneDelay: [200, 200, 200],
    paneSpeed: [500, 500, 500],
    mainFadeIn: false,
    mainFadeOut: false,
    speed: 500,
    autoPlay: 5000,
    pauseControls: true,
    autoStopSlide: false,
    autoStopLoop: false,  // stop auto play after looping autoplay this many times: number or false
    autoStopPause: false,   // when autoplay ends, pause and keep controls available.
    pauseOnHover: true,
    pauseOnFocus: true,
    easing: 'linear'
});