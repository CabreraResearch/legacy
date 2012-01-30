/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function CswClientChanges() {
    'use strict';
    
    var _changed = 0;
    var _checkChangesEnabled = true;

    function setChanged() {
        if (_checkChangesEnabled) {
            _changed = 1;
        }
    }
    Csw.register('setChanged',setChanged);
    Csw.setChanged = Csw.setChanged || setChanged;

    function unsetChanged() {
        if (_checkChangesEnabled) {
            _changed = 0;
        }
    }
    Csw.register('unsetChanged',unsetChanged);
    Csw.unsetChanged = Csw.unsetChanged || unsetChanged;
    
    function checkChanges() {
        if (_checkChangesEnabled && _changed === 1) {
            return 'If you continue, you will lose any changes made on this page.  To save your changes, click Cancel and then click the Save button.';
        }
    }
    Csw.register('checkChanges',checkChanges);
    Csw.checkChanges = Csw.checkChanges || checkChanges;
    
    function manuallyCheckChanges() {
        var ret = true;
        if (_checkChangesEnabled && _changed === 1) {
            /* remember: confirm is globally blocking call */
            ret = confirm('Are you sure you want to navigate away from this page?\n\nIf you continue, you will lose any changes made on this page.  To save your changes, click Cancel and then click the Save button.\n\nPress OK to continue, or Cancel to stay on the current page.');

            // this serves several purposes:
            // 1. after you've been prompted to lose this change, you won't be prompted again for the same change later
            // 2. multiple calls to manuallyCheckChanges() in the same event won't prompt more than once
            if (ret) {
                _changed = 0;
            }
        }
        return ret;
    }
    Csw.register('manuallyCheckChanges',manuallyCheckChanges);
    Csw.manuallyCheckChanges = Csw.manuallyCheckChanges || manuallyCheckChanges;
    
    function initCheckChanges() {
        // Assign the checkchanges event to happen onbeforeunload
        if (false === Csw.isNullOrEmpty(window.onbeforeunload)) {
            window.onbeforeunload = function () {
                var f = window.onbeforeunload;
                var ret = f();
                if (ret) {
                    return checkChanges();
                } else {
                    return false;
                }
            };
        } else {
            window.onbeforeunload = function () {
                return checkChanges();
            };
        }
    }
    Csw.register('initCheckChanges',initCheckChanges);
    Csw.initCheckChanges = Csw.initCheckChanges || initCheckChanges;

    $(window).load(initCheckChanges);
    
}());