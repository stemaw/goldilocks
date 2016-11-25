 myApp.controller('blogController', ['$scope', '$http', 'viewModel', 'postUrl', '$timeout','$rootScope',
    function ($scope, $http, viewModel, searchUrl, $timeout, $rootScope) {
        $scope.viewModel = viewModel;
        $scope.postUrl = searchUrl;
        $rootScope.title = "Car Quiz";
        
        $scope.Answer = function (answer) {
           $scope.doingStuff = true;
           $scope.viewModel.showAnswers = true;
           $scope.viewModel.Correct = answer == $scope.viewModel.Answer;

           var postData = $scope.viewModel;

            $http.post($scope.postUrl, postData).
               success(function (data, status, headers, config) {
                   $timeout(function() { $scope.viewModel = JSON.parse(data); }, 2000);
                   $scope.doingStuff = false;
               }).
               error(function (data, status, headers, config) {
                   $scope.failedToSend = true;
                   $scope.doingStuff = false;
               });
        };
   }]
);
