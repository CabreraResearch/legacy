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

    //Fires when all jQuery AJAX requests have completed
    $(document).ajaxStop(function () {
        window.name.ajaxCount = 0;
        Csw.main.onReady.then(function() {
            Csw.main.ajaxImage.hide();
            Csw.main.ajaxSpacer.show();
        });
    });

    //Fires when any jQuery AJAX request starts
    $(document).ajaxSend(function (event, jqXHR, ajaxOptions) {
        if (ajaxOptions.watchGlobal) {
            window.name.ajaxCount = window.name.ajaxCount || 0;
            window.name.ajaxCount += 1;

            Csw.main.onReady.then(function() {
                Csw.main.ajaxImage.show();
                Csw.main.ajaxSpacer.hide();
            });
        }
    });

    //Fires when any jQuery AJAX request ends
    $(document).ajaxComplete(function (event, jqXHR, ajaxOptions) {
        if (ajaxOptions.watchGlobal) {
            window.name.ajaxCount = window.name.ajaxCount || 1;
            window.name.ajaxCount -= 1;

            Csw.main.onReady.then(function() {
                Csw.main.ajaxImage.hide();
                Csw.main.ajaxSpacer.show();
            });
        }
    });

} ());
