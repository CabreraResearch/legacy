/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    'use strict';
    
    var ajaxCount = 0;
    var spinning = false;

    Csw.ajax.register('ajaxInProgress', function () {
        /// <summary> Evaluates whether a pending ajax request is still open. </summary>
        return (ajaxCount > 0);
    });
    
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
        if (ajaxOptions.watchGlobal) {
            toggleSpinner(1);
        }
    });

    //Fires when any jQuery AJAX request ends
    $(document).ajaxComplete(function (event, jqXHR, ajaxOptions) {
        if (ajaxOptions.watchGlobal) {
            toggleSpinner(-1);
        }
    });

}());
