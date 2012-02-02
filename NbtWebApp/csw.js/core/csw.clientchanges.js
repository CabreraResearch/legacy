;/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function CswClientChanges() {
    'use strict';

    /* The class can be instanced many times, but this will be available to all instances through the outer closure. */
    var _changed = 0;
    var _attachedToWindow = false;

    var clientChanges = (function clientChangesP(setCheckChanges) {
        /// <summary>Csw Client Changes class.</summary>
        /// <returns type="Object">Returns an instance of the class with methods for setting and unsetting changes.</returns>

        var checkChangesEnabled = (arguments.length === 0 || Csw.bool(setCheckChanges));

        function _checkChanges() {
            /// <summary>Check if changes have been made.</summary>
            /// <returns type="String">Confirmation text if changes have been made.</returns>
            if (checkChangesEnabled && _changed === 1) {
                return 'If you continue, you will lose any changes made on this page.  To save your changes, click Cancel and then click the Save button.';
            }
        }

        function _initCheckChanges() {
            /// <summary>Register the check change event on the window.onbeforeunload event.</summary>
            /// <returns type="Boolean">Always true</returns>
            // Assign the checkchanges event to happen onbeforeunload
            if (false === Csw.isNullOrEmpty(window.onbeforeunload)) {
                window.onbeforeunload = function() {
                    var f = window.onbeforeunload;
                    var ret = f();
                    if (ret) {
                        return _checkChanges();
                    } else {
                        return false;
                    }
                };
            } else {
                window.onbeforeunload = function() {
                    return _checkChanges();
                };
            }
        }

        function setChanged() {
            /// <summary>Register a change.</summary>
            /// <returns type="Boolean">True if registered.</returns>
            if (checkChangesEnabled) {
                _changed = 1;
            }
            return checkChangesEnabled;
        }

        function unsetChanged() {
            /// <summary>Unregister a change.</summary>
            /// <returns type="Boolean">True if unregistered.</returns>
            if (checkChangesEnabled) {
                _changed = 0;
            }
            return checkChangesEnabled;
        }

        function manuallyCheckChanges() {
            /// <summary>Manually check for changes.</summary>
            /// <returns type="Boolean">True if no changes are registered or the user clicks OK.</returns>
            var ret = true;
            if (checkChangesEnabled && _changed === 1) {
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

        if (false === _attachedToWindow) {
            $(window).load(_initCheckChanges);
            _attachedToWindow = true;
        }

        return {
            manuallyCheckChanges: manuallyCheckChanges,
            unsetChanged: unsetChanged,
            setChanged: setChanged
        };
    }());
    Csw.register('clientChanges', clientChanges);
    Csw.clientChanges = Csw.clientChanges || clientChanges;

}());
