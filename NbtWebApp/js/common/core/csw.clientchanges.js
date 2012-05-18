﻿/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {
    'use strict';

    var cswPrivateVar = {
        changed: 0,
        attachedToWindow: false,
        checkChangesEnabled: true
    };

    cswPrivateVar.checkChanges = function () {
        /// <summary>Check if changes have been made.</summary>
        /// <returns type="String">Confirmation text if changes have been made.</returns>
        if (cswPrivateVar.checkChangesEnabled && cswPrivateVar.changed === 1) {
            return 'If you continue, you will lose any changes made on this page.  To save your changes, click Cancel and then click the Save button.';
        }
    };

    cswPrivateVar.initCheckChanges = function () {
        /// <summary>Register the check change event on the window.onbeforeunload event.</summary>
        /// <returns type="Boolean">Always true</returns>
        // Assign the checkchanges event to happen onbeforeunload
        if (false === Csw.isNullOrEmpty(window.onbeforeunload)) {
            window.onbeforeunload = function () {
                var f = window.onbeforeunload;
                var ret = f();
                if (ret) {
                    return cswPrivateVar.checkChanges();
                } else {
                    return false;
                }
            };
        } else {
            window.onbeforeunload = function () {
                return cswPrivateVar.checkChanges();
            };
        }
    };

    if (false === cswPrivateVar.attachedToWindow) {
        $(window).load(cswPrivateVar.initCheckChanges);
        cswPrivateVar.attachedToWindow = true;
    }

    Csw.clientChanges = Csw.clientChanges ||
        Csw.register('clientChanges', Csw.makeNameSpace());

    Csw.clientChanges.register('setChanged', function () {
        /// <summary>Register a change.</summary>
        /// <returns type="Boolean">True if registered.</returns>
        if (cswPrivateVar.checkChangesEnabled) {
            cswPrivateVar.changed = 1;
        }
        return cswPrivateVar.checkChangesEnabled;
    });

    Csw.clientChanges.register('unsetChanged', function () {
        /// <summary>Unregister a change.</summary>
        /// <returns type="Boolean">True if unregistered.</returns>
        if (cswPrivateVar.checkChangesEnabled) {
            cswPrivateVar.changed = 0;
        }
        return cswPrivateVar.checkChangesEnabled;
    });

    Csw.clientChanges.register('manuallyCheckChanges', function () {
        /// <summary>Manually check for changes.</summary>
        /// <returns type="Boolean">True if no changes are registered or the user clicks OK.</returns>
        var ret = true,
            confirmString = '';
        if (cswPrivateVar.checkChangesEnabled && cswPrivateVar.changed === 1) {
            /* remember: confirm is globally blocking call */
            confirmString += 'Are you sure you want to navigate away from this page?\n\n';
            confirmString += 'If you continue, you will lose any changes made on this page.  ';
            confirmString += 'To save your changes, click Cancel and then click the Save button.\n\n';
            confirmString += 'Press OK to continue, or Cancel to stay on the current page.';
            ret = confirm(confirmString);

            // this serves several purposes:
            // 1. after you've been prompted to lose this change, you won't be prompted again for the same change later
            // 2. multiple calls to manuallyCheckChanges() in the same event won't prompt more than once
            if (ret) {
                cswPrivateVar.changed = 0;
            }
        }
        return ret;
    });

} ());
