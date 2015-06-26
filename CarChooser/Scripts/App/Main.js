var myApp = angular.module('mainApp', ['ngRoute', 'ngResource','angular-carousel']);

myApp.run(['$location', '$rootScope', function ($location, $rootScope) {

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

myApp.controller('mainController', ['$scope', '$http', 'viewModel', 'searchUrl', '$location', '$window', '$rootScope',
   function ($scope, $http, viewModel, searchUrl, $location, $window, $rootScope) {
       $scope.viewModel = viewModel;
       $scope.searchUrl = searchUrl;
       $scope.showReviews = false;
       $scope.comparisons = [];
       $location.path($scope.viewModel.CurrentCar.UrlName);
       
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
                   $location.path($scope.viewModel.CurrentCar.UrlName);
               }).
               error(function (data, status, headers, config) {
                   $scope.failedToSend = true;
                   $scope.doingStuff = false;
               });
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
       
       $rootScope.$on('$locationChangeSuccess', function () {
           $rootScope.actualLocation = $location.path();
           $rootScope.title = $scope.viewModel.CurrentCar.FullName;
       });

       $rootScope.$watch(function () { return $location.path() }, function (newLocation, oldLocation) {
           if ($rootScope.actualLocation === newLocation) {
               var back, historyState = $window.history.state;

               back = !!(historyState && historyState.position <= $rootScope.stackPosition);

               if (back) {
                   //back button
                   $rootScope.stackPosition--;
               } else {
                   //forward button
                   $rootScope.stackPosition++;
               }

               $scope.loadPrevious();
               
           } else {
               //normal-way change of page (via link click)

               if ($rootScope.current) {

                   $window.history.replaceState({
                       position: $rootScope.stackPosition
                   });

                   $rootScope.stackPosition++;
               }
           }
       });
       
       $scope.loadPrevious = function() {
           $scope.doingStuff = true;
           
           var postData = {
               CurrentCar: $scope.viewModel.CurrentCar,
               Likes: $scope.viewModel.Likes,
               Dislikes: $scope.viewModel.Dislikes,
               PreviousRejections: $scope.viewModel.PreviousRejections,
               RequestedCarId: $location.path().substring($location.path().lastIndexOf("/")+1, $location.path().length)
           };

           $http.post($scope.searchUrl, postData).
               success(function(data, status, headers, config) {
                   $scope.viewModel = JSON.parse(data);
                   $scope.Finished = $scope.viewModel.CurrentCar == null;
                   $scope.doingStuff = false;
               }).
               error(function(data, status, headers, config) {
                   $scope.failedToSend = true;
                   $scope.doingStuff = false;
               });
       };
   }]
);
