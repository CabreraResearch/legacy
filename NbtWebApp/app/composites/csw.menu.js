/// <reference path="~/vendor/extjs-4.1.0/ext-all-debug.js" />
/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    'use strict';

    Csw.composites.menu = Csw.composites.menu ||
        Csw.composites.register('menu', function (cswParent, options) {

            var cswPrivate = {
                ajax: {
                    urlMethod: '',
                    data: {}
                },
                width: 240,
                //onClick: null,  // function(itemName, itemJson)

                // Menu item handlers
                onLogout: null, // function () { },
                onAlterNode: null, // function (nodeid, nodekey) { },
                onMultiEdit: null, //function () { },
                onEditView: null, //function (viewid) { },
                onSaveView: null, //function (newviewid) { },
                onPrintView: null,  // function () { },
                onQuotas: null, // function () { },
                onModules: null, // function () { },
                onSessions: null, // function () { },
                onSubscriptions: null,
                onImpersonate: null,
                nodeTreeCheck: null,
                Multi: false
            };
            var cswPublic = {};

            cswPrivate.getSelectedNodes = function (menuItemJson) {
                var ret = {};
                var nodechecks = null;

                if (false == Csw.isNullOrEmpty(cswPrivate.nodeTreeCheck)) {
                    nodechecks = Csw.tryExec(cswPrivate.nodeTreeCheck.checkedNodes);
                }
                if (false === Csw.isNullOrEmpty(nodechecks, true)) {
                    Csw.each(nodechecks, function (thisObj) {
                        ret[thisObj.nodeid] = {
                            nodeid: thisObj.nodeid,
                            nodekey: thisObj.nodekey,
                            nodename: thisObj.nodename
                        };
                    });
                }
                if (Csw.isNullOrEmpty(ret)) {
                    ret[menuItemJson.nodeid] = {
                        nodeid: menuItemJson.nodeid,
                        nodename: menuItemJson.nodename,
                        nodetypeid: menuItemJson.nodetypeid
                    };
                }
                return ret;
            };

            cswPrivate.handleMenuItemClick = function (menuItemName, menuItemJson) {
                if (false === Csw.isNullOrEmpty(menuItemJson)) {

                    if (false === Csw.isNullOrEmpty(menuItemJson.href)) {
                        Csw.window.location(menuItemJson.href);

                    } else if (false === Csw.isNullOrEmpty(menuItemJson.popup)) {
                        window.open(menuItemJson.popup);

                    } else if (false === Csw.isNullOrEmpty(menuItemJson.action)) {

                        var nodeid = Csw.string(menuItemJson.nodeid);
                        var nodename = Csw.string(menuItemJson.nodename);
                        var nodetypeid = Csw.string(menuItemJson.nodetypeid);
                        var viewid = Csw.string(menuItemJson.viewid);

                        switch (menuItemJson.action) {
                            case 'About':
                                $.CswDialog('AboutDialog');
                                break;
                            case 'AddNode':
                                $.CswDialog('AddNodeDialog', {
                                    text: "New " + menuItemName,
                                    nodetypeid: Csw.string(menuItemJson.nodetypeid),
                                    relatednodeid: Csw.string(menuItemJson.relatednodeid), //for Grid Props
                                    relatednodename: Csw.string(menuItemJson.relatednodename), //for Grid Props
                                    relatednodetypeid: Csw.string(menuItemJson.relatednodetypeid), //for NodeTypeSelect
                                    relatedobjectclassid: Csw.string(menuItemJson.relatedobjectclassid),
                                    onAddNode: cswPrivate.onAlterNode
                                });
                                break;
                            case 'AddFeedback':
                                $.CswDialog('AddFeedbackDialog', {
                                    text: menuItemName,
                                    nodetypeid: Csw.string(menuItemJson.nodetypeid),
                                    onAddNode: cswPrivate.onAlterNode
                                });
                                break;
                            case 'Clear Cache':
                                window.location.reload(true);
                                break;
                            case 'DeleteNode':
                                $.CswDialog('DeleteNodeDialog', {
                                    nodes: cswPrivate.getSelectedNodes(menuItemJson),
                                    onDeleteNode: cswPrivate.onAlterNode,
                                    nodeTreeCheck: cswPrivate.nodeTreeCheck,
                                    Multi: cswPrivate.Multi
                                });
                                break;
                            case 'DeleteDemoNodes':
                                $.CswDialog('ConfirmDialog', 'You are about to delete all demo data nodes from the database.<br>This could take a few minutes to complete. Are you sure?<br>', 'Delete All Demo Data', function () {
                                    Csw.ajax.post({
                                        url: Csw.enums.ajaxUrlPrefix + 'DeleteDemoDataNodes',
                                        success: function (data) {
                                            var onOpen = function(dialogDiv) {
                                                var statusDiv = dialogDiv.div();
                                                statusDiv.span({ text: data.successtext });
                                                statusDiv.span({ text: data.failedtext });
                                                statusDiv.br();
                                                
                                                var table = dialogDiv.table({
                                                    cellpadding: '3px',
                                                    FirstCellRightAlign: true
                                                });
                                                var f = 1,s;
                                                if (data.counts) {
                                                    table.cell(1, 1).append('<b>Status</b>');
                                                    table.cell(1, 2).append('<b>Type</b>');
                                                    table.cell(1, 3).append('<b>Name</b>');
                                                    table.cell(1, 4).append('<b>Id</b>');
                                                    table.cell(1, 5).append('<b>Link</b>');
                                                    if (Csw.number(data.counts.failed) > 0) {
                                                       Csw.each(data.failed, function(failObj) {
                                                            f += 1;
                                                            table.cell(f, 1).append('Failed');
                                                            table.cell(f, 2).append(failObj.type);
                                                            table.cell(f, 3).append(failObj.name);
                                                            table.cell(f, 4).append(failObj.id);
                                                            if (failObj.link) {
                                                                table.cell(f, 5).nodeLink({ text: failObj.link });
                                                            } else {
                                                                table.cell(f, 5).append('No link. This may have deleted correctly.');
                                                            }
                                                       });
                                                        s = 2 + data.counts.failed;
                                                    }
                                                    if (Csw.number(data.counts.succeeded) > 0) {
                                                        
                                                        Csw.each(data.succeeded, function (successObj) {
                                                            s += 1;
                                                            table.cell(s, 1).append('Succeeded');
                                                            table.cell(s, 2).append(successObj.type);
                                                            table.cell(s, 3).append(successObj.name);
                                                            table.cell(s, 4).append(successObj.id);
                                                        });
                                                    }  
                                                }
                                                if(data.exceptions) {
                                                    Csw.each(data.exceptions, function(ex) {
                                                        Csw.debug.error(ex);
                                                    });
                                                }
                                                Csw.publish(Csw.enums.events.main.refreshHeader);
                                            };
                                            $.CswDialog('AlertDialog', '', 'Finished ' + Csw.string(data.counts.total) + ' deletes', Csw.goHome, 800, 600, onOpen);
                                        }
                                    });
                                }, 'Cancel');
                                break;
                            case 'editview':
                                Csw.tryExec(cswPrivate.onEditView, viewid);
                                break;
                            case 'CopyNode':
                                $.CswDialog('CopyNodeDialog', {
                                    nodename: nodename,
                                    nodeid: nodeid,
                                    nodetypeid: nodetypeid,
                                    onCopyNode: cswPrivate.onAlterNode
                                });
                                break;
                            case 'PrintView':
                                Csw.tryExec(cswPrivate.onPrintView);
                                break;
                            case 'PrintLabel':
                                $.CswDialog('PrintLabelDialog', {
                                    nodes: cswPrivate.getSelectedNodes(menuItemJson),
                                    nodetypeid: Csw.string(menuItemJson.nodetypeid)
                                });
                                break;
                            case 'Logout':
                                Csw.tryExec(cswPrivate.onLogout);
                                break;
                            case 'Home':
                                Csw.goHome();
                                break;
                            case 'Profile':
                                $.CswDialog('EditNodeDialog', {
                                    currentNodeId: menuItemJson.userid,
                                    filterToPropId: '',
                                    title: 'User Profile',
                                    onEditNode: null // function (nodeid, nodekey) { }
                                });
                                break;
                            case 'multiedit':
                                Csw.tryExec(cswPrivate.onMultiEdit);
                                break;
                            case 'SaveViewAs':
                                $.CswDialog('AddViewDialog', {
                                    viewid: viewid,
                                    viewmode: Csw.string(menuItemJson.viewmode),
                                    onAddView: cswPrivate.onSaveView
                                });
                                break;
                            case 'Quotas':
                                Csw.tryExec(cswPrivate.onQuotas);
                                break;
                            case 'Modules':
                                Csw.tryExec(cswPrivate.onModules);
                                break;
                            case 'Sessions':
                                Csw.tryExec(cswPrivate.onSessions);
                                break;
                            case 'Subscriptions':
                                Csw.tryExec(cswPrivate.onSubscriptions);
                                break;
                            case 'Impersonate':
                                $.CswDialog('ImpersonateDialog', { onImpersonate: cswPrivate.onImpersonate });
                                break;
                            case 'EndImpersonation':
                                Csw.tryExec(cswPrivate.onEndImpersonation);
                                break;
                            case 'Submit_Request':
                                Csw.tryExec(cswPrivate.onSubmitRequest);
                                break;
                            default:
                                Csw.main.handleAction({ actionname: menuItemJson.action });
                                break;
                        } // switch(menuItemJson.action)
                    } // else if (false === Csw.isNullOrEmpty(menuItemJson.action))
                } // if( false === Csw.isNullOrEmpty(menuItemJson))
            }; // handleMenuItemClick()

            cswPrivate.parseMenuItems = function (itemColl) {
                var items = [];
                Csw.each(itemColl, function (childItem, childItemName) {
                    if (childItemName != 'haschildren') {
                        var thisItem = {
                            text: childItemName,
                            icon: childItem.icon,
                            cls: 'menuitem'
                        };

                        if (Csw.bool(childItem.haschildren)) {
                            thisItem.menu = { items: cswPrivate.parseMenuItems(childItem) };
                        } else {
                            thisItem.listeners = {
                                click: function (item, event) {
                                    cswPrivate.handleMenuItemClick(childItemName, childItem);
                                }
                            };
                        }
                        items.push(thisItem);
                    } // if (childItemName != 'haschildren') {
                }); // each
                return items;
            }; // parseItems()

            //constructor
            (function () {
                Csw.extend(cswPrivate, options);

                cswPublic.ajax = Csw.ajax.post({
                    urlMethod: cswPrivate.ajax.urlMethod,
                    data: cswPrivate.ajax.data,
                    success: function (result) {

                        var items = [];
                        Csw.each(result, function (menuItem, menuItemName) {
                            if (items.length > 0) {
                                items.push({ xtype: 'tbseparator' });
                            }

                            var thisItem = {};
                            if (Csw.bool(menuItem.haschildren)) {
                                // Child items
                                thisItem = {
                                    xtype: 'splitbutton',
                                    text: menuItemName,
                                    menu: { items: [] },
                                    listeners: {
                                        click: function (button, event) {
                                            button.showMenu(); // open the menu on click, not just on arrowclick
                                        }
                                    },
                                    cls: 'menuitem'
                                }; // thisItem

                                thisItem.menu.items = cswPrivate.parseMenuItems(menuItem);
                            } // if (Csw.bool(menuItem.haschildren))
                            else {
                                // Root Items
                                thisItem = {
                                    xtype: 'button',
                                    text: menuItemName,
                                    listeners: {
                                        click: function (item, event) {
                                            cswPrivate.handleMenuItemClick(menuItemName, menuItem);
                                        }
                                    },
                                    cls: 'menuitem'
                                };
                            }
                            items.push(thisItem);
                        }); // each

                        cswParent.empty();

                        if (Csw.isElementInDom(cswParent.getId())) {
                            try {
                                cswPublic.menu = window.Ext.create('Ext.toolbar.Toolbar', {
                                    id: cswPrivate.ID + 'toolbar',
                                    renderTo: cswParent.getId(),
                                    width: cswPrivate.width,
                                    items: items,
                                    cls: 'menutoolbar'
                                }); // toolbar
                            } catch (e) {
                                Csw.debug.error('Failed to create Ext.toolbar.Toolbar in csw.menu');
                                Csw.debug.error(e);
                            }
                        } else {
                            cswPublic.menu = window.Ext.create('Ext.toolbar.Toolbar');
                        }
                        //}                                                            
                    }       //success
                }); // ajax
            }()); // constructor

            return cswPublic;
        });


    Csw.goHome = Csw.goHome ||
        Csw.register('goHome', function () {
            'use strict';
            Csw.clientState.clearCurrent();
            Csw.window.location(Csw.getGlobalProp('homeUrl'));
        });

}());
