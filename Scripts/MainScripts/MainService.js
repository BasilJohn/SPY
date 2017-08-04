app.service("MainService", function ($http) {
    this.getWebData = function () {
        return $http.get("api/Main");
    }
    this.getCinemaData = function (name, value) {
       
        return $http.get("api/GetCinemaData/" + name + "/" + value);
    }
 
    this.saveUserDetails = function (mobileNumber, selectedMovie, selectedDate, href, movieTime) {

        return $http.post("api/SaveUserDetails/" + mobileNumber + "/" + selectedMovie + "/" + selectedDate, '"' + href + "*" + movieTime+'"');
    }

    this.startSPYService = function ()
    {
        return $http.get("api/StartSPYService/service");
    }
});  