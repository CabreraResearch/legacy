/// <reference path="~/app/CswApp-vsdoc.js" />
(function () {
Csw.actions.hmisReporting = Csw.actions.template ||
    Csw.actions.register('hmisReporting', function (cswParent, cswPrivate) {
        'use strict';
        var cswPublic = {};

        if (Csw.isNullOrEmpty(cswParent)) {
            Csw.error.throwException('Cannot create an action without a valid Csw Parent object.', 'Csw.actions.hmisReporting', 'csw.hmisReporting.js', 10);
        }
        (function _preCtor() {
            //set default values on cswPrivate if none are supplied
            cswPrivate.name = cswPrivate.name || 'CswAction';
            cswPrivate.onSubmit = cswPrivate.onSubmit || function () {};
            cswPrivate.onCancel = cswPrivate.onCancel || function () {};

            cswParent.empty();
        }());

        cswPrivate.onSubmitClick = function() {
            Csw.tryExec(cswPrivate.onSubmit);
        };

        cswPrivate.onCancelClick = function() {
            Csw.tryExec(cswPrivate.onCancel);
        };

        (function _postCtor() {
            cswPrivate.action = Csw.layouts.action(cswParent, {
                title: 'HMIS Reporting',
                finishText: 'Finish',
                onFinish: cswPrivate.onSubmitClick,
                onCancel: cswPrivate.onCancelClick
            });

            cswPrivate.actionTbl = cswPrivate.action.actionDiv.table({
                name: cswPrivate.name + '_tbl',
                align: 'center'
            }).css('width', '95%');

            cswPrivate.actionTbl.cell(1, 1)
                .css('text-align', 'left')
                .span({ text: 'HMIS Reporting' });
        }());

        return cswPublic;
    });
}());