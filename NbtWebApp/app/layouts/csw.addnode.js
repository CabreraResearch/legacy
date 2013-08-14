/// <reference path="~/app/CswApp-vsdoc.js" />
(function () { 

    Csw.layouts.addnode = Csw.layouts.addnode ||
        Csw.layouts.register('addnode', function (cswPrivate) {
            'use strict';
            
            var cswPublic = {};

            (function _preCtor() {
                cswPrivate.name = cswPrivate.name || 'AddNode';
                cswPrivate.action = cswPrivate.action || 'AddNode';
                cswPrivate.dialogOptions = cswPrivate.dialogOptions || {};
            }());

            (function _postCtor() {
                if (false === Csw.isNullOrEmpty(cswPrivate.action) && cswPrivate.action !== 'AddNode') {
                    Csw.main.handleAction({ actionname: cswPrivate.action });
                } else {
                    $.CswDialog('AddNodeDialog', cswPrivate.dialogOptions);
                }
            } ());
            
            return cswPublic;
        });
} ());

