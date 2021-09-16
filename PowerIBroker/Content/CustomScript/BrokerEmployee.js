$('.company-information').empty();

var CData = removeDuplicates(EmployeeData, 'CompanyId');
if (CData.length > 0) {
    
    $('.company-information').append('<option value="0">Choose One Company</option>');
    $(CData).each(function (index, element) {
        $('.company-information').append('<option value="' + element.CompanyId + '">' + element.CCompanyName + '</option>');
        
    })
    
}
function AddMasking(number) {
    return number.replace(/^(\d{3})(\d{3})(\d{4}).*/, '($1) $2-$3');
}
function BindAllEmployees(EmpData) {
    EmpData = removeDuplicates(EmpData, 'Email');
    $('#txtmsg').text('').hide();
    if (($('.company-information').val() == "0" || $('.company-information').val() == "") && $('.txtSearch').val() == "") {
        $('#txtmsg').text('Please Select Company for display Employees').show();
    }
    else if (EmpData.length <= 0) {
        $('#txtmsg').text('No Result Found!! Please Change Your Filter Setting').show();
    }
    else {
        var html = '';
        var i = 1;
        $(EmpData).each(function (index, element) {
            var classname = i <= 3 ? "mg-sm-t-0" : "";
            var empimage = (element.EmpImage != null && element.EmpImage != "" ? "https://www.enrollmygroup.com/Areas/Company/Content/Upload/Employee/CompanyEmpThumb62x62/" + element.EmpImage : "https://www.enrollmygroup.com/Areas/Company/Content/images/user_smll_img.jpg")
            html += '<div class="col-sm-6 col-lg-4 mg-t-20 ' + classname + '">';
            html += '<div class="card-contact">';
            html += '<div class="tx-center">';
            html += '<a href=""><img src="' + empimage + '" class="card-img" alt=""></a>';
            html += '<h5 style="height: 20px;overflow: hidden;" class="mg-t-10 mg-b-5"><a href="" class="contact-name">' + (element.FirstName != null && element.FirstName != "" ? element.FirstName : "") + ' ' + (element.LastName != null && element.LastName != "" ? element.LastName : "") + '</a></h5>';
            html += '<p style="height: 16px;overflow: hidden;">' + (element.Title != null && element.Title != "" ? element.Title : "-------------------") + '</p>';
            html += '<p class="contact-social">';
            html += "<a style=\"cursor:pointer;\" onclick=\"EmployeeRedirect('/Company/Employee/GotoDashboard/" + element.EMPId + "','" + element.CompanyId+"')\"  class=\"deleteDashboard transition\"><img height=\"18px\" src=\"/Areas/Company/Content/images/dashboard.png\"  data-toggle=\"tooltip\" data-placement=\"right\" title=\"Dashboard\" class=\"actImg\"></a>";
            html += "<a style=\"cursor:pointer;\" onclick=\"EmployeeRedirect('/Company/Employee/MyBenefit/" + element.EMPId + "','" + element.CompanyId +"')\" class=\"deleteDashboard transition\"><img height=\"18px\" src=\"/Areas/Company/Content/images/Benefit.png\" alt=\"\" data-toggle=\"tooltip\" data-placement=\"right\" title=\"Enrollment\" class=\"actImg\"></a>";
            html += '<a href="javascript:void(0)" class="deleteDashboard transition"><img height="18px" src="/Areas/Company/Content/images/invite.png" alt="" data-toggle="tooltip" data-placement="right" title="Invite" class="actImg"></a>';
            html += '<a href="javascript:void(0)" data-empid="10138" class="deleteDashboard transition newhireinvite"><img height="18px" src="/Areas/Company/Content/images/ReopenNH.png" alt="" data-toggle="tooltip" data-placement="right" title="Reopen NH" class="actImg"></a>';
            html += '<a data-emp-id="10138" href="javascript:void(0)" class="delete transition"><img height="18px" src="/Areas/Company/Content/images/suspend.png" data-toggle="tooltip" data-placement="right" title="Suspend" alt="" class="actImg"></a>';
            html += "<a style=\"cursor:pointer;\" onclick=\"EmployeeRedirect('/Company/Employee/Terminate/" + element.EMPId + "','" + element.CompanyId +"')\"  class=\"deleteDashboardTerminate transition\"><img height=\"18px\" src=\"/Areas/Company/Content/images/terminate.png\" data-toggle=\"tooltip\" data-placement=\"right\" title=\"Terminate\" alt=\"\" class=\"actImg\"></a>";
            html += '</div>';
            html += '<p class="contact-item">';
            html += '<span>Phone:</span>';
            html += '<span>' + (element.Phone != null && element.Phone != "" ? AddMasking(element.Phone)  : "-----------") + '</span>';
            html += '</p>';
            html += '<p class="contact-item">';
            html += '<span>Mobile:</span>';
            html += '<span>' + (element.WorkPhone != null && element.WorkPhone != "" ? AddMasking(element.WorkPhone) : "-----------") + '</span>';
            html += '</p>';
            html += '<p class="contact-item">';
            html += '<span>Email:</span>';
            html += '<a href="" style="overflow:hidden;height:20px;width:80%;">' + (element.Email != null && element.Email != "" ? element.Email : "----") + '</a>';
            html += '</p>';
            html += '</div>';
            html += '</div>';
            i++;

        })

        
    }
    $('#EmployeeData').empty();
    $('#EmployeeData').append(html);
}
$(document).on('change', '.company-information', function () {
    var company = parseInt($(this).val());
    var CompanyData = EmployeeData.filter(function (item) { return item.CompanyId == company });
    var Division = removeDuplicates(CompanyData, 'DivisionId');
    var Location = removeDuplicates(CompanyData, 'ID');
    var Position = removeDuplicates(CompanyData, 'Title');
    $('.division-information').empty();
    if (Division.length > 0) {
        $('.division-information').append('<option value="0">Choose One Division</option>');
        $(Division).each(function (index, element) {
            if (element.DivisionId != null && element.DivisionName != null && element.DivisionId != "" && element.DivisionName != "") {
                $('.division-information').append('<option value="' + element.DivisionId + '">' + element.DivisionName + '</option>');
               
            }
            
        })
        
    }
   
    $('.location-information').empty();
    if (Location.length > 0) {
        $('.location-information').append('<option value="0">Choose One Location</option>');
        $(Location).each(function (index, element) {
            if ((element.ID != null && element.StateName != null) && (element.ID != "" && element.StateName != "")) {
                $('.location-information').append('<option value="' + element.ID + '">' + element.StateName + '</option>');
                
            }
        })
        
    }
    
    $('.position-information').empty();
    if (Position.length > 0) {
        $('.position-information').append('<option value="0">Choose One Position</option>');
        $(Position).each(function (index, element) {
            if (element.Title != null && element.Title!="") {
                $('.position-information').append('<option value="' + index + '">' + element.Title + '</option>');
                
            }
            
        })
        
    }
    Filter(CompanyData);
})
$(document).on('change', '.division-information', function () {
    var company = parseInt($('.company-information').val());
    var CompanyData = EmployeeData.filter(function (item) { return item.CompanyId == company });
    var Division = $(this).val();
    if (Division != "" && Division != "0") {
        CompanyData = CompanyData.filter(function (item) { return item.DivisionId == parseInt(Division) });
    }
    BindAllEmployees(CompanyData);
})
$(document).on('change', '.location-information', function () {
    var company = parseInt($('.company-information').val());
    var CompanyData = EmployeeData.filter(function (item) { return item.CompanyId == company });
    var Location = $(this).val();
    if (Location != "" && Location != "0") {
        CompanyData = CompanyData.filter(function (item) { return item.ID == parseInt(Location) });
    }
    BindAllEmployees(CompanyData);
})
$(document).on('change', '.position-information', function () {
    var company = parseInt($('.company-information').val());
    var CompanyData = EmployeeData.filter(function (item) { return item.CompanyId == company });
    var Position = $(this).val();
    if (Position != "" && Position != "0") {
        CompanyData = CompanyData.filter(function (item) { return item.Title == parseInt(Position) });
    }
    BindAllEmployees(CompanyData);
})
$(document).on('keyup', '.txtSearch', function () {
    var SearchValue = $(this).val().toLowerCase();
    /*var Data = EmployeeData.filter(function (item) {return (item.FirstName != null ? item.FirstName.indexOf(SearchValue) == 0 : false) || (item.LastName != null ? item.LastName.indexOf(SearchValue) == 0 : false) || (item.Email != null ? item.Email.indexOf(SearchValue) == 0 : false) || (item.Phone != null ? item.Phone.indexOf(SearchValue) == 0 : false) || (item.WorkPhone != null ? item.WorkPhone.indexOf(SearchValue) == 0 : false)});*/
    var Data = EmployeeData.filter(function (item) { return (item.FirstName != null ? item.FirstName.toLowerCase().lastIndexOf(SearchValue, 0) == 0 : false) || (item.LastName != null ? item.LastName.toLowerCase().lastIndexOf(SearchValue, 0) == 0 : false) || (item.Email != null ? item.Email.toLowerCase().lastIndexOf(SearchValue, 0) == 0 : false) || (item.Phone != null ? item.Phone.toLowerCase().lastIndexOf(SearchValue, 0) == 0 : false) || (item.WorkPhone != null ? item.WorkPhone.toLowerCase().lastIndexOf(SearchValue, 0) == 0 : false)});
    BindAllEmployees(Data);
})

