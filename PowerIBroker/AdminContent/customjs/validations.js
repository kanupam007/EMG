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
            else {

            }
        });


        if (isValid == false)
            e.preventDefault();
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
    
});




//file preview before upload
function showfilepreview(input, type) {
    
    var FileUploadPath = input.value;
    var Extension = FileUploadPath.substring(
                FileUploadPath.lastIndexOf('.') + 1).toLowerCase();

    if (input.files && input.files[0]) {
        if (input.files[0].size > 1048576) {
            imageChanged = false;
            $(input).removeClass('custom-input-success').addClass('custom-input-error');
            alert("Image Size should not exceed 1 MB. Please try again");
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
                        
                        case "agentProfile":
                            if (width >= 221 && height >= 221) {
                                $("#AGImage").val(input.files[0].name);

                            }
                            else {
                                $(input).removeClass('custom-input-success').addClass('custom-input-error');
                                alert("Image resolution should be 129x100 (width:129,height:100)")
                                return false;
                            }
                            return false;


                        case "agentlogo":
                            if (width >= 129 && height >= 14) {
                                $("#AGLogoImage").val(input.files[0].name);

                            }
                            else {
                                $(input).removeClass('custom-input-success').addClass('custom-input-error');
                                alert("Logo resolution should be 129x14 (width:129,height:14)")
                                return false;
                            }
                            return false;
                        case "adminimage":
                            if (width >= 160 && height >= 160) {
                               
                                $(input).prev().prev("img").attr("src", $(img).attr('src'));
                            }
                            else {
                                $(input).removeClass('custom-input-success').addClass('custom-input-error');
                                alert("Logo resolution should be 160x160 (width:160,height:160)")
                                return false;
                            }
                            return false;

                        case "BannerImage":
                            if (width == 1903 && height == 363) {

                                $(input).prev().prev("img").attr("src", $(img).attr('src'));
                            }
                            else {
                                $(input).removeClass('custom-input-success').addClass('custom-input-error');
                                alert("Logo resolution should be 1903x363 (width:1903,height:363)")
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
            alert("Only .jpg, .jpeg & .png formats are supported ")
        }
    }
}

function uploadProfilePic(id) {
   
    var fileUpload = $("#"+id).get(0);
    var files = fileUpload.files;
    var data = new FormData();
    for (var i = 0; i < files.length; i++) {
        data.append(files[i].name, files[i]);
    }
   
   
    $.ajax({
      
        type: 'POST',
        processData: false,
        contentType: false,
        url: '/Agent/Profile/UploadProfilePicture',
        data: data,
        success: function (response) {
            ;
            if (response == "redirectologin")
            {
                window.location='/home/index';
                
            }
            if (response != "error") {
                $("#" + id).parent().prev().attr("src", "/Areas/Agent/Content/Upload/AgentImageDashboard_Original/" + response);
                $("#layOutAgentImg").attr("src", "/Areas/Agent/Content/Upload/AgentImageDashboard4747/" + response);
            }
            else {
                alert("Erro occurred")
            }
        }
    });
}

function uploadProfilePicture(input) {
    var FileUploadPath = input.value;
    var Extension = FileUploadPath.substring(
                FileUploadPath.lastIndexOf('.') + 1).toLowerCase();

    if (input.files && input.files[0]) {
        if (input.files[0].size > 1048576) {
            imageChanged = false;
            $(input).removeClass('custom-input-success').addClass('custom-input-error');
            alert("Image Size should not exceed 1 MB. Please try again");
         
            return false;
        }
        if (Extension == "jpeg" || Extension == "jpg" || Extension == "png") {

            uploadProfilePic($(input).attr("id"));
          
        }
        else {
            $(input).removeClass('custom-input-success').addClass('custom-input-error');
            alert("Only .jpg, .jpeg & .png formats are supported")
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
            alert("Video Size should not exceed 10 MB. Please try again");
            return false;
        }
        if (Extension == format ) {

            $("#Video").val(input.files[0].name);
        }
        else {
            alert("Only ."+format+" format is supported ")
        }
    }
}
var loaded = false;
$(function () {
    
    $('#Country').change(function () {
        
        if ($(this).val() != "") {
            
            var URL = "/Broker/Managelocation/BindStateByCountry";
            $.getJSON(URL, { 'CountryID': $('#Country').val(), 'BrokerId': $('#BrokerId').val() }, function (data) {

                var items = '<option>--Select--</option>';
                $.each(data, function (i, state) {
                    items += "<option value='" + state.ID + "'>" + state.State + "</option>";
                });
              
                
                if (!loaded) {
                    $('#State').html(items);

                } else {
                    loaded = false;
                }
                    
                
            });
        }
        else
            $('#State').html('').append("<option>--Select--</option>");
        $('#State').next('span.holder').text('--Select--');
    });
})



//----------------admin cms management-----------------//