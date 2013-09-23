(function () {
    
    /**
     * Instance a web worker in an independent thread to make an AJAX POST request
     * Returns an object with a promise to fulfill the request
     * If web workers are supported, returns a handle on the worker
     *    else, returns the promise as a traditional AJAX promise
    */
    Csw.workers.register('ajax', function(url, data) {

        var ret = Csw.object();

        if (Modernizr.webworkers) { //Chrome, FF, IE10+
            var deferred = Q.defer();
            ret.promise = Q.promise;

            ret.webWorker = new Worker('webworkers/ajaxWorker.js');

            ret.listenTo = ret.webWorker.addEventListener;

            ret.webWorker.addEventListener('message', function(e) {
                if (e.error) {
                    deferred.reject(new Error(e));
                } else {
                    deferred.resolve(e.data);
                }
            }, false);

            ret.webWorker.addEventListener('error', function (e) {
                deferred.reject(new Error(e));
            }, false);

            ret.webWorker.addEventListener('log', function (e) {
                deferred.notify(e);
            }, false);

            ret.message = { data: data, url: 'Services/' + url };

            ret.webWorker.postMessage(ret.message);
        } else { //IE9
            var ajax = Csw.ajaxWcf.post({
                urlMethod: url,
                data: data
            });
            ret.promise = ajax;
        }
        return ret;
    });

    


}());