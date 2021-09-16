$(document).ready(function () {
    setNavigation();


    //Excute sql query and export data
    $('#SubmitSqlQuery,#ExportSqlQuery').click(function (e) {

        var isValid = true;
        $('#txtQuery').each(function () {
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

    ///////////////////////2 Decimal Allowed/////////////////////////////////////
    $('#EmployeeValue').keypress(function (event) {
        var $this = $(this);
        if ((event.which != 46 || $this.val().indexOf('.') != -1) &&
           ((event.which < 48 || event.which > 57) &&
           (event.which != 0 && event.which != 8))) {
            event.preventDefault();
        }

        var text = $(this).val();
        if ((event.which == 46) && (text.indexOf('.') == -1)) {
            setTimeout(function () {
                if ($this.val().substring($this.val().indexOf('.')).length > 3) {
                    $this.val($this.val().substring(0, $this.val().indexOf('.') + 3));
                }
            }, 1);
        }

        if ((text.indexOf('.') != -1) &&
            (text.substring(text.indexOf('.')).length > 2) &&
            (event.which != 0 && event.which != 8) &&
            ($(this)[0].selectionStart >= text.length - 2)) {
            event.preventDefault();
        }
    });

    $('#EmployeeValue').bind("paste", function (e) {
        var text = e.originalEvent.clipboardData.getData('Text');
        if ($.isNumeric(text)) {
            if ((text.substring(text.indexOf('.')).length > 3) && (text.indexOf('.') > -1)) {
                e.preventDefault();
                $(this).val(text.substring(0, text.indexOf('.') + 3));
            }
        }
        else {
            e.preventDefault();
        }
    });

});
function setNavigation() {
    var path = window.location.pathname;
    path = path.replace(/\/$/, "");
    path = decodeURIComponent(path);
    $(".treeview  a").each(function () {
        var href = $(this).attr('href');
        if (path.substring(0, href.length) === href) {

            $(this).closest('li').addClass('active');
            $(this).closest('.treeview').addClass('active');
        }
    });
}

function isNumber(evt) {
    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        return false;
    }
    return true;
}

// Delete Home Page Banner Image
$(document).on('click', '.BannerImageHome', function (evt) {
   
    var Id = $(this).attr('data-id');
    swal({
        title: "",
        text: "Do you want to delete the image?",
        //type: "warning",
        showCancelButton: true,
        confirmButtonClass: "#f27474",
        confirmButtonText: "Yes",
        cancelButtonText: "Cancel",
        closeOnConfirm: true,
        closeOnCancel: true
    },
function (isConfirm) {
    if (isConfirm) {
        
        $.ajax({
            type: "POST",
            url: "/Admin/DeleteHomePageImage",
            data: "{'Id':'" + Id + "'}",
            contentType: "application/json; charset=utf-8",
            success: function (response) {
        
                if (response == true) {
                    location.reload();
                }
                else {
                    toastr["error"]("Some error occured while deleting image.", "Error !");
                }
            },
            failure: function (msg) {
                alert(msg);
            }
        });

    }
    else {

    }
});

});
// End Delete Home Page Banner Image

// Delete ContactUs Page Banner Image
$(document).on('click', '.BannerImageContactUs', function (evt) {

    var Id = $(this).attr('data-id');
    swal({
        title: "",
        text: "Do you want to delete the image?",
        //type: "warning",
        showCancelButton: true,
        confirmButtonClass: "#f27474",
        confirmButtonText: "Yes",
        cancelButtonText: "Cancel",
        closeOnConfirm: true,
        closeOnCancel: true
    },
function (isConfirm) {
    if (isConfirm) {
   
        $.ajax({
            type: "POST",
            url: "/Admin/DeleteContactUsPageImage",
            data: "{'Id':'" + Id + "'}",
            contentType: "application/json; charset=utf-8",
            success: function (response) {
           
                if (response == true) {
                    location.reload();
                }
                else {
                    toastr["error"]("Some error occured while deleting image.", "Error !");
                }
            },
            failure: function (msg) {
                alert(msg);
            }
        });

    }
    else {

    }
});

});
// End Delete ContactUs Page Banner Image


// Delete Aboutus Page Banner Image
$(document).on('click', '.BannerImageAboutUs', function (evt) {
 
    var Id = $(this).attr('data-id');
    var imageType = $(this).attr('data-image');
    swal({
        title: "",
        text: "Do you want to delete the image?",
        //type: "warning",
        showCancelButton: true,
        confirmButtonClass: "#f27474",
        confirmButtonText: "Yes",
        cancelButtonText: "Cancel",
        closeOnConfirm: true,
        closeOnCancel: true
    },
function (isConfirm) {
    if (isConfirm) {
  
        $.ajax({
            type: "POST",
            url: "/Admin/DeleteAboutUsPageImage",
            data: "{'Id':'" + Id + "','ImageType':'" + imageType + "'}",
            contentType: "application/json; charset=utf-8",
            success: function (response) {
         
                if (response == true) {
                    location.reload();
                }
                else {
                    toastr["error"]("Some error occured while deleting image.", "Error !");
                }
            },
            failure: function (msg) {
                alert(msg);
            }
        });

    }
    else {

    }
});

});
// End Delete ContactUs Page Banner Image
