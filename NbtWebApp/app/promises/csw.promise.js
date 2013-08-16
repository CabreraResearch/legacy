/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    'use strict';

    Csw.register('promises', Csw.makeNameSpace());

    /**
     * A promise wrapper around AJAX requests.
    */
    Csw.promises.register('ajax', function (ajax) {
        var promise = Q.when(ajax);

        //Hybrid compatability with jQuery's XHR object.
        promise.abort = ajax.abort;
        promise.readyState = ajax.readyState;

        return promise;
    });

    Csw.promises.register('all', function (initArray) {

        var reqs = initArray || [];
        var promise = Q.all(reqs);

        promise.push = function (item) {
            reqs.push(item);
        };

        return promise;
    });

}());
