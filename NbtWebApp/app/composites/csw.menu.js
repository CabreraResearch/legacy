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
                onLoginData: null,
                onSuccess: null,
                nodeTreeCheck: null,
                Multi: false,
                viewMode: 'Tree'
            };
            var cswPublic = {};

            cswPrivate.getSelectedNodes = function (menuItemJson) {
                var ret = Csw.object();
                var selectedNodes = [];

                if (cswPrivate.viewMode !== 'Grid' && false == Csw.isNullOrEmpty(cswPrivate.nodeTreeCheck)) {
                    selectedNodes = Csw.tryExec(cswPrivate.nodeTreeCheck.checkedNodes);
                }
                if (cswPrivate.viewMode === 'Grid' && false == Csw.isNullOrEmpty(cswPrivate.nodeGrid)) {
                    selectedNodes = cswPrivate.nodeGrid.getSelectedNodes();
                }
                if (false === Csw.isNullOrEmpty(selectedNodes, true)) {
                    Csw.iterate(selectedNodes, function (thisObj) {
                        ret[thisObj.nodeid] = {
                            nodeid: thisObj.nodeid,
                            nodekey: thisObj.nodekey,
                            nodename: thisObj.nodename
                        };
                    });
                }
                if (Csw.isNullOrEmpty(ret) &&
                    !Csw.isNullOrEmpty(menuItemJson.nodeid)) {
                    ret[menuItemJson.nodeid] = {
                        nodeid: menuItemJson.nodeid,
                        nodename: menuItemJson.nodename,
                        nodetypeid: menuItemJson.nodetypeid
                    };
                }
                return ret;
            };

            cswPrivate.handleMenuItemClick = function (menuItemName, menuItemJson, menuItem) {
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
                        var isWholePageNavigation = false; //If we're switching to a completely new context

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
                                if (Csw.clientChanges.manuallyCheckChanges()) {
                                    window.location.reload(true);
                                }
                                break;
                            case 'DeleteNode':
                                Csw.clientChanges.unsetChanged();
                                $.CswDialog('DeleteNodeDialog', {
                                    nodes: cswPrivate.getSelectedNodes(menuItemJson),
                                    onDeleteNode: cswPrivate.onAlterNode,
                                    nodeTreeCheck: cswPrivate.nodeTreeCheck,
                                    Multi: cswPrivate.Multi
                                });
                                break;
                            case 'editview':
                                if (Csw.clientChanges.manuallyCheckChanges()) {
                                    isWholePageNavigation = true;
                                    Csw.tryExec(cswPrivate.onEditView, viewid);
                                }
                                break;
                            case 'CopyNode':
                                if (Csw.clientChanges.manuallyCheckChanges()) {
                                    $.CswDialog('CopyNodeDialog', {
                                        nodename: nodename,
                                        nodeid: nodeid,
                                        nodetypeid: nodetypeid,
                                        onCopyNode: cswPrivate.onAlterNode
                                    });
                                }
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
                                if (Csw.clientChanges.manuallyCheckChanges()) {
                                    isWholePageNavigation = true;
                                    Csw.tryExec(cswPrivate.onLogout);
                                }
                                break;
                            case 'Home':
                                var enable = function () {
                                    menuItem.enable();
                                };
                                if (Csw.clientChanges.manuallyCheckChanges()) {
                                    menuItem.disable();
                                    isWholePageNavigation = true;
                                    Csw.goHome(enable).then(enable);
                                } 
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
                                if (Csw.clientChanges.manuallyCheckChanges()) {
                                    isWholePageNavigation = true;
                                    Csw.tryExec(cswPrivate.onQuotas);
                                }
                                break;
                            case 'Manage Locations':
                                if (Csw.clientChanges.manuallyCheckChanges()) {
                                    isWholePageNavigation = true;
                                    Csw.tryExec(cswPrivate.onQuotas);
                                }
                                break;
                            case 'Delete Demo Data':
                                if (Csw.clientChanges.manuallyCheckChanges()) {
                                    isWholePageNavigation = true;
                                    Csw.tryExec(cswPrivate.onQuotas);
                                }
                                break;
                            case 'Modules':
                                if (Csw.clientChanges.manuallyCheckChanges()) {
                                    isWholePageNavigation = true;
                                    Csw.tryExec(cswPrivate.onModules);
                                }
                                break;
                            case 'Sessions':
                                if (Csw.clientChanges.manuallyCheckChanges()) {
                                    isWholePageNavigation = true;
                                    Csw.tryExec(cswPrivate.onSessions);
                                }
                                break;
                            case 'Subscriptions':
                                if (Csw.clientChanges.manuallyCheckChanges()) {
                                    isWholePageNavigation = true;
                                    Csw.tryExec(cswPrivate.onSubscriptions);
                                }
                                break;
                            case 'Impersonate':
                                if (Csw.clientChanges.manuallyCheckChanges()) {
                                    $.CswDialog('ImpersonateDialog', { onImpersonate: cswPrivate.onImpersonate });
                                }
                                break;
                            case 'EndImpersonation':
                                if (Csw.clientChanges.manuallyCheckChanges()) {
                                    isWholePageNavigation = true;
                                    Csw.tryExec(cswPrivate.onEndImpersonation);
                                }
                                break;
                            case 'Submit_Request':
                                if (Csw.clientChanges.manuallyCheckChanges()) {
                                    isWholePageNavigation = true;
                                    Csw.tryExec(cswPrivate.onSubmitRequest);
                                }
                                break;
                            case 'Login_Data':
                                if (Csw.clientChanges.manuallyCheckChanges()) {
                                    isWholePageNavigation = true;
                                    Csw.tryExec(cswPrivate.onLoginData);
                                }
                                break;
                            case 'NbtManager':
                                if (Csw.clientChanges.manuallyCheckChanges()) {
                                    Csw.tryExec(cswPrivate.onReturnToNbtManager);
                                }
                                break;
                            default:
                                Csw.main.handleAction({ actionname: menuItemJson.action });
                                break;
                        } // switch(menuItemJson.action)

                        if (isWholePageNavigation === true) {
                            //If we're changing the contents of the entire page, make sure all dangling events are torn down
                            Csw.publish('initGlobalEventTeardown');
                            Csw.main.initGlobalEventTeardown();
                        }
                    } // else if (false === Csw.isNullOrEmpty(menuItemJson.action))
                } // if( false === Csw.isNullOrEmpty(menuItemJson))
            }; // handleMenuItemClick()

            cswPrivate.parseMenuItems = function (itemColl) {
                var items = [];
                Csw.iterate(itemColl, function (childItem, childItemName) {
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
                                    cswPrivate.handleMenuItemClick(childItemName, childItem, item);
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
                cswParent.empty();
                
                cswPublic.ajax = Csw.ajax.post({
                    urlMethod: cswPrivate.ajax.urlMethod,
                    data: cswPrivate.ajax.data,
                    success: function (result) {

                        var items = [];
                        Csw.iterate(result, function (menuItem, menuItemName) {
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
                                            cswPrivate.handleMenuItemClick(menuItemName, menuItem, item);
                                        }
                                    },
                                    cls: 'menuitem'
                                };
                            }
                            items.push(thisItem);
                        }); // each

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
                        Csw.tryExec(cswPrivate.onSuccess);
                    }       //success
                }); // ajax

                cswPublic.abort = cswPublic.ajax.abort;


            } ()); // constructor

            return cswPublic;
        });


    Csw.goHome = Csw.goHome ||
        Csw.register('goHome', function (onError) {
            'use strict';
            var toDo = [];
            toDo.push(Csw.clientState.clearCurrent());
            toDo.push(Csw.main.refreshWelcomeLandingPage());
            return Q.all(toDo).fail(onError);
        });

} ());
