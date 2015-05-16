var myApp = angular.module('mainApp', ['ngRoute', 'ngResource']);

myApp.run(['$location', '$rootScope', function ($location, $rootScope) {
    $rootScope.$on('$routeChangeSuccess', function (event, current, previous) {
        $rootScope.title = current.$$route.title;
    });

}]);

myApp.filter('enum', function() {
    return function(input) {
        input = input || '';
        var out = "";
        // insert a space before all caps
        out = input.replace(/([A-Z])/g, ' $1')
        // uppercase the first character
        out = out.replace(/^./, function(str) { return str.toUpperCase(); });
        return out;
    };
});

myApp.directive('onErrorSrc', function () {
    return {
        link: function(scope, element, attrs) {
            element.bind('error', function() {
                if (attrs.src != attrs.onErrorSrc) {
                    attrs.$set('src', attrs.onErrorSrc);
                }
            });
        }
    };
});

myApp.controller('mainController', ['$scope', '$http', 'viewModel', 'searchUrl',
   function ($scope, $http, viewModel, searchUrl) {
       $scope.viewModel = viewModel;
       $scope.searchUrl = searchUrl;
       
       $scope.submitRejection = function (reason) {
           $scope.sending = true;

           var postData = {
               CurrentCar: {
                   Id: $scope.viewModel.CurrentCar.Id,
                   Manufacturer: $scope.viewModel.CurrentCar.Manufacturer,
                   Model: $scope.viewModel.CurrentCar.Model,
               },
               RejectionReason: reason,
               Likes: $scope.viewModel.Likes,
               Dislikes: $scope.viewModel.Dislikes,
               PreviousRejections: $scope.viewModel.PreviousRejections
           };

           $http.post($scope.searchUrl, postData).
               success(function (data, status, headers, config) {
                   $scope.viewModel = JSON.parse(data);
                   $scope.sending = false;
               }).
               error(function (data, status, headers, config) {
                   $scope.failedToSend = true;
                   $scope.sending = false;
               });
       };
   }]
);
