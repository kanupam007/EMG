var app = angular.module('gridData', ['ngTouch', 'ui.grid', 'ui.grid.expandable', 'ui.grid.selection', 'ui.grid.exporter', 'ui.grid.pinning', 'ui.grid.pagination']);

app.controller('MainCtrl', ['$scope', '$http', '$log', function ($scope, $http, $log) {

    var myService = new GridDataService();
    $scope.gridOptions = {
        enableGridMenu: true,
        expandableRowTemplate: 'expandableRowTemplate',
        expandableRowHeight: 150,
        enableFiltering: true,

        //enablePaging:true,
        paginationPageSizes: [10,20,30, 50, 75],
        paginationPageSize: 10,
        expandableRowScope: {
            subGridVariable: 'subGridScopeVariable'
        },

    }

  

   

    $scope.GetCompanyName = function () {
        myService.GetCompanyName($http, function (data) {
            $scope.companyList = data;
        });
    };
    $scope.GetCompanyName();

    
    // $scope.gridOptions.columnDefs = [
    //{ name: 'id' },
    //{ name: 'name' },
    //{ name: 'age' },
    //{ name: 'address.city' }
    // ];
    $http.get('https://cdn.rawgit.com/angular-ui/ui-grid.info/gh-pages/data/500_complex.json')
    .success(function (data) {
        ;

        for (i = 0; i < data.length; i++) {
            data[i].subGridOptions = {
                columnDefs: [{ name: "id", field: "id" }, { name: "Name", field: "name" }],
                data: data[i]
             
            }
        }
        
        $scope.gridOptions.data = data;
    });
    $scope.gridOptions.columnDefs = [{
        field: 'Description',
        displayName: 'Description'
    }, {
        field: 'StartDate',
        displayName: 'Start Date'
    }, {
        field: 'EndDate',
        displayName: 'End Date'
    }

    ]

    $scope.CompanyID = 0;
    $scope.CompanyChange = function (CompanyID) {
        $scope.CompanyID = CompanyID;
        myService.GetAllGridData($http, $scope.CompanyID, function (data) {
            ;
            for (i = 0; i < data.length; i++) {
                data[i].subGridOptions = {
                    columnDefs: [{ name: "ID", field: "ID" }, { name: "Description", field: "Description" }],
                    data: data[i].OpenEnroll
                }
            }
            
            $scope.gridOptions.data = data;

        });
    }
    //myService.GetAllGridData($http, 2, function (data) {
    //    for (i = 0; i < data.length; i++) {
    //        data[i].subGridOptions = {
    //            columnDefs: [{ name: "ID", field: "ID" }, { name: "Description", field: "Description" }],
    //            data: data
    //        }
    //    }
    //    
    //    $scope.gridOptions.data = data;

    //});
    $scope.gridOptions.onRegisterApi = function (gridApi) {
        
        $scope.gridApi = gridApi;
        
    };

    //$scope.expandAllRows = function () {
    //    $scope.gridApi.expandable.expandAllRows();
    //}

    //$scope.collapseAllRows = function () {
    //    $scope.gridApi.expandable.collapseAllRows();
    //}
}]);

