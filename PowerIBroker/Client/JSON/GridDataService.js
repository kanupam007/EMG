
//var app = angular.module("gridData", ['ngGrid']);
function GridDataService() {
    this.GetAllGridData = function ($http,companyID, successCallBack) {
        
        $http.get("/Admin/ManageOpenEnrollment1?CID="+companyID).success(function (data) {
            
            successCallBack(data);
        });
    }

    this.GetCompanyName = function ($http, successCallBack) {
        $http.get("/Admin/GetCompanyDropdown").success(function (data) {
            successCallBack(data);
        });
    }
};


