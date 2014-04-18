/// <reference path="~/app/CswApp-vsdoc.js" />



(function () {
    'use strict';


    Csw.composites.register('permissionsGrid', function (cswParent, options) {

        var cswPrivate = {
            $parent: '',
            name: '',
            propname: '',
            dataModel: {},
            multiple: true,
            cssclass: '',
            onChange: function () {},
            onValidate: null,
            isControl: false,
            EditMode: '',
            required: false,
            nodeId: '',
            showMore: true,
        };

        var cswPublic = {};
        var permissionsCtrl;


        (function () {

            if (options) {
                Csw.extend(cswPrivate, options);
            }
            //create the cells for the various elements of the composite
            var parentDiv = cswParent.div({ name: 'permissionsDiv' });
            var table = parentDiv.table({ name: 'tbl' });
            var moreDivCell = table.cell(1, 1);
            var permissionsGridCell = table.cell(2, 1);

            cswPrivate.morediv = moreDivCell.moreDiv({ name: cswPrivate.name + '_morediv' });

            permissionsGridCell.hide();
            cswPublic = Csw.dom({}, cswPrivate.select);
            
            //now create the Ext representation of the data
            var dataModel = Ext.define('PermissionModel', {
                extend: 'Ext.data.Model',
                fields: [
                    { name: 'itemname', type: 'string' },
                    { name: 'create', type: 'bool' },
                    { name: 'view', type: 'bool' },
                    { name: 'edit', type: 'bool' },
                    { name: 'delete', type: 'bool' },
                    { name: 'itemid', type: 'int' },
                ]
            });

            cswPrivate.store = Ext.create('Ext.data.TreeStore', {
                model: 'PermissionModel',
                root: cswPrivate.dataModel,
                storeId: "permissionsStore",
            });

            cswPrivate.store.sort('itemname', 'ASC');

            var getReadOnlyText = function (start, end) {
                var outputLines = [];

                Csw.each(cswPrivate.store.getRootNode().childNodes, function (nodetype) {
                    //we only want to display rows where we have permissions when in readonly
                    if (nodetype.data.create || nodetype.data.edit || nodetype.data.view || nodetype.data.delete) {

                        var thisLine = nodetype.data.itemname + ": ";

                        var permissionLine = "";
                        Csw.each(["Create", "View", "Edit", "Delete"], function (permission) {
                            if (nodetype.data[permission.toLowerCase()]) {
                                permissionLine += ", " + permission;
                            }
                        });
                        thisLine += permissionLine.substr(1);
                        outputLines.push(thisLine);

                        Csw.each(nodetype.childNodes, function (tab) {
                            if (tab.data.create || tab.data.edit || tab.data.view || tab.data.delete) {
                                thisLine = nodetype.data.itemname + ", " + tab.data.itemname + ": ";

                                permissionLine = "";
                                Csw.each(["Create", "View", "Edit", "Delete"], function (permission) {
                                    if (tab.data[permission.toLowerCase()]) {
                                        permissionLine += ", " + permission;
                                    }
                                });
                                thisLine += permissionLine.substr(1);
                                outputLines.push(thisLine);
                            }//if there are permissions on the tab
                        });//for each tab on this nodetype
                    }//if there are permissions on the nodetype
                });//for each nodetype permission row

                start = start || 0;
                end = end || outputLines.length;
                return outputLines.slice(start, end).join("<br />");
            };//getReadOnlyText()

            cswPrivate.morediv.shownDiv.span({ text: getReadOnlyText(0, 5) });
            cswPrivate.morediv.hiddenDiv.span({ text: getReadOnlyText(6) });
            cswPrivate.morediv.moreLink.show();


            
            var makePermissionsGrid = function(inDialog, div, height, width) {

                var sanitizeName = function(name) {
                    var invalidCharacters = [',', ' ', '(', ')', '.'];
                    invalidCharacters.forEach(function(char) {
                        name = name.split(char).join("");
                    });
                    return name;
                };

                //The Ext.JS renderer requires we return raw HTML text to display the column... things are about to get ugly
                var checkboxRenderer = function(value, styledata, record, rowIndex, colIndex, view) {
                    var field = record.fields.getAt(colIndex).name;
                    var divId = 'ntperms' + sanitizeName(record.internalId) + "_" + field;

                    Csw.defer(Csw.method(function() {

                        var div = Csw.domNode({
                            ID: divId,
                            tagName: 'DIV'
                        });

                        //we only want to display checkboxes for Edit/View on tabs
                        if (record.isLeaf() == false
                            || (field != "create" && field != "delete")) {
                            record.checkboxes = record.checkboxes || {};

                            //see if the permission is not on the top level, and if not, whether we should be rendering this checkbox disabled
                            var parentIsChecked = (record.getDepth() > 1 && record.parentNode.get(field) == true);

                            record.checkboxes[field] = div.input({
                                type: Csw.enums.inputTypes.checkbox,
                                checked: value || parentIsChecked,
                                disabled: parentIsChecked,
                                onClick: function() {
                                    var newState = record.checkboxes[field].checked();
                                    record.set(field, newState);

                                    if (record.isLeaf() == false) {
                                        record.childNodes.forEach(function(tabPermission) {
                                            if (tabPermission.checkboxes != undefined && tabPermission.checkboxes[field] !== undefined) {
                                                if (newState == true) {
                                                    tabPermission.checkboxes[field].checked(true);
                                                    tabPermission.checkboxes[field].disabled(true);
                                                } else {
                                                    tabPermission.checkboxes[field].checked(tabPermission.get(field));
                                                    tabPermission.checkboxes[field].disabled(false);
                                                }
                                            }
                                        });
                                    }

                                    Csw.tryExec(cswPrivate.onChange);

                                }//onClick()
                            });

                        } //if the box is for a nodetype or E/V on a tab

                    }), 100);
                    return '<div id="' + divId + '" style="height:18px;"></div>';

                }; //checkboxRenderer()

                var permissionsGrid = window.Ext.create('Ext.tree.Panel', {
                    columns: [
                        { text: 'Permission', xtype: 'treecolumn', dataIndex: 'itemname' },
                        { draggable: false, width: 20, text: 'Create', xtype: 'booleancolumn', dataIndex: 'create', renderer: checkboxRenderer },
                        { draggable: false, width: 20, text: 'View', xtype: 'booleancolumn', dataIndex: 'view', renderer: checkboxRenderer },
                        { draggable: false, width: 20, text: 'Edit', xtype: 'booleancolumn', dataIndex: 'edit', renderer: checkboxRenderer },
                        { draggable: false, width: 20, text: 'Delete', xtype: 'booleancolumn', dataIndex: 'delete', renderer: checkboxRenderer },
                    ],
                    layout: 'fit',
                    forceFit: true,
                    height: height,
                    width: width,
                    autoScroll: true,
                    renderTo: div[0],
                    store: cswPrivate.store,
                    rootVisible: false,
                }); //permissionsGrid

            };//makePermissionsGrid

            if (cswPrivate.EditMode === Csw.enums.editMode.Add
                 || cswPrivate.EditMode === Csw.enums.editMode.Edit) {
                cswPrivate.morediv.hide();
                permissionsCtrl = makePermissionsGrid(false, parentDiv, 400, 450);
            }
        }());//pre-constructor

        


        cswPublic.val = function () {
            //! Once CIS-53434 is complete, we should delete the compatibleString function and return something closer to what we're using.
            return cswPublic.getMultiListCompatibleString();
        };

        cswPublic.getValue = function () { //need func with this name for the Csw.validator
            return cswPublic.val();
        };


        
        cswPublic.getMultiListCompatibleString = function() {
            var resultString = "";
            //if the control hasn't been rendered yet, iterate through the data sent in to build the value
            if (cswPrivate.store == undefined) {
                Csw.each(cswPrivate.dataModel.children, function(nodetype) {
                    Csw.each(["create", "edit", "view", "delete"], function(permission) {
                        if (nodetype[permission] == true) {
                            resultString += ",nt_" + nodetype.itemid + "_" + permission;
                        }
                    });

                    Csw.each(nodetype.children, function(tab) {
                        Csw.each(["edit", "view"], function(permission) {
                            if (tab[permission] == true) {
                                resultString += ",nt_" + nodetype.itemid + "_tab_" + tab.itemid + "_" + permission;
                            }
                        });
                    });
                });
            } else {
                //if the control has been rendered, we want the most recent copy of the data from the ext store
                Csw.each(cswPrivate.store.getRootNode().childNodes, function (nodetype) {
                    Csw.each(["create", "edit", "view", "delete"], function (permission) {
                        if (nodetype.data[permission] == true) {
                            resultString += ",nt_" + nodetype.data.itemid + "_" + permission;
                        }
                    });

                    Csw.each(nodetype.childNodes, function (tab) {
                        Csw.each(["edit", "view"], function (permission) {
                            if (tab.data[permission] == true) {
                                resultString += ",nt_" + nodetype.data.itemid + "_tab_" + tab.data.itemid + "_" + permission;
                            }
                        });
                    });
                });
            }
            //remove the extra comma at the beginning
            resultString = resultString.substr(1);
            
            return resultString;
        };
        

        return cswPublic;
    });


}());
