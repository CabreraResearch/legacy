/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {

    Csw.actions.deletedemodata = Csw.actions.deletedemodata ||
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
            o.action.actionDiv.append("Select the demo data item you wish to remove.<BR><BR>");
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
            var inventoryGroupSelect = null;

            var check_children_of_current_check_box = null;

            var mark_all_delete_link;
            var mark_all_to_convert_link;


            //EMD: GLOBAL VARS FOR CONTROLS
            //*******************************************
            var selectedToDelete = Csw.object();
            var selectedToPromote = Csw.object();
            
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
                        
                        var onMakeCustomColumn = function (div, colObj, metaData, record, rowIndex, colIndex) {
                            if(record && record.raw && (record.raw['is_required_by'] > 0 || record.raw['is_used_by'] > 0 )) {
                                
                                var bindWithPeer = function (e, obj) {
                                    if (colIndex !== obj.colNo) {
                                        if (obj.checked) {
                                            checkBox.hide();
                                        } else {
                                            checkBox.show();
                                        }
                                    }
                                };
                                var checkBox = div.checkBox({
                                    onClick: function(val) {
                                        switch(colObj.header) {
                                            case 'Remove':
                                                selectedToDelete[record.raw.nodeid] = val;
                                                selectedToPromote[record.raw.nodeid] = !val;
                                                break;
                                            default:
                                                selectedToPromote[record.raw.nodeid] = val;
                                                selectedToDelete[record.raw.nodeid] = !val;
                                                break;
                                        }
                                        Csw.publish('click_deletedemodata_' + rowIndex, { checked: val, colNo: colIndex });
                                    }
                                });
                                Csw.subscribe('click_deletedemodata_' + rowIndex, bindWithPeer);
                            }
                        }; //iterate columns

                        mainGrid = grid_cell.grid({
                            name: gridId,
                            makeCustomColumns: true,
                            customColumns: [result.ColumnIds.convert_to_non_demo, result.ColumnIds.remove],
                            onMakeCustomColumn: onMakeCustomColumn,
                            storeId: gridId,
                            data: result.Grid,
                            stateId: gridId,
                            height: 375,
                            width: '950px',
                            forceFit: true,
                            title: 'Demo Data',
                            usePaging: false,
                            showActionColumn: false,
                            canSelectRow: false,
                            selModel: {
                                selType: 'cellmodel'
                            },
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
                                                relatedNodeName: nodeData.nodename
                                            }); //CswDialog()
                                        } //onClick() 
                                    }); //div a

                                } else {
                                    div.p({ text: '0' });
                                } //if-else there are related nodes
                                
                            }, //onRender
                            reapplyViewReadyOnLayout: true

                        }); //grid.cell.grid() 

                    } //success of post() 

                }); //post

            } //initGrid()


            function initLinks() {

                var mark_all_delete_link = mark_all_delete_link_cell.a({
                    text: "Mark All Delete",
                    onClick: function () {
                        mainGrid.checkAllInColumn('delete');
                    }

                });

                var mark_all_to_convert_link = mark_all_to_convert_link_cell.a({
                    text: "Mark All Convert",
                    onClick: function () {
                        //do stuff here
                    }
                });

            } //initLinks()



            function initButtons() {

                delete_button_cell.buttonExt({
                    name: 'delete_button_action',
                    disableOnClick: false,
                    onClick: function () {

                        var request = {};
                        request.NodeIds = [];
                        request.node_ids_convert_to_non_demo = [];
                        request.view_ids_convert_to_non_demo = [];
                        request.node_ids_remove = [];
                        request.view_ids_remove = [];

                        mainGrid.iterateRows(function (row) {

                            if (true === row.data["convert_to_non_demo"]) {
                                if ("View" != row.data.type) {
                                    request.node_ids_convert_to_non_demo.push(row.data.nodeid);
                                } else {
                                    request.view_ids_convert_to_non_demo.push(row.data.nodeid);
                                }
                            }

                            if (true === row.data["remove"]) {
                                if ("View" != row.data.type) {
                                    request.node_ids_remove.push(row.data.nodeid);
                                } else {

                                    request.view_ids_remove.push(row.data.nodeid);
                                }
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
                    enabledText: 'Save Selected'
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
            //initSelectBox();
            //initCheckBox();
            initButtons();
            initLinks();


            //initTable();
        }); // methods
} ());
