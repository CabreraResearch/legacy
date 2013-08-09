/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    'use strict';

    Csw.register('promises', Csw.makeNameSpace());

    /**
     * A promise wrapper around AJAX requests.
    */
    Csw.promises.register('ajax', function(ajax) {
        var promise = Q.when(ajax);
        
        //Hybrid compatability with jQuery's XHR object.
        promise.abort = ajax.abort;
        promise.readyState = ajax.readyState;
                
        return promise;
    });

    var ajaxCount = 0;
    var spinning = false;
    function showAjaxSpinner() {
        if (ajaxCount > 0 && spinning === false) {
            spinning = true;
            if (Csw.main.ajaxImage) {
                Csw.main.ajaxImage.show();
            }
            if (Csw.main.ajaxSpacer) {
                Csw.main.ajaxSpacer.hide();
            }
        }
    }
    function hideAjaxSpinner() {
        if (ajaxCount === 0 && spinning === true) {
            spinning = false;
            if (Csw.main.ajaxImage) {
                Csw.main.ajaxImage.hide();
            }
            if (Csw.main.ajaxSpacer) {
                Csw.main.ajaxSpacer.show();
            }
        }
    }

    //Fires when all jQuery AJAX requests have completed
    $(document).ajaxStop(function () {
        ajaxCount = 0;
        hideAjaxSpinner();
    });

    //Fires when any jQuery AJAX request starts
    $(document).ajaxSend(function (event, jqXHR, ajaxOptions) {
        if (ajaxOptions.watchGlobal) {
            ajaxCount += 1;
            showAjaxSpinner();
        }
    });

    //Fires when any jQuery AJAX request ends
    $(document).ajaxComplete(function (event, jqXHR, ajaxOptions) {
        if (ajaxOptions.watchGlobal) {
            ajaxCount -= 1;
            hideAjaxSpinner();
        }
    });

} ());
