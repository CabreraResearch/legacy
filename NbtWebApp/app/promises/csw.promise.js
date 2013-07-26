/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    'use strict';
    
    Csw.register('promises', Csw.makeNameSpace());

    /**
     * A promise wrapper around AJAX requests.
    */
    Csw.promises.register('ajax', function(ajax) {
        var promise = Csw.object();
        //Grab a handle on the original Q deferred object
        promise.Q = Q(ajax);

        //Hybrid compatability with jQuery's XHR object.
        promise.abort = ajax.abort;
        promise.readyState = ajax.readyState;

        //Expose subset of promise methods on the Csw promise object.
        promise.then = promise.Q.then;
        promise.progress = promise.Q.progress;
        promise.fail = promise.Q.fail;
        promise.done = promise.Q.done;


        return promise;
    });


} ());
