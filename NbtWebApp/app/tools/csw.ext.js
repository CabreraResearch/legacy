/// <reference path="~/app/CswApp-vsdoc.js" />

   /* globals Csw:false, $:false */
(function () {
    'use strict';

    var cswPrivate = Object.create(null);

    Object.defineProperties(cswPrivate, {
       stateId: {
           value: function(id) {
               return Csw.clientSession.currentUserName() + '_' + id;
           }
       } 
    });

    Csw.ext = Csw.ext ||
        Csw.register('ext', cswPrivate);

} ());