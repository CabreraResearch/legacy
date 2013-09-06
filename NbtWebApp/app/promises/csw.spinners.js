/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    'use strict';

    var ajaxeces = [];
    var ajaxCount = 0;
    var spinning = false;

    function toggleSpinner(incrementor) {
        ajaxCount += incrementor;
        if (Csw.main.ajaxImage && Csw.main.ajaxSpacer) {
            if (ajaxCount > 0 && spinning === false) {
                spinning = true;
                Csw.main.ajaxImage.show();
                Csw.main.ajaxSpacer.hide();
            }
            else if (ajaxCount === 0 && spinning === true) {
                spinning = false;

                Csw.main.ajaxImage.hide();
                Csw.main.ajaxSpacer.show();
            }
        }
    }

    //Fires when all jQuery AJAX requests have completed
    $(document).ajaxStop(function () {
        ajaxCount = 0;
        toggleSpinner(0);
    });

    //Fires when any jQuery AJAX request starts
    $(document).ajaxSend(function (event, jqXHR, ajaxOptions) {
        ajaxeces.push(jqXHR);
        if (ajaxOptions.watchGlobal) {
            toggleSpinner(1);
        }
    });

    //Fires when any jQuery AJAX request ends
    $(document).ajaxComplete(function (event, jqXHR, ajaxOptions) {
        var idx = ajaxeces.indexOf(jqXHR);
        if (-1 !== idx) {
            ajaxeces.splice(idx, 1);
        }
        if (ajaxOptions.watchGlobal) {
            toggleSpinner(-1);
        }
    });

    Csw.ajax.register('ajaxInProgress', function () {
        /// <summary> Evaluates whether a pending ajax request is still open. </summary>
        return (ajaxCount > 0);
    });

    Csw.ajax.register('abortAll', function () {
        /// <summary> Terminates all open AJAX requests </summary>
        Csw.iterate(ajaxeces, function(ajax) {
            if (ajax && ajax.abort) {
                ajax.abort();
            }
        });
    });
}());
