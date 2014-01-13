
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

        var getCol = function (colNo) {
            var colId = _colIdPrefix + colNo;
            var extComponent = window.Ext.getCmp(colId);
            if (!extComponent) { //Did we get a real column?
                Csw.error.throwException('Cannot render to drag column', 'csw.draggablepanel.js');
            }
            return extComponent;
        };

        (function () {
            if (options) {
                Csw.extend(cswPrivate, options);
            }

            var myDiv = cswParent.div({ width: cswPrivate.width });

            var makeCol = function (colNo) {
                return {
                    id: _colIdPrefix + colNo,
                    width: 200,
                    border: 1
                };
            };

            //Create column items up front
            var _columns = [];
            for (var i = 0; i < cswPrivate.columns; i++) {
                _columns.push(makeCol(i));
            }

            //Set up parent
            var dragPanelCmpId = 'draggablepanel' + window.Ext.id();
            window.Ext.create('Ext.panel.Panel', {
                renderTo: myDiv.getId(),
                border: 1,
                items: [{
                    id: dragPanelCmpId,
                    xtype: 'dragpanel',
                    items: _columns
                }]
            });
            var dragPanelCmp = window.Ext.getCmp(dragPanelCmpId);

            cswPublic.addItemToCol = function (colNo, paramsIn) {
                var params = {
                    style: {},
                    render: function () { },
                    onDrop: function (extCmp, col, row) { }
                };
                if (paramsIn) {
                    Csw.extend(params, paramsIn);
                }

                var extCol = getCol(colNo);

                //Add our drag panel
                var extRenderTo = extCol.add({
                    id: window.Ext.id(),
                    onDrop: params.onDrop
                });
                _draggables.push(extRenderTo);

                //Create a csw dom obj to render Csw content too
                var cswRenderTo = Csw.domNode({
                    ID: extRenderTo.body.id,
                    el: extRenderTo.body.dom
                });

                Csw.tryExec(params.render, extRenderTo, cswRenderTo);

                //Apply any styling supplied
                for (var name in params.style) {
                    window.Ext.get(extRenderTo.getId()).setStyle(name, params.style[name]);
                }
            };

            cswPublic.allowDrag = function (allow) {
                for (var idx in _draggables) {
                    var draggableItem = _draggables[idx];
                    draggableItem.allowDrag(allow);
                }
            };

            cswPublic.addCol = function () {
                //figure out how many columns we have
                var existingCols = dragPanelCmp.items.items.length;

                dragPanelCmp.add(makeCol(existingCols + 1));
            };

            cswPublic.removeCol = function (colNo) {
                var extComponent;
                if (colNo) {
                    extComponent = getCol(colNo);
                } else {
                    if (dragPanelCmp.items.items.length > 0) {
                        extComponent = dragPanelCmp.items.items[0];
                    }
                }

                if (extComponent) {
                    extComponent.destroy();
                }
            };

        }());
        return cswPublic;
    });

})(jQuery);

