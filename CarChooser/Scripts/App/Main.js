var myApp = angular.module('mainApp', ['ngRoute', 'ngResource','angular-carousel']);

myApp.config(['$routeProvider', '$locationProvider', function AppConfig($routeProvider, $locationProvider) {

    //$routeProvider
    //    .when(
    //        '/Game', {
    //            redirectTo: '/Game'
    //        })
    //    .when('/whatisgoldilocks', {
    //        templateUrl: 'whatisgoldilocks.html'
    //    });
        //.when('/login', {
        //    templateUrl: 'templates/login.html'
        //})
        //.when('/news', {
        //    templateUrl: 'templates/news.html'
        //})
        //.when('/news/archive', {
        //    templateUrl: 'templates/newsarchive.html'
        //})
        //// removed other routes ... *snip
        //.otherwise({
        //    redirectTo: '/home'
        //}
    //);

    // enable html5Mode for pushstate ('#'-less URLs)
    $locationProvider.html5Mode(true);
    $locationProvider.hashPrefix('!');

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

 myApp.directive("starRating", function () {
    return {
        restrict: "EA",
        template: "<ul class='rating' ng-class='{readonly: readonly}'>" +
                   "  <li ng-repeat='star in stars' ng-class='star' ng-click='toggle($index)'>" +
                   "    <i class='fa fa-star'></i>" + //&#9733
                   "  </li>" +
                   "</ul>",
        scope: {
            ratingValue: "=ngModel",
            max: "=?", //optional: default is 5
            onRatingSelected: "&?",
            readonly: "=?"
        },
        link: function (scope, elem, attrs) {
            if (scope.max == undefined) { scope.max = 5; }
            function updateStars() {
                scope.stars = [];
                for (var i = 0; i < scope.max; i++) {
                    scope.stars.push({
                        filled: i < scope.ratingValue
                    });
                }
            };
            scope.toggle = function (index) {
                if (scope.readonly == undefined || scope.readonly == false) {
                    scope.ratingValue = index + 1;
                }
            };
            scope.$watch("ratingValue", function (oldVal, newVal) {
                if (newVal) { updateStars(); }
            });
        }
    };
});

 myApp.controller('mainController', ['$scope', '$http', 'viewModel', 'searchUrl', '$location', '$window', '$rootScope', 'problemUrl', 'manufacturersUrl', 'modelsUrl', 'derivativesUrl', '$anchorScroll', '$window',
    function ($scope, $http, viewModel, searchUrl, $location, $window, $rootScope, problemUrl, manufacturersUrl, modelsUrl, derivativesUrl, $anchorScroll, $window) {
        $scope.viewModel = viewModel;
        $scope.searchUrl = searchUrl;
        $scope.problemUrl = problemUrl;
        $scope.manufacturersUrl = manufacturersUrl;
        $scope.modelsUrl = modelsUrl;
        $scope.derivativesUrl = derivativesUrl;
        $scope.showReviews = false;
        $scope.comparisons = [];
        $scope.showFigures = true;
        

        $scope.submitRejection = function (reason) {
           $scope.doingStuff = true;
           $location.path($scope.viewModel.CurrentCar.UrlName);
            
           var likeIds = $.map($scope.viewModel.Likes, function (c) {
               return { Id: c.Id };
           });

           var postData = {
               CurrentCar: $scope.viewModel.CurrentCar,
               RejectionReason: reason,
               Likes: likeIds,
               Dislikes: $scope.viewModel.Dislikes,
               PreviousRejections: $scope.viewModel.PreviousRejections,
               LikeIt: reason == 'like',
               UserRatings: $scope.viewModel.CurrentCar.hasRated ? $scope.viewModel.CurrentCar.UserRatings : null
           };

           $http.post($scope.searchUrl, postData).
               success(function (data, status, headers, config) {
                   $scope.viewModel = JSON.parse(data);
                   $scope.Finished = $scope.viewModel.CurrentCar == null;
                   $scope.doingStuff = false;
                   $location.path($scope.viewModel.CurrentCar.UrlName);
                   $window.document.title = $scope.viewModel.CurrentCar.FullName + " - your next car?";
               }).
               error(function (data, status, headers, config) {
                   $scope.failedToSend = true;
                   $scope.doingStuff = false;
               });
        };

        $scope.startAgain = function() {
            $location.path("");
            history.go(0);
        };
        
        $scope.submitProblem = function (reason) {
            $scope.doingStuff = true;

            var postData = {
                Id: $scope.viewModel.CurrentCar.Id,
                reason: reason
            };

            $http.post($scope.problemUrl, postData).
                success(function (data, status, headers, config) {
                    $scope.doingStuff = false;
                    $scope.viewModel.CurrentCar.ShowThanks = true;
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
           //if ($location.path() !== "/") {
           //    $rootScope.title = $scope.viewModel.CurrentCar.FullName;
           //}
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
                   $window.document.title = $scope.viewModel.CurrentCar.FullName + " - your next car?";
               }).
               error(function(data, status, headers, config) {
                   $scope.failedToSend = true;
                   $scope.doingStuff = false;
               });
       };
       
       $scope.getManufacturers = function () {
           
           $http.get($scope.manufacturersUrl).
               success(function (data, status, headers, config) {
                   $scope.manufacturers = JSON.parse(data);
                   $scope.doingStuff = false;
               }).
               error(function (data, status, headers, config) {
                   $scope.failedToSend = true;
                   $scope.doingStuff = false;
               });
       };

       $scope.getManufacturers();

       $scope.getModels = function () {
           $scope.doingStuff = true;

           $http.get($scope.modelsUrl + '?manufacturer=' + $scope.selectedManufacturer).
               success(function (data, status, headers, config) {
                   $scope.models = JSON.parse(data);
                   $scope.doingStuff = false;
               }).
               error(function (data, status, headers, config) {
                   $scope.failedToSend = true;
                   $scope.doingStuff = false;
               });
       };
        
       $scope.getDerivatives = function () {
           $scope.doingStuff = true;

           $http.get($scope.derivativesUrl + '?model=' + $scope.selectedModel).
               success(function (data, status, headers, config) {
                   $scope.derivatives = JSON.parse(data);
                   $scope.doingStuff = false;
               }).
               error(function (data, status, headers, config) {
                   $scope.failedToSend = true;
                   $scope.doingStuff = false;
               });
       };

       $scope.selectCar = function () {
           $scope.doingStuff = true;

           var postData = {
               CurrentCar: $scope.viewModel.CurrentCar,
               Likes: $scope.viewModel.Likes,
               Dislikes: $scope.viewModel.Dislikes,
               PreviousRejections: $scope.viewModel.PreviousRejections,
               RequestedCarId: $scope.selectedDerivative
           };

           $http.post($scope.searchUrl, postData).
               success(function (data, status, headers, config) {
                   $scope.viewModel = JSON.parse(data);
                   $scope.Finished = $scope.viewModel.CurrentCar == null;
                   $location.path($scope.viewModel.CurrentCar.UrlName);
                   $scope.doingStuff = false;
                   $("#scrollToChoose").click();
                   $window.document.title = $scope.viewModel.CurrentCar.FullName + " - your next car? like or dislike?";
               }).
               error(function (data, status, headers, config) {
                   $scope.failedToSend = true;
                   $scope.doingStuff = false;
               });
       };

        $scope.showCarSelection = function() {
            angular.element(document.querySelector('#selectCar')).removeClass("hidden-xs").removeClass("hidden-sm");
        };
    }]
);
