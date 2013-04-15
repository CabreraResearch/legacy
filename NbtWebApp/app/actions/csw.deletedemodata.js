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
            var mark_all_to_convert_link_cell = action_table.cell(2, 2);

            var spacer = action_table.cell(3, 1);
            spacer.propDom('colspan', 2);
            spacer.br();


            var delete_button_cell = action_table.cell(4, 1);
            //delete_button_cell.css({ 'vertical-align': 'bottom' });

            var close_button_cell = action_table.cell(4, 2);
            close_button_cell.css({ 'text-align': 'right' });
            //close_button_cell.css({ 'vertical-align': 'bottom' });

            //            var right_side_table = action_table.cell(1, 2).table();
            //            action_table.cell(1, 2).propDom( 'rowspan', 2 ); 
            //            action_table.cell(1, 2).css( { 'vertical-align' : 'top' } );
            //            var select_box_label_cell = right_side_table.cell(1, 1);
            ////            var select_inventory_group_cell = right_side_table.cell(1, 2);
            //            var save_button_cell = right_side_table.cell(2, 2);

            //END HTML TABLE VOO DOO
            //***************************************************************************

            //*******************************************
            //BEGIN: GLOBAL VARS FOR CONTROLS
            var mainTree = null;
            var inventoryGroupSelect = null;

            var check_children_of_current_check_box = null;

            var mark_all_delete_link;
            var mark_all_to_convert_link;


            //EMD: GLOBAL VARS FOR CONTROLS
            //*******************************************

            function initGrid() {

                var gridId = 'demoDataGrid';
                Csw.ajaxWcf.post({
                    urlMethod: 'DemoData/getDemoDataGrid',
                    //data: cswPrivate.selectedCustomerId,
                    success: function (result) {

                        //see case 29437
                        result.Grid.data.items.forEach(function (element, index, array) {
                            Csw.extend(element, element.Row);
                        }
                                                      );
                        mainTree = grid_cell.grid({
                            name: gridId,
                            storeId: gridId,
                            data: result.Grid,
                            stateId: gridId,
                            height: 375,
                            //width: '95%',
                            width: '610px',
                            title: 'Demo Data',
                            usePaging: false,
                            showActionColumn: false,
                            canSelectRow: false,
                            selModel: {
                                selType: 'cellmodel'
                            }
                        });
                    } //success
                }) //post
            } //initGrid()






            /*
            function initSelectBox() {

            var selected_node_id = null;
            if( ( null !== o.actionjson ) && ( null !== o.actionjson.ivgnodeid ) ) 
            {
            selected_node_id = o.actionjson.ivgnodeid;
            }

            inventoryGroupSelect = select_inventory_group_cell.span().nodeSelect({
            name: 'Inventory Group',
            objectClassName: 'InventoryGroupClass',
            selectedNodeId: selected_node_id,
            allowAdd: true,
            isRequired: true,
            showSelectOnLoad: true,
            isMulti: false,
            onSuccess: function () {
            }

            });

            select_box_label_cell.span({ text: 'Inventory Group:' }).addClass('propertylabel');

            } //initSelectBox()
            */

            //check_children_of_current_check_box.checked 
            //            function initCheckBox() {

            //                include_children_checkbox_label_cell.empty();
            //                include_children_checkbox_label_cell.span({ text: 'Include Children:' }).addClass('propertylabel');

            //                include_children_checkbox_cell.empty();
            //                check_children_of_current_check_box = include_children_checkbox_cell.input({
            //                    name: "include_children",
            //                    type: Csw.enums.inputTypes.checkbox,
            //                    checked: Csw.bool("false"),
            //                })
            //            }; //initCheckBox()


            function initLinks() {

                var mark_all_delete_link = mark_all_delete_link_cell.a({
                    text: "Mark All Delete",
                    onClick: function () {
                        //do stuff here
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

                        //                        var inventory_group_node_id = inventoryGroupSelect.selectedVal();
                        //                        var selected_locations_node_keys = '';

                        //                        Csw.each( mainTree.checkedNodes() , function ( node ) {
                        //                            if( null !== node.nodeid ) 
                        //                            {
                        //                                selected_locations_node_keys += node.nodekey + ',';
                        //                            }
                        //                        });

                        //                        var AssignRequest = {  
                        //                            LocationNodeKeys : selected_locations_node_keys,
                        //                            InventoryGroupNodeId : inventory_group_node_id
                        //                        }

                        Csw.ajaxWcf.post({
                            urlMethod: 'Locations/assignInventoryGroupToLocations',
                            //                            data: AssignRequest,
                            success: function (ajaxdata) {
                                initCheckBox();
                                initGrid();
                            }
                        });

                    },
                    enabledText: 'Delete Selected'
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
