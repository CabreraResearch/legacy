
/// <reference path="~/app/CswApp-vsdoc.js" />

(function ($) {

    Csw.composites.register('draggablepanel', function (cswParent, options) {
        'use strict';
        var cswPrivate = {
            height: 400,
            width: 400,
            border: 1,

            columns: 1,
            showAddColumnButton: true
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
                    width: 200
                };
            };

            //Create column items up front
            var _columns = [];
            var addedCol = cswPrivate.columns;
            if (cswPrivate.columns + 1 <= 4) {
                addedCol = cswPrivate.columns + 1;
            }
            for (var i = 0; i < addedCol; i++) { //guarantee we provide one more col than asked for
                _columns.push(makeCol(i));
            }
            //Set up parent
            var dragPanelCmpId = 'draggablepanel' + window.Ext.id();

            window.Ext.create('Csw.ext.dragpanel', {
                renderTo: myDiv.getId(),
                id: dragPanelCmpId,
                border: 0,
                bodyStyle: cswPrivate.bodyStyle,
                items: _columns
            });
            var dragPanelCmp = window.Ext.getCmp(dragPanelCmpId);
            var addColBtn = myDiv.div().buttonExt({
                enabledText: 'Add Column',
                onClick: function () {
                    cswPublic.addCol();
                }
            });
            if (false === cswPrivate.showAddColumnButton) {
                addColBtn.hide();
            }

            var getLastColumn = function () {
                var colId = _colIdPrefix + (cswPublic.getNumCols() - 1);
                return window.Ext.getCmp(colId);
            };

            cswPublic.addItemToCol = function (colNo, paramsIn) {
                var params = {
                    id: window.Ext.id(),
                    style: {},
                    showRearrangeButton: true,
                    showConfigureButton: true,
                    showCloseButton: true,
                    render: function () { },
                    onDrop: function (extCmp, col, row) { },
                    onClose: function () { },
                    onConfigure: function () { },
                    onRearrange: function () { }
                };
                if (paramsIn) {
                    Csw.extend(params, paramsIn);
                }

                var extCol = getCol(colNo);

                var tools = [];
                if (params.showRearrangeButton) {
                    tools.push({
                        type: 'restore',
                        tooltip: 'Rearrange sub properties',
                        handler: params.onRearrange
                    });
                }
                if (params.showConfigureButton) {
                    tools.push({
                        type: 'gear',
                        tooltip: 'Configure property',
                        handler: function () {
                            params.onConfigure(extRenderTo);
                        }
                    });
                }
                if (params.showCloseButton) {
                    tools.push({
                        type: 'close',
                        tooltip: 'Remove from layout',
                        handler: function () {
                            params.onClose(extRenderTo);
                        }
                    });
                }

                //Add our drag panel
                var extRenderTo = extCol.add({
                    id: params.id,
                    tools: tools,
                    bodyStyle: cswPrivate.bodyStyle,
                    frame: false,
                    border: 0,
                    listeners: {
                        render: function (c) {
                            c.body.on('click', function () {
                                Csw.iterate(_draggables, function (draggable) {
                                    draggable.header.hide();
                                    draggable.getEl().applyStyles('border: 0px');
                                });
                                extRenderTo.getEl().applyStyles('border: 1px solid #BDD3F0');
                                extRenderTo.header.show();
                            });
                        },
                        afterrender: function () {
                            if (this.header) {
                                this.header.hide();
                            }
                        }
                    },
                    onDrop: function () {
                        params.onDrop();
                        var lastCol = getLastColumn();
                        if ( lastCol.items.items.length > 0 && dragPanelCmp.items.items.length < 4) {
                            cswPublic.addCol();
                        }
                    }
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

                dragPanelCmp.doLayout();
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

            cswPublic.removeDraggableFromCol = function (colNo, draggableId) {
                var newDraggables = [];
                Csw.iterate(_draggables, function (draggable) {
                    if (draggable.getId() !== draggableId) {
                        newDraggables.push(draggable);
                    }
                });
                _draggables = newDraggables;
                var extCol = getCol(colNo);
                extCol.remove(window.Ext.getCmp(draggableId));
            };

        }());
        return cswPublic;
    });

})(jQuery);

