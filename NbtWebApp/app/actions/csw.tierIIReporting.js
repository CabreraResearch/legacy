/// <reference path="~/app/CswApp-vsdoc.js" />
(function () {
Csw.actions.tierIIReporting = Csw.actions.template ||
    Csw.actions.register('tierIIReporting', function (cswParent, cswPrivate) {
        'use strict';
        var cswPublic = {};
        if (Csw.isNullOrEmpty(cswParent)) {
            Csw.error.throwException('Cannot create an action without a valid Csw Parent object.', 'Csw.actions.tierIIReporting', 'csw.tierIIReporting.js', 10);
        }
        
        //#region _preCtor
        (function _preCtor() {
            cswPrivate.name = cswPrivate.name || 'Tier II Reporting';
            cswPrivate.onCancel = cswPrivate.onCancel || function () {};

            cswParent.empty();
        }());
        //#endregion _preCtor

        //#region Action Functions
        cswPrivate.onCancelClick = function() {
            Csw.tryExec(cswPrivate.onCancel);
        };
        //#endregion Action Functions

        //#region _postCtor
        (function _postCtor() {
            cswPrivate.action = Csw.layouts.action(cswParent, {
                title: 'Tier II Reporting',
                useFinish: false,
                cancelText: 'Close',
                onCancel: cswPrivate.onCancelClick,
                hasButtonGroup: true
            });
            
            cswPrivate.controlTbl = cswPrivate.action.actionDiv.table({
                name: cswPrivate.name + '_control_tbl',
                cellpadding: '5px',
                cellalign: 'left',
                cellvalign: 'middle',
                width: '95%',
                FirstCellRightAlign: true
            });
            
            cswPrivate.gridTbl = cswPrivate.action.actionDiv.table({
                name: cswPrivate.name + '_control_tbl',
                cellpadding: '5px',
                cellalign: 'left',
                align: 'center',
                cellvalign: 'middle',
                width: '95%'
            });

            //TODO -
            /// Select location, start date, and end date (a la Reconciliation) - click an "update" button (?)
            /// update loads a grid with the data supplied in tier ii web service
        }());
        //#endregion _postCtor
        
        return cswPublic;
    });
}());