
/// <reference path="~/app/CswApp-vsdoc.js" />

(function ($) {

    Csw.composites.register('draggablepanel', function (cswParent, options) {
        'use strict';
        var cswPrivate = {
            height: 400,
            width: 400,
            border: 1,

            columns: 2
        };
        var cswPublic = {};

        var _colIdPrefix = window.Ext.id() + "_dragcol_";
        var _draggables = [];

        (function () {
            if (options) {
                Csw.extend(cswPrivate, options);
            }

            var myDiv = cswParent.div({ width: cswPrivate.width });

            //Create column items up front
            var _columns = [];
            for (var i = 0; i < cswPrivate.columns; i++) {
                _columns.push({
                    id: _colIdPrefix + i,
                    width: 200,
                    border: 1
                });
            }

            //Set up parent
            window.Ext.create('Ext.panel.Panel', {
                renderTo: myDiv.getId(),
                border: 1,
                items: [{
                    id: 'draggablepanel' + window.Ext.id(),
                    xtype: 'dragpanel',
                    items: _columns
                }]
            });

            cswPublic.addItemToCol = function (colNo, styleParams, render) {
                var colId = _colIdPrefix + colNo;
                var extComponent = window.Ext.getCmp(colId);
                if (!extComponent) { //Did we get a real column?
                    Csw.error.throwException('Cannot render to drag column', 'csw.draggablepanel.js');
                }

                //Add our drag panel
                var extRenderTo = extComponent.add({
                    id: window.Ext.id()
                });
                _draggables.push(extRenderTo);

                //Create a csw dom obj to render Csw content too
                var cswRenderTo = Csw.domNode({
                    ID: extRenderTo.body.id,
                    el: extRenderTo.body.dom
                });

                Csw.tryExec(render, extRenderTo, cswRenderTo);

                //Apply any styling supplied
                for (var name in styleParams) {
                    window.Ext.get(extRenderTo.getId()).setStyle(name, styleParams[name]);
                }
            };

            cswPublic.allowDrag = function (allow) {
                for (var idx in _draggables) {
                    var draggableItem = _draggables[idx];
                    draggableItem.allowDrag(allow);
                }
            };


        }());
        return cswPublic;
    });

})(jQuery);

