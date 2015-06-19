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
       $scope.showReviews = false;
       $scope.comparisons = [];

       $scope.submitRejection = function (reason) {
           $scope.doingStuff = true;

           var postData = {
               CurrentCar: $scope.viewModel.CurrentCar,
               RejectionReason: reason,
               Likes: $scope.viewModel.Likes,
               Dislikes: $scope.viewModel.Dislikes,
               PreviousRejections: $scope.viewModel.PreviousRejections,
               LikeIt: reason == 'like'
           };

           $http.post($scope.searchUrl, postData).
               success(function (data, status, headers, config) {
                   $scope.viewModel = JSON.parse(data);
                   $scope.Finished = $scope.viewModel.CurrentCar == null;
                   $scope.doingStuff = false;
                   startTheClock();
               }).
               error(function (data, status, headers, config) {
                   $scope.failedToSend = true;
                   $scope.doingStuff = false;
               });
       };

       $scope.startTheClock = function() {
           stop = $interval(function () {
               if ($scope.blood_1 > 0 && $scope.blood_2 > 0) {
                   $scope.blood_1 = $scope.blood_1 - 3;
                   $scope.blood_2 = $scope.blood_2 - 4;
               } else {
                   $scope.stopFight();
               }
           }, 100);
       };
       
       $scope.selectToCompare = function (index) {

           var carId = $scope.viewModel.Likes[index].Id;
           for (var i = 0; i < $scope.comparisons.length; i++) {
               if ($scope.comparisons[i].Id == carId ) {
                   return $scope.comparisons.splice(i, 1);
               }
           }
           return $scope.comparisons.push($scope.viewModel.Likes[index]);
       };

       $scope.removeLike = function(id) {
           for (var i = 0; i < $scope.comparisons.length; i++) {
               if ($scope.comparisons[i].Id == id) {
                   $scope.comparisons.splice(i, 1);
               }
           }
           
           for (var j = 0; j < $scope.viewModel.Likes.length; j++) {
               if ($scope.viewModel.Likes[j].Id == id) {
                   $scope.viewModel.Likes.splice(j, 1);
               }
           }
       };
   }]
);
