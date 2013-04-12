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

            o.action = Csw.layouts.action( cswParent, {
                title: 'Delete Demo Data',
                useFinish: false,
                useCancel: false
                } );

    
            o.action.actionDiv.css( { padding: '10px' } ); 
            o.action.actionDiv.append( "Select the demo data item you wish to remove.<BR><BR>" ); 

            //Where we are putting stuff
            var action_table = o.action.actionDiv.table();

            //action_table.p
//            debugger;
//            action_table.propDom( 'border', '1'); 
//            action_table.css({ width: '100%' }); 
            var tree_cell = action_table.cell(2, 1);
            //tree_cell.css( {'width' : '1500px' } ); 

             
            var include_children_table = action_table.cell(1, 1).table();
            action_table.cell(1, 1).css( { 'vertical-align' : 'bottom' } );
            var include_children_checkbox_label_cell = include_children_table.cell(1, 1);
            var include_children_checkbox_cell = include_children_table.cell(1, 2);


            var close_button_cell = action_table.cell(3, 1);

            var right_side_table = action_table.cell(1, 2).table();
            action_table.cell(1, 2).propDom( 'rowspan', 2 ); 
            action_table.cell(1, 2).css( { 'vertical-align' : 'top' } );
            var select_box_label_cell = right_side_table.cell(1, 1);
            var select_inventory_group_cell = right_side_table.cell(1, 2);
            var save_button_cell = right_side_table.cell(2, 2);


            var mainTree = null;
            var inventoryGroupSelect = null;

            var check_children_of_current_check_box = null;

            function initGrid() {

                var gridId = 'demoDataGrid';
                Csw.ajaxWcf.post({
                    urlMethod: 'DemoData/getDemoDataGrid',
                    //data: cswPrivate.selectedCustomerId,
                    success: function ( result ) {
                          
                          //see case 29437
                          result.Grid.data.items.forEach(  function (element, index, array ) {
                                                            Csw.extend(  element  , element.Row ); 
                                                          }
                                                      ); 
                        mainTree = tree_cell.grid({
                                    name: gridId,
                                    storeId: gridId,
                                    data: result.Grid,
                                    stateId: gridId,
                                    height: 375,
                                    width: '95%',
                                    title: 'Demo Data',
                                    usePaging: false,
                                    showActionColumn: false,
                                    canSelectRow: false,
                                    selModel: {
                                        selType: 'cellmodel'
                                    }
                                });
                      }//success
                }) //post
            }//initGrid()

            /*
            function initGrid() {

                Csw.ajaxWcf.get({
                    urlMethod: "Trees/locations",
                    success: function (data) {
                        tree_cell.empty();
                        mainTree = Csw.nbt.nodeTreeExt(tree_cell, {
                            width: 500,
                            overrideBeforeSelect: true,
                            ExpandAll: true,
                            forSearch: false,
                            PropsToShow: ['inventory group'],
                            onBeforeSelectNode: function ( node , tree ) 
                            { 

                                if( null != check_children_of_current_check_box && true == check_children_of_current_check_box.checked() ) 
                                {
                                    if( false == node.raw.checked ) //in other words, we are now toggling it to checked :-( 
                                    {
                                        mainTree.nodeTree.checkChildrenOfNode( node , true );

                                    } else 
                                    {
                                        mainTree.nodeTree.checkChildrenOfNode( node , false );
                                    }//if the client says to check children of checked node

                                }//if the next state of the node is checked

                                return (false);  //allow selection of multiple node types
                            }, 
                            isMulti: true, //checkboxes
                            state: {
                                viewId: data.NewViewId,
                                viewMode: "tree",
                                includeInQuickLaunch: false
                            }
                        });
                        

                    } //success
                }); //ajaxget

                
            } //initGrid()
            */





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

            //check_children_of_current_check_box.checked 
            function initCheckBox() {

                include_children_checkbox_label_cell.empty();
                include_children_checkbox_label_cell.span({ text: 'Include Children:' }).addClass('propertylabel');

                include_children_checkbox_cell.empty();
                check_children_of_current_check_box = include_children_checkbox_cell.input({
                    name: "include_children",
                    type: Csw.enums.inputTypes.checkbox,
                    checked: Csw.bool("false"),
                })
            }; //initCheckBox()

            function initButtons() {

                save_button_cell.buttonExt({
                    name: 'save_action',
                    disableOnClick: false,
                    onClick: function () {

                        var inventory_group_node_id = inventoryGroupSelect.selectedVal();
                        var selected_locations_node_keys = '';

                        Csw.each( mainTree.checkedNodes() , function ( node ) {
                            if( null !== node.nodeid ) 
                            {
                                selected_locations_node_keys += node.nodekey + ',';
                            }
                        });

                        var AssignRequest = {  
                            LocationNodeKeys : selected_locations_node_keys,
                            InventoryGroupNodeId : inventory_group_node_id
                        }

                        Csw.ajaxWcf.post({
                            urlMethod: 'Locations/assignInventoryGroupToLocations',
                            data: AssignRequest,
                            success: function (ajaxdata) { 
                                    initCheckBox();
                                    initGrid();
                                }
                            });

                    },
                    enabledText: 'Set To'
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
            initSelectBox();
            initCheckBox();
            initButtons();


            //initTable();
        }); // methods
} ());
