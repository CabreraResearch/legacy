/// <reference path="~/app/CswApp-vsdoc.js" />
(function () { 

    Csw.layouts.addnode = Csw.layouts.addnode ||
        Csw.layouts.register('addnode', function (cswPrivate) {
            'use strict';
            
            var cswPublic = {};

            (function _preCtor() {
                cswPrivate.name = cswPrivate.name || 'addnode';
                cswPrivate.dialogOptions = cswPrivate.dialogOptions || 'addnode';
            }());

            (function _postCtor() {
                $.CswDialog('AddNodeDialog', cswPrivate.dialogOptions);
            } ());
            
            return cswPublic;
        });
} ());

