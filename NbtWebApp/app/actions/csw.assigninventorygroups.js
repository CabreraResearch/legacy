/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {

    Csw.actions.assigninventorygroups = Csw.actions.assigninventorygroups ||
        Csw.actions.register('assigninventorygroups', function (cswParent, options) {
            'use strict';
            var o = {
                saveUrlMethod: 'saveassigninventorygroups',
                name: 'action_assigninventorygroups',
                onQuotaChange: null // function () { }
            };

            if (options) {
                Csw.extend(o, options);
            }

            //Where we are putting stuff
            var action_table = cswParent.div().table();
//            debugger;
//            action_table.propDom( 'border', '1'); 
//            action_table.css({ width: '100%' }); 
            var tree_cell = action_table.cell(2, 1);

             
            var include_children_table = action_table.cell(1, 1).table();
            action_table.cell(1, 1).css( { 'vertical-align' : 'bottom' } );
            var include_children_checkbox_label_cell = include_children_table.cell(1, 1);
            var include_children_checkbox_cell = include_children_table.cell(1, 2);


            var close_button_cell = action_table.cell(3, 1);

            var right_side_table = action_table.cell(1, 2).table();
            action_table.cell(1, 2).css( { 'vertical-align' : 'bottom' } );
            var select_box_label_cell = right_side_table.cell(1, 1);
            var select_inventory_group_cell = right_side_table.cell(1, 2);
            var save_button_cell = right_side_table.cell(2, 2);


            var mainTree = null;
            var inventoryGroupSelect = null;

            var check_children_of_current_check_box = null;




            function initTree() {

                Csw.ajaxWcf.get({
                    urlMethod: "Trees/locations",
                    success: function (data) {
                        tree_cell.empty();
                        mainTree = Csw.nbt.nodeTreeExt(tree_cell, {
                            width: '500px',
                            overrideBeforeSelect: true,
                            forSearch: false,
                            onBeforeSelectNode: function ( node ) 
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
                            onSelectNode: function (optSelect) {           
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

            } //initTree()

            function initSelectBox() {

                    inventoryGroupSelect = select_inventory_group_cell.span().nodeSelect({
                    name: 'intentory_group_select',
                    objectClassName: 'InventoryGroupClass',
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

                include_children_checkbox_label_cell.span({ text: 'Include Children:' }).addClass('propertylabel');
                check_children_of_current_check_box = include_children_checkbox_cell.input({
                    name: "include_children",
                    type: Csw.enums.inputTypes.checkbox,
                    checked: Csw.bool("false"),
                })
            }; //initCheckBox()

            function initButtons() {

                save_button_cell.button({
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
                                    initTree();
                                }
                            });

                    },
                    enabledText: 'Set To'
                });

                close_button_cell.button({
                    name: 'close_action',
                    disableOnClick: false,
                    onClick: function () {
                        Csw.tryExec(options.onCancel);
                        //cswParent.clearState();
                    },
                    enabledText: 'Close'
                });

            } //initButtons() 


            initTree();
            initSelectBox();
            initCheckBox();
            initButtons();

            /*
            var form;
            var table;
            var row;
            var quotaJson;
            var saveBtn;



            function initTable() {
            div.empty();
            form = div.form();
            table = form.table({
            suffix: 'tbl',
            border: 1,
            cellpadding: 5
            });
            row = 1;

            // Header row
            table.cell(row, 1).span({ cssclass: 'CswThinGridHeaderShow', text: 'Object Class' });
            table.cell(row, 2).span({ cssclass: 'CswThinGridHeaderShow', text: 'Node Types' });
            table.cell(row, 3).span({ cssclass: 'CswThinGridHeaderShow', text: 'Current Usage' });
            table.cell(row, 4).span({ cssclass: 'CswThinGridHeaderShow', text: 'Quota' });
            table.cell(row, 5).span({ cssclass: 'CswThinGridHeaderShow', text: 'Exclude In Meter' });
            row += 1;

            // Quota table
            Csw.ajax.post({
            urlMethod: o.urlMethod,
            data: {},
            success: function (result) {
            quotaJson = result;
            var canedit = Csw.bool(quotaJson.canedit);

            Csw.eachRecursive(quotaJson.objectclasses, function (childObj) {
            if (Csw.number(childObj.nodetypecount) > 0) {

            // one object class row                                
            makeQuotaRow(row, canedit, 'OC' + childObj.objectclassid, childObj.objectclass, '', childObj.currentusage, childObj.quota, childObj.excludeinquotabar);
            row += 1;

            // several nodetype rows
            Csw.eachRecursive(childObj.nodetypes, function (childObjNt) {
            makeQuotaRow(row, canedit, 'NT' + childObjNt.nodetypeid, '', childObjNt.nodetypename, childObjNt.currentusage, childObjNt.quota, childObjNt.excludeinquotabar);
            row += 1;
            }, false);
            }
            }, false); // Csw.eachRecursive()

            if (canedit) {
            saveBtn = div.button({
            name: 'Save',
            enabledText: 'Save',
            disabledText: 'Saving',
            onClick: handleSave
            });
            }
            } // success
            }); // ajax()
            } // initTable()
            */

            /*
            function makeQuotaRow(qRow, canedit, rowid, objectclass, nodetype, currentusage, quota, excludeinquotabar) {
            // one object class row                                
            var cell4;
            var cell5;
            table.cell(qRow, 1).text(objectclass);
            table.cell(qRow, 2).text(nodetype);
            table.cell(qRow, 3).text(currentusage);

            if (canedit) {
            cell4 = table.cell(qRow, 4);
            var numBox = cell4.numberTextBox({
            name: o.name + rowid + 'quota',
            width: '15px',
            value: quota,
            MinValue: 0,
            Precision: 0
            });

            cell5 = table.cell(qRow, 5);
            cell5.input({
            name: o.name + rowid + 'excludeinquotabar',
            type: Csw.enums.inputTypes.checkbox,
            checked: excludeinquotabar
            });

            } else {
            table.cell(qRow, 4).text(quota);
            table.cell(qRow, 5).text(excludeinquotabar);
            }
            } // makeQuotaRow()
            */

            /*
            function handleSave() {

            if (form.isFormValid()) {
            Csw.eachRecursive(quotaJson.objectclasses, function (childObj) {
            childObj.quota = $('[name="' + o.name + 'OC' + childObj.objectclassid + 'quota"]').val();
            childObj.excludeinquotabar = $('[name="' + o.name + 'OC' + childObj.objectclassid + 'excludeinquotabar"]').is(':checked');
            Csw.eachRecursive(childObj.nodetypes, function (childObjNt) {
            childObjNt.quota = $('[name="' + o.name + 'NT' + childObjNt.nodetypeid + 'quota"]').val();
            childObjNt.excludeinquotabar = $('[name="' + o.name + 'NT' + childObjNt.nodetypeid + 'excludeinquotabar"]').is(':checked');
            }, false);
            }, false);

            Csw.ajax.post({
            urlMethod: o.saveUrlMethod,
            data: { assigninventorygroups: JSON.stringify(quotaJson) },
            success: function () {
            initTable();
            Csw.tryExec(o.onQuotaChange);
            }
            });
            } else {
            saveBtn.quickTip({ html: 'One or more of the set assigninventorygroups are invalid' });
            saveBtn.enable();
            }
            }

            // handleSave()
            */

            //initTable();
        }); // methods
} ());