function removeDuplicates(originalArray, prop) {
    var newArray = [];
    var lookupObject = {};

    for (var i in originalArray) {
        lookupObject[originalArray[i][prop]] = originalArray[i];
    }

    for (i in lookupObject) {
        newArray.push(lookupObject[i]);
    }
    return newArray;
}

function Filter(Data) {

    var Company = $('.company-information').val();
    var Division = $('.division-information').val();
    var Location = $('.location-information').val();
    var Position = $('.position-information').val();
    
    if (Company != "" && Company != "0") {
        Data = Data.filter(function (item) { return item.CompanyId == parseInt(Company) });
    }
    if (Division != "" && Division != "0") {
        Data = Data.filter(function (item) { return item.DivisionId == parseInt(Division) });
    }
    if (Location != "" && Location != "0") {
        Data = Data.filter(function (item) { return item.ID == parseInt(Location) });
    }
    if (Position != "" && Position != "0") {
        Data = Data.filter(function (item) { return item.Title == parseInt(Position) });
    }
    BindAllEmployees(Data);
}
function EmployeeRedirect(url,companyid) {
    window.location.href = '/' + location.pathname.split('/')[1].toLowerCase()+'/EmployeeRedirect?URL=' + url + '&CompanyId=' + companyid;
}
//function GetAllFIlter() {

//    var filteredValue = EmployeeData.filter(function (item) {return item.CompanyId == "Joe" &&});
//}
$(document).on('mouseover', '.actImg', function () {
    $(this).attr('src', $(this).attr('src').replace('.png', '-hover.png'));
})
$(document).on('mouseout', '.actImg', function () {
    $(this).attr('src', $(this).attr('src').replace('-hover.png', '.png'));
})