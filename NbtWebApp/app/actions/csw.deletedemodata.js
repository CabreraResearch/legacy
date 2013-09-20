/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {

    Csw.actions.register('deletedemodata', function (cswParent, options) {
        'use strict';
        var o = {
            saveUrlMethod: 'savedeletedemodata',
            name: 'action_deletedemodata',
            actionjson: null,
            onQuotaChange: null // function () { }
        };

        if (options) {
            Csw.extend(o, options);
        }

        o.action = Csw.layouts.action(cswParent, {
            title: 'Delete Demo Data',
            useFinish: false,
            useCancel: false
        });

        //***************************************************************************
        //BEGIN HTML TABLE VOO DOO
        o.action.actionDiv.css({ padding: '10px' });
        o.action.actionDiv
            .span({ text: 'Select the demo data item(s) you wish to convert to non-demo data or to permanently delete.' })
            .br({ number: 2 });

        var action_table = o.action.actionDiv.table();
        //action_table.css({ 'width' : '600px' });
        //Where we are putting stuff

        var grid_cell = action_table.cell(1, 1);
        grid_cell.propDom('colspan', 2);
        //grid_cell.css({'width':'800px'});



        //var include_children_table = action_table.cell(1, 1).table();
        //action_table.cell(1, 1).css( { 'vertical-align' : 'bottom' } );
        //var include_children_checkbox_label_cell = include_children_table.cell(1, 1);
        //var include_children_checkbox_cell = include_children_table.cell(1, 2);


        var mark_all_delete_link_cell = action_table.cell(2, 1);
        mark_all_delete_link_cell.css({ 'padding-top': '5px' });
        mark_all_delete_link_cell.css({ 'padding-left': '5px' });

        var mark_all_to_convert_link_cell = action_table.cell(2, 2);
        mark_all_to_convert_link_cell.css({ 'padding-top': '5px' });

        var spacer = action_table.cell(3, 1);
        spacer.propDom('colspan', 2);
        spacer.br();


        var delete_button_cell = action_table.cell(4, 1);

        var close_button_cell = action_table.cell(4, 2);
        close_button_cell.css({ 'text-align': 'right' });
        //END HTML TABLE VOO DOO
        //***************************************************************************

        //*******************************************
        //BEGIN: GLOBAL VARS FOR CONTROLS
        var mainGrid = null;

        //EMD: GLOBAL VARS FOR CONTROLS
        //*******************************************
        var pools = Csw.object(null, {
            toDelete: { value: new Map() },
            toConvert: { value: new Map() },
            init: { value: new Map() }
        });

        function initGrid() {

            var gridId = 'demoDataGrid';
            grid_cell.empty();
            Csw.ajaxWcf.post({
                urlMethod: 'DemoData/getDemoDataGrid',
                //data: cswPrivate.selectedCustomerId,
                success: function (result) {

                    //see case 29437: Massage row structure
                    Csw.iterate(result.Grid.data.items,
                        function (element, index, array) {
                            Csw.extend(element, element.Row);
                        }
                    ); //foreach on grid rows


                    //"Convert To Non Demo"
                    //"Delete"
                    //The callback ExtJs will execute with our aid to define the render event for this row.
                    var onMakeCustomColumn = function (div, colObj, metaData, record, rowIndex, colIndex) {
                        //Checkboxes will only appear if no child record exists
                        if ((record && record.raw) && ((("Delete" == colObj.header) && (record.raw['is_required_by'] <= 0)) || ("Convert To Non Demo" == colObj.header))) {

                            //Define an object representing the entity to be tracked for modification or deletion
                            var key = record.data.type + '_' + record.raw.nodeid;
                            var demoObj = {
                                type: record.data.type,
                                id: record.raw.nodeid
                            };

                            //Add all objets to the init pool
                            pools.init.set(key, demoObj);

                            //Define the state mechanics for moving the demo object between pools. It should occupy only one.
                            var shiftBetweenPools = function (dest, checked) {
                                var clearInit = (checked === true || dest === 'toDelete' || dest === 'toConvert'),
                                    clearDelete = (checked === false || dest === 'toConvert' || dest === 'init'),
                                    clearConvert = (checked === false || dest === 'toDelete' || dest === 'init');

                                if (true === checked && pools[dest] && false === pools[dest].has(key)) {
                                    pools[dest].set(key, demoObj);
                                }

                                if (clearInit && pools.init.has(key)) {
                                    pools.init.delete(key);
                                }
                                if (clearDelete && pools.toDelete.has(key)) {
                                    pools.toDelete.delete(key);
                                }
                                if (clearConvert && pools.toConvert.has(key)) {
                                    pools.toConvert.delete(key);
                                }

                                Csw.publish('click_deletedemodata_' + rowIndex, { checked: checked, colNo: colIndex });
                            };

                            //Define the binding event between checkboxes in the same row.
                            //Checkboxes are exclusive--only one can be checked.
                            var bindWithPeer = function (e, obj) {
                                if (colIndex !== obj.colNo) {
                                    if (obj.checked) {
                                        checkBox.hide();
                                        checkBox.checked(false);
                                    } else {
                                        checkBox.show();
                                    }
                                }
                            };

                            //Define the checkbox and a click event
                            var checkBox = div.input({
                                type: Csw.enums.inputTypes.checkbox,
                                canCheck: true,
                                value: false,
                                onClick: function () {
                                    if (checkBox.checked() === true) {
                                        switch (colObj.header) {
                                            case 'Delete':
                                                shiftBetweenPools('toDelete', true);
                                                break;
                                            default:
                                                shiftBetweenPools('toConvert', true);
                                                break;
                                        }
                                    } else {
                                        shiftBetweenPools('init', false);
                                    }
                                }
                            });

                            //Subscibe this row to the callback initiated by the click event of another checkbox in this row
                            Csw.subscribe('click_deletedemodata_' + rowIndex, bindWithPeer);

                            //Show checkBox method. We'll rely on the state mechanics of the checkBox's click event to manage the visibility of peers 
                            var showCheckBox = function () {
                                checkBox.show();
                                if (false === checkBox.checked()) {
                                    checkBox.click();
                                }
                            };

                            //Subscribe all rows to a "check all" event to be defined at a later time
                            switch (colObj.header) {
                                case 'Delete':
                                    Csw.subscribe('deleteall_deletedemodata', showCheckBox);
                                    break;
                                default:
                                    Csw.subscribe('convertall_deletedemodata', showCheckBox);
                                    break;
                            }

                        }
                    }; //onMakeCustomColumn

                    mainGrid = grid_cell.grid({
                        name: gridId,
                        makeCustomColumns: true,
                        customColumns: [result.ColumnIds.convert_to_non_demo, result.ColumnIds.delete],
                        onMakeCustomColumn: onMakeCustomColumn,
                        storeId: gridId,
                        data: result.Grid,
                        stateId: gridId,
                        height: 375,
                        width: '950px',
                        title: 'Demo Data',
                        usePaging: false,
                        showActionColumn: false,
                        canSelectRow: false,
                        onButtonRender: function (div, colObj, thisBtn) {
                            var nodeData = Csw.deserialize(thisBtn[0].menuoptions);
                            var NodeIds;
                            if (("Is Used By" === colObj.header) && nodeData.usedby) {

                                NodeIds = nodeData.usedby;
                            } else if (("Is Required By" === colObj.header) && nodeData.requiredby) {
                                NodeIds = nodeData.requiredby;
                            }
                            if (NodeIds && NodeIds.length > 0) {
                                var CswDemoNodesGridRequest = {
                                    NodeIds: NodeIds
                                };

                                div.a({
                                    text: NodeIds.length,
                                    onClick: function () {
                                        $.CswDialog('RelatedToDemoNodesDialog', {
                                            relatedNodesGridRequest: CswDemoNodesGridRequest,
                                            relatedNodeName: nodeData.nodename,
                                            onCloseDialog: function () {
                                                initGrid();
                                            }
                                        }); //CswDialog()
                                    } //onClick() 
                                }); //div a

                            } else {
                                div.p({ text: '0' });
                            } //if-else there are related nodes

                        }, //onRender
                        reapplyViewReadyOnLayout: true,
                        topToolbarCustomItems: [{
                            xtype: 'button',
                            text: 'Check All Convert',
                            handler: Csw.method(function () {
                                Csw.publish('convertall_deletedemodata');
                            })
                        }, {
                            xtype: 'button',
                            text: 'Check All Delete',
                            handler: Csw.method(function () {
                                Csw.publish('deleteall_deletedemodata');
                            })
                        }]

                    }); //grid.cell.grid() 

                } //success of post() 

            }); //post

        } //initGrid()


        function initButtons() {

            delete_button_cell.buttonExt({
                name: 'delete_button_action',
                disableOnClick: false,
                onClick: function () {

                    var request = {};
                    request.NodeIds = [];

                    request.node_ids_convert_to_non_demo = [];
                    request.view_ids_convert_to_non_demo = [];

                    request.node_ids_delete = [];
                    request.view_ids_delete = [];

                    var itemsToConvert = pools.toConvert.values();
                    Csw.iterate(itemsToConvert, function (obj) {
                        if (obj.type == "View") {
                            request.view_ids_convert_to_non_demo.push(obj.id);
                        } else {
                            request.node_ids_convert_to_non_demo.push(obj.id);
                        }
                    });

                    var itemsToDelete = pools.toDelete.values();
                    Csw.iterate(itemsToDelete, function (obj) {
                        if (obj.type == "View") {
                            request.view_ids_delete.push(obj.id);
                        } else {
                            request.node_ids_delete.push(obj.id);
                        }
                    });

                    Csw.ajaxWcf.post({
                        urlMethod: 'DemoData/updateDemoData',
                        data: request,
                        success: function (ajaxdata) {
                            //initCheckBox();
                            initGrid();
                        }
                    });

                },
                enabledText: 'Apply Changes'
            });

            close_button_cell.buttonExt({
                name: 'close_action',
                disableOnClick: false,
                onClick: function () {
                    Csw.tryExec(options.onCancel);
                },
                enabledText: 'Close'
            });

        } //initButtons() 


        initGrid();
        initButtons();



    }); // methods
}());
