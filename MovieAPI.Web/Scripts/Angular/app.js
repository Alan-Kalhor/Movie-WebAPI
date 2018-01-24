angular.module('myMovieApp', [])
    .controller('movieController', function ($scope, $http) {
        $scope.errmsg = '';
        $scope.showError = false;
        $scope.movieModel = {};
        $scope.column = 'MovieId';
        $scope.reverse = false;
        $scope.search = "";

        getallData();

        function getallData() {
            $scope.showLoading = true;
            $scope.showError = false;
            $scope.showGrid = false;
            $scope.showForm = false;

            $http({
                method: "GET",
                url: "http://localhost:29869/api/movies?sort=" + $scope.column + ($scope.reverse ? "&desc=1" : "") + ($scope.search ? "&filter=" + $scope.search : "")
            }).then(function mySuccess(response) {
                $scope.movieList = response.data;
                $scope.showGrid = true;
                $scope.showLoading = false;
            }, function myError(response) {
                $scope.showLoading = false;
                $scope.showError = true;
                $scope.errmsg = response.status + " - " + response.statusText;
            });
        }

        $scope.sortColumn = function (col) {
            if ($scope.column != col) {
                $scope.column = col;
                $scope.reverse = false;
                $scope.reverseclass = 'asc';
            }
            else {
                if ($scope.reverse) {
                    $scope.reverse = false;
                    $scope.reverseclass = 'desc';
                } else {
                    $scope.reverse = true;
                    $scope.reverseclass = 'asc';
                }
            }
            getallData();
        }
        // remove and change class
        $scope.sortClass = function (col) {
            if ($scope.column == col) {
                if ($scope.reverse) {
                    return 'asc';
                } else {
                    return 'desc';
                }
            } else {
                return '';
            }
        }

        $scope.AddMovieDiv = function () {
            //ClearFields();
            $scope.Action = "Add";
            $scope.showForm = true;
        }
        $scope.updateMovie = function (movie) {
            $scope.Action = "Update";
            $scope.showForm = true;
            $scope.movieModel = movie;
            $scope.scrollToTop();
        }

        $scope.scrollToTop = function ($var) {
            // 'html, body' denotes the html element, to go to any other custom element, use '#elementID'
            $('html, body').animate({
                scrollTop: 0
            }, 'fast'); // 'fast' is for fast animation
        };
        $scope.AddUpdateMovie = function () {
            var httpMethod = "POST";
            var httpURL = "http://localhost:29869/api/movies";
            if ($scope.Action == "Update") {
                httpMethod = "PUT";
                httpURL = "http://localhost:29869/api/movies/" + $scope.movieModel.MovieId;
            }
            var castStr = $scope.movieModel.Cast.toString();
            var castArr = {};
            castArr = castStr.split(',');
            $scope.movieModel.Cast = castArr;

            $http({
                method: httpMethod,
                url: httpURL,
                data: $scope.movieModel
            }).then(function mySuccess(response) {
                getallData();
                $scope.movieModel = {};
            }, function myError(response) {
                $scope.showError = true;
                $scope.errmsg = "Failed to save the data. Please make sure you enter valid values - " + response.status + " - " + response.statusText;
            });

        }

        $scope.Cancel = function () {
            $scope.showForm = false;
            $scope.showError = false;
        }

    });
