
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
                Csw.error.throwException('Could not find column: ' + colNo, 'csw.draggablepanel.js');
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
                    xtype: 'toolbar',
                    items: [{
                        xtype: 'button',
                        text: 'Add Column (+)',
                        onClick: function () {
                            cswPublic.addCol();
                        }
                    }]
                }, {
                    id: dragPanelCmpId,
                    xtype: 'dragpanel',
                    items: _columns
                }]
            });
            var dragPanelCmp = window.Ext.getCmp(dragPanelCmpId);

            cswPublic.addItemToCol = function (colNo, paramsIn) {
                var params = {
                    id: window.Ext.id(),
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
                    id: params.id,
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

            cswPublic.addCol = function () {
                //figure out how many columns we have
                var existingCols = dragPanelCmp.items.items.length;

                dragPanelCmp.add(makeCol(existingCols));
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

            cswPublic.doLayout = function () {
                /* When rendering Csw content to ExtJS controls, the layout is already calculated. Calling doLayout()
                   after rendering arbitrary csw content will cause the Ext controls to fix their width/height/ect */
                dragPanelCmp.doLayout();
            };

            cswPublic.lockPanels = function (lock) {
                /* Lock/Unlock all the child draggables in this drag panel. 
                   This prevents the draggables from being moved, but other items can be dragged into the panel */
                for (var idx in _draggables) {
                    var draggableItem = _draggables[idx];
                    draggableItem.allowDrag(lock);
                }
            };

            cswPublic.allowDrag = function (allow) {
                /* Lock/unlock all child draggables inside this drag panel and then register/unregister the DropTarget*/
                cswPublic.lockPanels(allow);
                dragPanelCmp.allowDrag(allow);
            };

            cswPublic.getItemsInCol = function (colNo) {
                /* Gets all draggables in the specified column */
                var extComponent = getCol(colNo);
                return extComponent.items.items;
            };

            cswPublic.getNumCols = function () {
                /* How many columns does this panel have? */
                return dragPanelCmp.items.items.length;
            };

        }());
        return cswPublic;
    });

})(jQuery);

