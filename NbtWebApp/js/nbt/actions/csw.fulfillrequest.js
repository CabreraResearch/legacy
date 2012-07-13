/// <reference path="~/js/CswCommon-vsdoc.js" />
/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="http://cdn.sencha.io/ext-4.1.0-gpl/ext-all-debug.js" />

(function() {
    'use strict';
    var cswFulfillRequestActionState = 'cswFulfillRequestActionState';
    
    Csw.actions.fulfillRequest = Csw.actions.fulfillRequest ||
        Csw.actions.register('fulfillRequest', function (cswParent, options) {
            //#region variables
            var cswPrivate = {
                ID: 'csw_request_fulfillment',
                table: {},
                requestTypeSelect: {},
                destinationLocation: {},
                materialInput: {},
                requestInput: {},
                inventoryGroupSelect: {},
                grid: {},
                cells: {},
                state: {
                    requesttype: '',
                    locationviewid: '',
                    materialname: '',
                    requestname: '',
                    inventorygroupid: ''
                }
            };

            var cswPublic = { };
            //#endregion variables

            //#region safety net
            Csw.tryExec(function() {

                //#region init ctor
                (function _pre() {
                    $.extend(cswParent, options);
                    (function() {
                        var state = Csw.clientDb.getItem(cswFulfillRequestActionState);
                        if(false === Csw.isNullOrEmpty(state)) {
                            $.extend(cswParent.state, state);
                        }
                    }());

                    Csw.clientDb.setItem(cswFulfillRequestActionState, cswPrivate.state);

                    cswPrivate.action = Csw.layouts.action(cswParent, {
                        Title: 'Fulfill Request',
                        hasButtonGroup: false
                    });

                    cswPrivate.table = cswPrivate.action.actionDiv.table({ ID: cswPrivate.ID + '_tbl', align: 'center' }).css('width', '80%');

                    cswPrivate.table.cell(1, 1).span({ text: 'Request Type' }).addClass('CswAction_LabelCell');
                    cswPrivate.table.cell(1, 2).span({ text: 'Destination' }).addClass('CswAction_LabelCell');
                    cswPrivate.table.cell(1, 3).span({ text: 'Material' }).addClass('CswAction_LabelCell');
                    cswPrivate.table.cell(1, 4).span({ text: 'Request' }).addClass('CswAction_LabelCell');
                    cswPrivate.table.cell(1, 5).span({ text: 'Inventory Group' }).addClass('CswAction_LabelCell');

                    var buildFilters = function() {
                        cswPrivate.cells.cell21 = cswPrivate.cells.cell21 || cswPrivate.table.cell(2, 1);
                        cswPrivate.cells.cell21.empty();
                        cswPrivate.cells.cell21.select({
                            values: cswPrivate.state.requesttype.split(','),
                            width: ('Request Type'.length * 8) + 'px'
                        });

                        cswPrivate.cells.cell22 = cswPrivate.cells.cell22 || cswPrivate.table.cell(2, 2);
                        cswPrivate.cells.cell22.empty();
                        cswPrivate.cells.cell22.location({
                            viewId: cswPrivate.state.locationviewid
                        });

                        cswPrivate.cells.cell23 = cswPrivate.cells.cell23 || cswPrivate.table.cell(2, 3);
                        cswPrivate.cells.cell23.empty();
                        cswPrivate.cells.cell23.input({
                            placeholder: 'Trade Name'
                        });

                        cswPrivate.cells.cell24 = cswPrivate.cells.cell24 || cswPrivate.table.cell(2, 4);
                        cswPrivate.cells.cell24.empty();
                        cswPrivate.cells.cell24.input({
                            placeholder: 'Request Name'                            
                        });

                        cswPrivate.cells.cell25 = cswPrivate.cells.cell25 || cswPrivate.table.cell(2, 5);
                        cswPrivate.cells.cell25.empty();
                        cswPrivate.cells.cell25.nodeSelect({
                            objectClassId: cswPrivate.state.inventorygroupid
                        });
                    };
                    buildFilters();

                    Csw.ajax.post({
                        urlMethod: 'getFulfillRequestFilters',
                        data: {
                            SelectedFilters: ''
                        },
                        success: function(data) {
                            $.extend(cswPrivate.state, data.filters);
                            Csw.clientDb.setItem(cswFulfillRequestActionState, cswPrivate.state);
                            buildFilters();
                        }
                    });
                }());
                //#endregion init ctor

                //#region cswPrivate/cswPublic methods and props

                //#endregion cswPrivate/cswPublic methods and props

                //#region final ctor
                (function _post() {

                }());
                //#region final ctor

            });
            //#endregion safety net

            return cswPublic;

        });


}());
