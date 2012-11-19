/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {

    Csw.actions.containerMove = Csw.actions.containerMove ||
        Csw.actions.register('containerMove', function (cswParent, options) {
            'use strict';
            var cswPublic = {};
            var cswPrivate = {
                name: 'CswContainerMove',
                onSubmit: null,
                onCancel: null,
                gridOpts: {},
                viewid: '',
                requestitemid: '',
                location: ''
            };

            if (Csw.isNullOrEmpty(cswParent)) {
                Csw.error.throwException('Cannot create a Request action without a valid Csw Parent object.', 'Csw.actions.containerMove', 'csw.containerMove.js', 14);
            }
            (function _preCtor() {
                Csw.extend(cswPrivate, options, true);
                cswParent.empty();
            }());
            
            cswPrivate.submitRequest = function () {
                Csw.ajaxWcf.post({
                    urlMethod: 'Requests/fulfill',
                    data: {
                        RequestItemId: cswPrivate.requestitemid,
                        ContainerIds: cswPrivate.containerGrid.grid.getSelectedRowsVals('nodeid')
                    },
                    success: function (json) {
                        if (json.Succeeded) {
                            Csw.tryExec(cswPrivate.onSubmit);
                        } else {
                            cswPrivate.containerGrid.grid.reload();
                        }
                    }
                });
            };

            cswPrivate.initGrid = function () {
                
                Csw.ajax.post({
                    urlMethod: 'getDispenseContainerView',
                    data: {
                        RequestItemId: cswPrivate.requestitemid
                    },
                    success: function (data) {
                        if (Csw.isNullOrEmpty(data.viewid)) {
                            Csw.error.throwException(Csw.error.exception('Could not get a grid of containers for this request item.', '', 'csw.containerMove.js', 54));
                        }
                        cswPrivate.containerGrid = Csw.wizard.nodeGrid(cswPublic.gridParent, {
                            hasMenu: false,
                            viewid: data.viewid,
                            forceFit: false,
                            readonly: true,
                            showCheckboxes: true
                        });
                    }
                });
            }; // initGrid()
           
            (function _postCtor() {

                cswPrivate.action = Csw.layouts.action(cswParent, {
                    Title: cswPrivate.title || 'Fulfill Material Request by Size',
                    FinishText: 'Fulfill Request',
                    onFinish: cswPrivate.submitRequest,
                    onCancel: cswPrivate.onCancel
                });

                cswPrivate.actionTbl = cswPrivate.action.actionDiv.table({
                    name: cswPrivate.name + '_tbl',
                    align: 'center'
                }).css('width', '95%');

                cswPrivate.actionTbl.cell(1, 1)
                    .css('text-align', 'left')
                    .span({ text: 'Select the containers to move to: ' + cswPrivate.location });
                
                cswPrivate.actionTbl.cell(3, 1).br({ number: 2 });
                cswPrivate.gridId = cswPrivate.name + '_csw_requestGrid_outer';
                cswPublic.gridParent = cswPrivate.actionTbl.cell(4, 1).div({
                    name: cswPrivate.gridId
                }); 

                cswPrivate.initGrid();

            }());
            
            return cswPublic;
        });
}());

