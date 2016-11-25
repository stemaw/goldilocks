myApp.controller('carEditController', ['$scope', '$http', 'viewModel', 'updateUrl', 'pageNumber', 'getUrl', '$window', 'updateAttractivenessUrl',
   function ($scope, $http, viewModel, updateUrl, pageNumber, getUrl, $window, updateAttractivenessUrl) {
       $scope.viewModel = viewModel;
       $scope.updateUrl = updateUrl;
       $scope.getUrl = getUrl;
       $scope.pageNumber = pageNumber;
       $scope.updateAttractivenessUrl = updateAttractivenessUrl;
       
       $scope.CurrentCarIndex = 0;

       $scope.nextCar = function () {
           $scope.CurrentCarIndex++;
           if ($scope.CurrentCarIndex === 10) {
               $scope.pageNumber++;
               $scope.getCars();
               $scope.CurrentCarIndex = 0;
           }
       };

       $scope.previousCar = function () {
           $scope.CurrentCarIndex--;
           if ($scope.CurrentCarIndex < 0) {
               $scope.pageNumber--;
               $scope.getCars();
               $scope.CurrentCarIndex = 0;
           }
       };

       $scope.getCars = function () {
           $http.get(getUrl + $scope.pageNumber).
               success(function (data, status, headers, config) {
                   $scope.viewModel = JSON.parse(data);
                   $scope.sending = false;
               }).
               error(function (data, status, headers, config) {
                   $scope.failedToSend = true;
                   $scope.sending = false;
               });
       };

       $scope.getCars();

       $scope.replaceImage = function() {
           var site;
           switch ($scope.viewModel[$scope.CurrentCarIndex].Manufacturer) {
           case "Alfa Romeo":
               $window.open('http://www.alfaromeopress.com/gallery');
           case "Fiat":
               $window.open('http://www.fiatpress.com/gallery');
               break;
           case "Jeep":
               $window.open('http://www.jeeppress-europe.com/gallery');
           case "Abarth":
               $window.open('http://www.abarthpress.com/gallery');
               break;
               case "Audi":
                   //$window.open('https://www.audi-mediacenter.com/en/photos/topics/modelle-16');
                   site = "http://www.audi-mediacenter.com";
                   break;
           case "BMW":
               site = "http://www.bmw.co.uk";
               break;
           case "Citroen":
               site = "http://www.citroen.co.uk";
               break;
           case "Volvo":
               site = "http://www.media.volvocars.com";
               break;
           }
           
           var query = $scope.viewModel[$scope.CurrentCarIndex].Manufacturer + '+' + $scope.viewModel[$scope.CurrentCarIndex].Model + '+' + $scope.viewModel[$scope.CurrentCarIndex].Derivative + '+' + $scope.viewModel[$scope.CurrentCarIndex].YearFrom + '+' + $scope.viewModel[$scope.CurrentCarIndex].YearTo + '+site:' + site;
           $window.open('https://www.google.co.uk/search?espv=2&biw=1745&bih=814&source=lnms&tbm=isch&sa=X&ei=j_tNVdvjN8OC7gaxvIE4&ved=0CAYQ_AUoAQ#tbs=isc:black%2Cic:trans%2Cisz:l&tbm=isch&q=' + query);
       };

     //  $scope.uploadImage = function () {
     //      var f = document.getElementById('file').files[0],
     //r = new FileReader();
     //      r.onloadend = function (e) {
     //          var data = e.target.result;
     //          //send you binary data via $http or $resource or do anything else with it
     //      }
     //      r.readAsBinaryString(f);
     //  };

       $scope.update = function (reason) {
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
       
       $scope.updateAttractiveness = function() {
           var postData = {
               Id: $scope.viewModel[$scope.CurrentCarIndex].Id,
               Attractiveness: $scope.viewModel[$scope.CurrentCarIndex].Attractiveness
           };

           $http.post($scope.updateAttractivenessUrl, postData).
               success(function (data, status, headers, config) {
                   $scope.viewModel = JSON.parse(data);
                   $scope.sending = false;
               }).
               error(function (data, status, headers, config) {
                   $scope.failedToSend = true;
                   $scope.sending = false;
               });
       }
   }]
);
