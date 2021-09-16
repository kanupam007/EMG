//----------------admin cms management-----------------//

//saving homepage content
$(document).ready(function () {

    $('#btnAdminLogin').click(function (e) {
      
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
        //    $("#CaptchaAdminLogin").click();
        //}
        if (isValid) {
            //var response = grecaptcha.getResponse();
            //if (response.length == 0) {
            //    isValid = false;
            //    alert('Please check the checkbox on reCAPTCHA');
            //}
            //else {
            //    //$("#CaptchaAdminLogin").click();
       $("#AdminLoginformsubmit").submit();
            //}
        }

        if (isValid == false)
            e.preventDefault();
        else {
            var culturename = window.navigator.languages[0];
            $('#CultureName').val(culturename);

        }
    });
    $('#btnSaveHomePageContent').click(function (e) {
        
        var isValid = true;
        $('#Heading,#HeaderText').each(function () {
            if ($.trim($(this).val()) == '') {
                isValid = false;
                $(this).removeClass('custom-input-success').addClass('custom-input-error');
                $(this).keypress(function () {
                    $(this).removeClass('custom-input-error').addClass('custom-input-success');
                });
            }
            else {
                
            }
        });
        if ($('#Banner').val() == '') {
            isValid = false;
            $("#file").removeClass('custom-input-success').addClass('custom-input-error');
            $("#file").change(function () {
                $(this).removeClass('custom-input-error').addClass('custom-input-success');
            });
        }
        if (isValid == false)
            e.preventDefault();
    });


    //saving homepage client banner
    $('#btnSaveHomePageClientBanner').click(function (e) {
        
        var isValid = true;
        if ($('#Banner').val() == '') {
            isValid = false;
            $("#file").removeClass('custom-input-success').addClass('custom-input-error');
            $("#file").change(function () {
                $(this).removeClass('custom-input-error').addClass('custom-input-success');
            });
        }
        if (isValid == false)
            e.preventDefault();
    });

    //saving homepage 
    $('#btnSaveBenefitsPageContent').click(function (e) {
        
        var isValid = true;
        //$('#LeftContentForBroker,#RightContentForBroker').each(function () {
        //    var lhtml = $(this).val();
        //    var rhtml = $(this).val();
        //    if ($.trim($(lhtml).text()) == '') {
        //        isValid = false;
        //        $(this).prev("div.note-editor").removeClass('custom-input-success').addClass('custom-input-error');
        //        $(this).prev("div.note-editor").children("div.note-editable").keypress(function () {
        //            
        //            $(this).parent("div.note-editor").removeClass('custom-input-error').addClass('custom-input-success');
        //        });
        //    }
        //    else {

        //    }
        //    if ($.trim($(rhtml).text()) == '') {
        //        isValid = false;
        //        $(this).prev("div.note-editor").removeClass('custom-input-success').addClass('custom-input-error');
        //        $(this).prev("div.note-editor").children("div.note-editable").keypress(function () {
        //            
        //            $(this).parent("div.note-editor").removeClass('custom-input-error').addClass('custom-input-success');
        //        });
        //    }
        //    else {

        //    }
        //});
        if ($('#BackgroundImage').val() == '') {
            isValid = false;
            $("#file").addClass('custom-input-error');
            $("#file").change(function () {
                $(this).removeClass('custom-input-error').addClass('custom-input-success');
            });
        }
        if (isValid == false)
            e.preventDefault();
    });
   
    $('#btnSaveRequestDemoPageContent').click(function (e) {
        
        var isValid = true;
        $('#Title').each(function () {
            if ($.trim($(this).val()) == '') {
                isValid = false;
                $(this).removeClass('custom-input-success').addClass('custom-input-error');
                $(this).keypress(function () {
                    $(this).removeClass('custom-input-error').addClass('custom-input-success');
                });
            }
            else {

            }
        });
        if ($('#BackgroundImage').val() == '') {
            isValid = false;
            $("#file").removeClass('custom-input-success').addClass('custom-input-error');
            $("#file").change(function () {
                $(this).removeClass('custom-input-error').addClass('custom-input-success');
            });
        }
        if ($('#Video').val() == '') {
            isValid = false;
            $("#video").removeClass('custom-input-success').addClass('custom-input-error');
            $("#video").change(function () {
                $(this).removeClass('custom-input-error').addClass('custom-input-success');
            });
        }
        if (isValid == false)
            e.preventDefault();
    });
    $('#btnSaveFeaturesContent,#btnSaveHomePageTopContent').click(function (e) {
        
        var isValid = true;
        $('#Title,#Description').each(function () {
            if ($.trim($(this).val()) == '') {
                isValid = false;
                $(this).removeClass('custom-input-success').addClass('custom-input-error');
                $(this).keypress(function () {
                    $(this).removeClass('custom-input-error').addClass('custom-input-success');
                });
            }
            else {

            }
        });
        if ($('#Image').val() == '') {
            isValid = false;
            $("#file").removeClass('custom-input-success').addClass('custom-input-error');
            $("#file").change(function () {
                $(this).removeClass('custom-input-error').addClass('custom-input-success');
            });
        }
        
        if (isValid == false)
            e.preventDefault();
    });

    $('#btnSavePricingPageContent').click(function (e) {
        
        var isValid = true;
        $('#Title,#Description,#LeftContent,#RightContent').each(function () {
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

    $('#btnSaveAboutUsPageContent').click(function (e) {
        
        var isValid = true;
        $('#HeaderTitle').each(function () {
            if ($.trim($(this).val()) == '') {
                isValid = false;
                $(this).removeClass('custom-input-success').addClass('custom-input-error');
                $(this).keypress(function () {
                    $(this).removeClass('custom-input-error').addClass('custom-input-success');
                });
            }
            else {

            }
        });
        if ($('#HeaderBanner').val() == '') {
            isValid = false;
            $("#file").removeClass('custom-input-success').addClass('custom-input-error');
            $("#file").change(function () {
                $(this).removeClass('custom-input-error').addClass('custom-input-success');
            });
        }
        $('#BottomLeftSectionTitle,#BottomMiddleSectionTitle,#BottomRightSectionTitle').each(function () {
            if ($.trim($(this).val()) == '') {
                isValid = false;
                $(this).removeClass('custom-input-success').addClass('custom-input-error');
                $(this).keypress(function () {
                    $(this).removeClass('custom-input-error').addClass('custom-input-success');
                });
            }
            else {

            }
        });


        if ($('#BottomLeftSectionImage').val() == '') {
            isValid = false;
            $("#abtUsBtmLeftBanner").removeClass('custom-input-success').addClass('custom-input-error');
            $("#abtUsBtmLeftBanner").change(function () {
                $(this).removeClass('custom-input-error').addClass('custom-input-success');
            });
        }
        if ($('#BottomMiddleSectionImage').val() == '') {
            isValid = false;
            $("#abtUsBtmMiddleBanner").removeClass('custom-input-success').addClass('custom-input-error');
            $("#abtUsBtmMiddleBanner").change(function () {
                $(this).removeClass('custom-input-error').addClass('custom-input-success');
            });
        }
        if ($('#BottomRightSectionImage').val() == '') {
            isValid = false;
            $("#abtUsBtmRightBanner").removeClass('custom-input-success').addClass('custom-input-error');
            $("#abtUsBtmRightBanner").change(function () {
                $(this).removeClass('custom-input-error').addClass('custom-input-success');
            });
        }

        if (isValid == false)
            e.preventDefault();
    });

    $('#btnSaveFaqContent').click(function (e) {
        
        var isValid = true;
        $('#Question,#Answer').each(function () {
            if ($.trim($(this).val()) == '') {
                isValid = false;
                $(this).removeClass('custom-input-success').addClass('custom-input-error');
                $(this).keypress(function () {
                    $(this).removeClass('custom-input-error').addClass('custom-input-success');
                });
            }
            else {

            }
        });
        if ($('#Banner').val() == '') {
            isValid = false;
            $("#file").removeClass('custom-input-success').addClass('custom-input-error');
            $("#file").change(function () {
                $(this).removeClass('custom-input-error').addClass('custom-input-success');
            });
        }
       
        if (isValid == false)
            e.preventDefault();
    });
    
    $('#btnSaveOtherPageContent').click(function (e) {
        
        var isValid = true;
        $('#Title').each(function () {
            if ($.trim($(this).val()) == '') {
                isValid = false;
                $(this).removeClass('custom-input-success').addClass('custom-input-error');
                $(this).keypress(function () {
                    $(this).removeClass('custom-input-error').addClass('custom-input-success');
                });
            }
            else {

            }
        });
        if ($('#Banner').val() == '') {
            isValid = false;
            $("#file").removeClass('custom-input-success').addClass('custom-input-error');
            $("#file").change(function () {
                $(this).removeClass('custom-input-error').addClass('custom-input-success');
            });
        }

        if (isValid == false)
            e.preventDefault();
    });

    $('#btnSaveContactUsPageContent').click(function (e) {
        
        var isValid = true;
        $('#Title,#Address,#Email').each(function () {
            if ($.trim($(this).val()) == '') {
                isValid = false;
                $(this).removeClass('custom-input-success').addClass('custom-input-error');
                $(this).keypress(function () {
                    $(this).removeClass('custom-input-error').addClass('custom-input-success');
                });
            }
            else {

            }
        });
        if ($('#Banner').val() == '') {
            isValid = false;
            $("#file").removeClass('custom-input-success').addClass('custom-input-error');
            $("#file").change(function () {
                $(this).removeClass('custom-input-error').addClass('custom-input-success');
            });
        }
        if (isValid == false)
            e.preventDefault();
    });
    
    $('#btnSaveSocialMediaLink').click(function (e) {
        
        var isValid = true;
        $('#Title,#Link').each(function () {
            if ($.trim($(this).val()) == '') {
                isValid = false;
                $(this).removeClass('custom-input-success').addClass('custom-input-error');
                $(this).keypress(function () {
                    $(this).removeClass('custom-input-error').addClass('custom-input-success');
                });
            }
            else {

            }
        });
        if (isValid == false)
            e.preventDefault();
    });

    $('#btnSaveNewsContent').click(function (e) {
        
        var isValid = true;
        $('#Title,#Description').each(function () {
            if ($.trim($(this).val()) == '') {
                isValid = false;
                $(this).removeClass('custom-input-success').addClass('custom-input-error');
                $(this).keypress(function () {
                    $(this).removeClass('custom-input-error').addClass('custom-input-success');
                });
            }
            else {

            }
        });
        if (isValid == false)
            e.preventDefault();
    });

    $('#btnSavePricingPageContent').click(function (e) {
        
        var isValid = true;
        $('#Title,#Description,#LeftContent,#RightContent').each(function () {
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
    $('#btnAddInsurancePlan,#btnEditInsurancePlan').click(function (e) {
        
        var isValid = true;
        $('#ProviderCompanyId,#PlanId,#PlanTypeId,#AveragePremiumForEmployee,#AveragePremiumForEmployee_Spouse,#AveragePremiumForEmployee_Children,#AveragePremiumForEmployee_Family').each(function () {
            if ($.trim($(this).val()) == '') {
                isValid = false;
                $(this).removeClass('custom-input-success').addClass('custom-input-error');
                $(this).next(".validateMsg").remove();
                $(this).after("<span class=\"validateMsg\">" + $(this).attr("data-val-required").replace("ProviderCompanyId", "ProviderCompany") + "</span>")
                $(this).keypress(function () {
                    $(this).removeClass('custom-input-error').addClass('custom-input-success');
                    $(this).next(".validateMsg").remove();
                });
                if ($(this).prop("tagName").toLowerCase() == "select") {
                    $(this).change(function () {
                       
                        $(this).removeClass('custom-input-error').addClass('custom-input-success');
                        $(this).next(".validateMsg").remove();
                    });
                }
            }

        });

        if (isValid == false)
            e.preventDefault();
    });

    $('#btnUploadCompanyPremium').click(function (e) {
        
        var isValid = true;
        if ($('#FileName').val() == '') {
            isValid = false;
            $("#File").removeClass('custom-input-success').addClass('custom-input-error');
            $("#File").change(function () {
                $(this).removeClass('custom-input-error').addClass('custom-input-success');
            });
        }
        if (isValid == false)
            e.preventDefault();
    });

  
    
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
    if (strength == 0) {
        $('#result').removeClass()
        $('#result').addClass('Provide')
        return 'Please provide a password'
    }
    if (strength < 2 && strength > 0) {
        $('#result').removeClass()
        $('#result').addClass('weak')
        return 'Weak'
    } else if (strength == 2) {
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


//file preview before upload
function showfilepreview(input,type) {
    var FileUploadPath = input.value;
    var Extension = FileUploadPath.substring(
                FileUploadPath.lastIndexOf('.') + 1).toLowerCase();

    if (input.files && input.files[0]) {
        if (input.files[0].size > 1048576) {
            imageChanged = false;
            $(input).removeClass('custom-input-success').addClass('custom-input-error');
            //alert("Image Size should not exceed 1 MB. Please try again");
            toastr['error']("Image Size should not exceed 1 MB. Please try again", "Error !");
            return false;
        }
        if (Extension == "jpeg" || Extension == "jpg" || Extension == "png") {
                      

            var reader = new FileReader();
            imageChanged = true;
            reader.onload = function (e) {
               // $('#imgSliderDiv').fadeOut(function () {

                    var img = document.createElement('img');
                    $(img).attr('src', e.target.result);
                    var width = img.naturalWidth;
                    var height = img.naturalHeight;
                    
                    switch (type) {
                        case "homepageBanner":
                            if (width >= 1300 && height >= 830) {
                                $(input).parent().next().children().attr('src', e.target.result);
                                $("#Banner").val(input.files[0].name);
                                $(input).removeClass('custom-input-error').addClass('custom-input-success');
                            }
                            else {
                                $(input).removeClass('custom-input-success').addClass('custom-input-error');
                                //alert("Banner resolution should be 1300x830 (width:1300,height:830")
                                toastr['error']("Banner resolution should be 1300x830 (width:1300,height:830)", "Error !");
                                return false;
                            }
                            return false;
                        case "abtUsHeaderBanner":
                            if (width >= 1300 && height >= 830) {
                                $(input).parent().next().children().attr('src', e.target.result);
                                $("#HeaderBanner").val(input.files[0].name);
                                $(input).removeClass('custom-input-error').addClass('custom-input-success');
                            }
                            else {
                                $(input).removeClass('custom-input-success').addClass('custom-input-error');
                                //alert("Banner resolution should be 1300x830 (width:1300,height:830")
                                toastr['error']("Banner resolution should be 1300x830 (width:1300,height:830)", "Error !");
                                return false;
                            }
                            return false;
                        case "clientBanner":
                            if (width >= 100 && height >= 28) {
                                $(input).parent().next().children().attr('src', e.target.result);
                                $("#Banner").val(input.files[0].name);
                                $(input).removeClass('custom-input-error').addClass('custom-input-success');
                            }
                            else {
                                $(input).removeClass('custom-input-success').addClass('custom-input-error');
                                //alert("Banner resolution should be 100x28 (width:1300,height:28")
                                toastr['error']("Banner resolution should be 100x28 (width:1300,height:28)", "Error !");
                                return false;
                            }
                            return false;
                        case "benefitsBanner":
                            if (width >= 1500 && height >= 603) {
                                $(input).parent().next().children().attr('src', e.target.result);
                                $("#BackgroundImage").val(input.files[0].name);
                                $(input).removeClass('custom-input-error').addClass('custom-input-success');
                            }
                            else {
                                $(input).removeClass('custom-input-success').addClass('custom-input-error');
                                //alert("Banner resolution should be 1500x603 (width:1500,height:603)")
                                toastr['error']("Banner resolution should be 1500x603 (width:1500,height:603)", "Error !");
                                return false;
                            }
                            return false;
                        case "featuresBanner":
                            if (width >= 70 && height >= 70) {
                                $(input).parent().next().children().attr('src', e.target.result);
                                $("#Image").val(input.files[0].name);
                                $(input).removeClass('custom-input-error').addClass('custom-input-success');
                            }
                            else {
                                $(input).removeClass('custom-input-success').addClass('custom-input-error');
                                //alert("Image resolution should be 70x70 (width:70,height:70)")
                                toastr['error']("Image resolution should be 70x70 (width:70,height:70)", "Error !");
                                return false;
                            }
                            return false;
                        case "topContentBanner":
                            if (width >= 60 && height >= 66) {
                                $(input).parent().next().children().attr('src', e.target.result);
                                $("#Image").val(input.files[0].name);
                                $(input).removeClass('custom-input-error').addClass('custom-input-success');
                            }
                            else {
                                $(input).removeClass('custom-input-success').addClass('custom-input-error');
                                //alert("Image resolution should be 60x66 (width:60,height:66)")
                                toastr['error']("Image resolution should be 60x66 (width:60,height:66)", "Error !");
                                return false;
                            }
                            return false;
                        case "requestDemoBanner":
                            if (width >= 1500 && height >= 591) {
                                $(input).parent().next().children().attr('src', e.target.result);
                                $("#BackgroundImage").val(input.files[0].name);

                            }
                            else {
                                $(input).removeClass('custom-input-success').addClass('custom-input-error');
                                //alert("Banner resolution should be 1500x591 (width:1500,height:591)")
                                toastr['error']("Banner resolution should be 1500x591 (width:1500,height:591)", "Error !");
                                return false;
                            }
                            return false;
                        case "abtUsBtmLeftBanner":
                            if (width >= 370 && height >= 225) {
                                $(input).parent().next().children().attr('src', e.target.result);
                                $("#BottomLeftSectionImage").val(input.files[0].name);

                            }
                            else {
                                $(input).removeClass('custom-input-success').addClass('custom-input-error');
                                //alert("Banner resolution should be 272x227 (width:272,height:227)")
                                toastr['error']("Banner resolution should be 272x227 (width:272,height:227)", "Error !");
                                return false;
                            }
                            return false;
                        case "abtUsBtmMiddleBanner":
                            if (width >= 370 && height >= 225) {
                                $(input).parent().next().children().attr('src', e.target.result);
                                $("#BottomMiddleSectionImage").val(input.files[0].name);

                            }
                            else {
                                $(input).removeClass('custom-input-success').addClass('custom-input-error');
                                //alert("Banner resolution should be 272x227 (width:272,height:227)")
                                toastr['error']("Banner resolution should be 272x227 (width:272,height:227)", "Error !");
                                return false;
                            }
                            return false;
                        case "abtUsBtmRightBanner":
                            if (width >= 370 && height >= 225) {
                                $(input).parent().next().children().attr('src', e.target.result);
                                $("#BottomRightSectionImage").val(input.files[0].name);

                            }
                            else {
                                $(input).removeClass('custom-input-success').addClass('custom-input-error');
                                //alert("Banner resolution should be 272x227 (width:272,height:227)")
                                toastr['error']("Banner resolution should be 272x227 (width:272,height:227)", "Error !");
                                return false;
                            }
                            return false;
                        case "faqPageBanner":
                            if (width >= 1500 && height >= 406) {
                                $(input).parent().next().children().attr('src', e.target.result);
                                $("#Banner").val(input.files[0].name);

                            }
                            else {
                                $(input).removeClass('custom-input-success').addClass('custom-input-error');
                                //alert("Banner resolution should be 1500x406 (width:1500,height:406)")
                                toastr['error']("Banner resolution should be 1500x406 (width:1500,height:406)", "Error !");
                                return false;
                            }
                            return false;
                        case "contactUsBanner":
                            if (width >= 1500 && height >= 406) {
                                $(input).parent().next().children().attr('src', e.target.result);
                                $("#Banner").val(input.files[0].name);

                            }
                            else {
                                $(input).removeClass('custom-input-success').addClass('custom-input-error');
                                //alert("Banner resolution should be 1500x406 (width:1500,height:406)")
                                toastr['error']("Banner resolution should be 1500x406 (width:1500,height:406)", "Error !");
                                return false;
                            }
                            return false;
                        case "othePageBanner":
                            if (width >= 1500 && height >= 406) {
                                $(input).parent().next().children().attr('src', e.target.result);
                                $("#Banner").val(input.files[0].name);

                            }
                            else {
                                $(input).removeClass('custom-input-success').addClass('custom-input-error');
                                //alert("Banner resolution should be 1500x406 (width:1500,height:406)")
                                toastr['error']("Banner resolution should be 1500x406 (width:1500,height:406)", "Error !");
                                return false;
                            }
                            return false;
                        case "planDocument":
                            if (input.files[0].size <= 1024) {
                                $(input).parent().next().children().attr('src', e.target.result);
                                $("#Banner").val(input.files[0].name);

                            }
                            else {
                                $(input).removeClass('custom-input-success').addClass('custom-input-error');
                                //alert("Banner resolution should be 1500x406 (width:1500,height:406)")
                                toastr['error']("Banner resolution should be 1500x406 (width:1500,height:406)", "Error !");
                                return false;
                            }
                            return false;
                        
                    }
                    
               // });
               // $('#imgSliderDiv').fadeIn();
            }

            reader.readAsDataURL(input.files[0]);
        }
        else {
            $(input).removeClass('custom-input-success').addClass('custom-input-error');
            //alert("Only .jpg, .jpeg & .png formats are supported ")
            toastr['error']("Only .jpg, .jpeg & .png formats are supported", "Error !");
        }
    }
}

//check video file format
function checkVideoFile(input,format) {
    var FileUploadPath = input.value;
    var Extension = FileUploadPath.substring(
                FileUploadPath.lastIndexOf('.') + 1).toLowerCase();

    if (input.files && input.files[0]) {
        if (input.files[0].size > (1048576*10)) {
            imageChanged = false;
            //alert("Video Size should not exceed 10 MB. Please try again");
            toastr['error']("Video Size should not exceed 10 MB. Please try again", "Error !");
            return false;
        }
        if (Extension == format ) {

            $("#Video").val(input.files[0].name);
        }
        else {
            //alert("Only ."+format+" format is supported ")
            toastr['error']("Only ." + format + " format is supported", "Error !");
        }
    }
}

//check document
function checkDocumentFile(input, format) {
    
    var FileUploadPath = input.value;
    var Extension = FileUploadPath.substring(
                FileUploadPath.lastIndexOf('.') + 1).toLowerCase();

    if (input.files && input.files[0]) {
        if (input.files[0].size > (1048576 * 2)) {
            imageChanged = false;
            //alert("Document size should not exceed 2 MB. Please try again");
            toastr['error']("Document size should not exceed 2 MB. Please try again", "Error !");
            return false;
        }
        if (Extension == format) {
            $("#PlanDocumentName,#FileName").val(input.files[0].name);
        }
        else {
            //alert("Only ." + format + " format is supported ")
            toastr['error']("Only ." + format + " format is supported", "Error !");
            $("#File").removeClass('custom-input-success').addClass('custom-input-error');
        }
    }
}



//----------------admin cms management-----------------//

//------------ Managclient Page js Start--------------//
function Resendemail(id) {


    swal({
        title: "Are you sure?",
        text: "Want to Resend-email?",
      //  type: "warning",
        showCancelButton: true,
        //confirmButtonColor: "#DD6B55",
        //confirmButtonText: "Yes",
        //cancelButtonText: "Cancel",
        closeOnConfirm: false,
        showLoaderOnConfirm: true
    },
   function () {
       $.ajax({
           type: "POST",
           url: "/Admin/ResendEmail",
           data: "{'itemId':'" + id + "'}",
           contentType: "application/json; charset=utf-8",
           success: function (response) {

               if (response == "Success") {
                   swal("Success!", "Mail sent successfully", "success");

               }
               else {
                   swal("Error!", "Some error occurred", "error");
                   return false;

               }
           }
       });
   })
}
//------------ Managclient Page js END--------------//