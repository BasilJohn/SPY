app.controller('MainController', ['$scope', '$sce', 'MainService', '$filter', function ($scope, $sce, MainService, $filter) {
    getCinemasList();


    function getCinemasList() {
        var servCall = MainService.getWebData();
        servCall.then(function (d) {

            var divCinemasList = document.createElement('divCinemasList');
            divCinemasList.innerHTML = d.data;
            var cinemasdiv = divCinemasList.querySelector('#qbc-div-cinema')
            $scope.cinemasAnchorList = [];

            for (var i = 0; i < $(cinemasdiv).find('a').length; i++) {
                $scope.cinemasAnchorList.push({
                    value: $(cinemasdiv).find('a')[i].getAttribute('onclick').replace('getAllCinemaData', '').replace(/[()]/g, '').split(',')[0].replace(/["']/g, ""),
                    name: $(cinemasdiv).find('a')[i].getAttribute('onclick').replace('getAllCinemaData', '').replace(/[()]/g, '').split(',')[1].replace(/["']/g, "")
                });
            }

        }, function (error) {
            $log.error('Oops! Something went wrong while fetching the data.')
        })
    }

    $scope.getCinemaData = function () {

        var currentSelected = $filter('filter')($scope.cinemasAnchorList, { value: $scope.selectedMovie })[0]
        var getCinemaDataCall = MainService.getCinemaData(currentSelected.name, currentSelected.value);
        getCinemaDataCall.then(function (d) {

            var divCinemaData = document.createElement('divCinemaData');
            divCinemaData.innerHTML = d.data;
            var cinemaDateDiv = divCinemaData.querySelector('.venue');

            $scope.dateList = [];
            for (var i = 0; i < $(cinemaDateDiv).find('.md-date a').length; i++) {
                $scope.dateList.push({
                    text: $(cinemaDateDiv).find('.md-date a')[i].innerHTML
                });
            }

            var movieSessionsDiv = cinemaDateDiv.querySelector('#movie-sessions');
            $scope.movieList = [];
            $scope.movieTimingList = [];


            var filmListDiv = $(movieSessionsDiv).find('ul:first>li')

            for (var j = 0; j < $(filmListDiv).length; j++) {

                var li = $(filmListDiv)[j];

                for (var k = 0; k < $(li).find('.show-times li').length; k++) {

                    $scope.movieTimingList.push({
                        name: $(li).find('.cinema-name span.cn-ellipsis')[0].innerHTML,
                        value: $(li).find('.show-times li>a')[k].innerHTML,
                        classType: $(li).find('.show-times li>a')[k].className,
                        href: $(li).find('.show-times li>a')[k].href
                    });
                }
            }
        }, function (error) {
            $log.error('Oops! Something went wrong while fetching the data.')
        })
    };

    $scope.saveUserData = function () {
        
        var saveUserDetails = MainService.saveUserDetails($scope.mobileNumber, $scope.selectedMovie, $scope.selectedDate, $scope.href, $scope.movieTime);
        saveUserDetails.then(function (d) {

            if (d.data)
            {
                alert('Details saved.')
            }
        });
    };

    $scope.getMovieData = function (movieData) {

        $scope.href = movieData.href;
        $scope.movieTime = movieData.value;
    };

    $scope.startService = function () {

        var serviceDetails = MainService.startSPYService();

    };
}]);  