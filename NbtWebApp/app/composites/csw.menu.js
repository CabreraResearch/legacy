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

                useCache: false,

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

            var menuAction = Csw.object();
            menuAction.add('About', function () { $.CswDialog('AboutDialog'); });
            menuAction.add('AddNode', function (menuItemName, menuItemJson, menuItem) {
                Csw.dialogs.addnode({
                    title: 'Add New ' + menuItemName,
                    nodetypeid: Csw.string(menuItemJson.nodetypeid),
                    relatednodeid: Csw.string(menuItemJson.relatednodeid), //for Grid Props
                    onAddNode: cswPrivate.onAlterNode
                });
            });
            menuAction.add('AddFeedback', function (menuItemName, menuItemJson, menuItem) {
                $.CswDialog('AddFeedbackDialog', {
                    text: menuItemName,
                    nodetypeid: Csw.string(menuItemJson.nodetypeid),
                    onAddNode: cswPrivate.onAlterNode
                });
            });
            menuAction.add('Clear Cache', function (menuItemName, menuItemJson, menuItem) {
                if (Csw.clientChanges.manuallyCheckChanges()) {
                    window.location.reload(true);
                }
            });
            menuAction.add('DeleteNode', function (menuItemName, menuItemJson, menuItem) {
                Csw.clientChanges.unsetChanged();
                $.CswDialog('DeleteNodeDialog', {
                    nodes: cswPrivate.getSelectedNodes(menuItemJson),
                    onDeleteNode: cswPrivate.onAlterNode,
                    nodeTreeCheck: cswPrivate.nodeTreeCheck,
                    Multi: cswPrivate.Multi
                });
            });
            menuAction.add('editview', function (menuItemName, menuItemJson, menuItem) {
                if (Csw.clientChanges.manuallyCheckChanges()) {
                    Csw.tryExec(cswPrivate.onEditView, Csw.string(menuItemJson.viewid));
                    return true;  //isWholePageNavigation
                }
            });
            menuAction.add('CopyNode', function (menuItemName, menuItemJson, menuItem) {
                if (Csw.clientChanges.manuallyCheckChanges()) {
                    Csw.dialogs.copynode({
                        copyType: Csw.string(menuItemJson.copytype),
                        nodename: Csw.string(menuItemJson.nodename),
                        nodeid: Csw.string(menuItemJson.nodeid),
                        nodetypeid: Csw.string(menuItemJson.nodetypeid),
                        onCopyNode: cswPrivate.onAlterNode
                    });
                }
            });
            menuAction.add('PrintView', function (menuItemName, menuItemJson, menuItem) { Csw.tryExec(cswPrivate.onPrintView); });
            menuAction.add('PrintLabel', function (menuItemName, menuItemJson, menuItem) {
                $.CswDialog('PrintLabelDialog', {
                    nodes: cswPrivate.getSelectedNodes(menuItemJson),
                    nodetypeid: Csw.string(menuItemJson.nodetypeid)
                });
            });
            menuAction.add('Logout', function (menuItemName, menuItemJson, menuItem) {
                if (Csw.clientChanges.manuallyCheckChanges()) {
                    Csw.tryExec(cswPrivate.onLogout);
                    return true;  //isWholePageNavigation
                }
            });
            menuAction.add('Home', function (menuItemName, menuItemJson, menuItem) {
                var enable = function () {
                    menuItem.enable();
                };
                menuItem.disable();
                if (Csw.clientChanges.manuallyCheckChanges()) {
                    Csw.goHome().then(enable);
                    return true;  //isWholePageNavigation
                } else {
                    enable();
                }
            });
            menuAction.add('Profile', function (menuItemName, menuItemJson, menuItem) {
                $.CswDialog('EditNodeDialog', {
                    currentNodeId: menuItemJson.userid,
                    filterToPropId: '',
                    title: 'User Profile',
                    onEditNode: null // function (nodeid, nodekey) { }
                });
            });
            menuAction.add('multiedit', function (menuItemName, menuItemJson, menuItem) { Csw.tryExec(cswPrivate.onMultiEdit); });
            menuAction.add('SaveViewAs', function (menuItemName, menuItemJson, menuItem) {
                $.CswDialog('AddViewDialog', {
                    viewid: Csw.string(menuItemJson.viewid),
                    viewmode: Csw.string(menuItemJson.viewmode),
                    onAddView: cswPrivate.onSaveView
                });
            });
            menuAction.add('Quotas', function (menuItemName, menuItemJson, menuItem) {
                if (Csw.clientChanges.manuallyCheckChanges()) {
                    Csw.tryExec(cswPrivate.onQuotas);
                    return true;  //isWholePageNavigation
                }
            });
            menuAction.add('Manage Locations', function (menuItemName, menuItemJson, menuItem) {
                if (Csw.clientChanges.manuallyCheckChanges()) {
                    Csw.tryExec(cswPrivate.onQuotas);
                    return true;  //isWholePageNavigation
                }
            });
            menuAction.add('Delete Demo Data', function (menuItemName, menuItemJson, menuItem) {
                if (Csw.clientChanges.manuallyCheckChanges()) {
                    Csw.tryExec(cswPrivate.onQuotas);
                    return true;  //isWholePageNavigation
                }
            });
            menuAction.add('Modules', function (menuItemName, menuItemJson, menuItem) {
                if (Csw.clientChanges.manuallyCheckChanges()) {
                    Csw.tryExec(cswPrivate.onModules);
                    return true;  //isWholePageNavigation
                }
            });
            menuAction.add('Sessions', function (menuItemName, menuItemJson, menuItem) {
                if (Csw.clientChanges.manuallyCheckChanges()) {
                    Csw.tryExec(cswPrivate.onSessions);
                    return true;  //isWholePageNavigation
                }
            });
            menuAction.add('Subscriptions', function (menuItemName, menuItemJson, menuItem) {
                if (Csw.clientChanges.manuallyCheckChanges()) {
                    Csw.tryExec(cswPrivate.onSubscriptions);
                    return true;  //isWholePageNavigation
                }
            });
            menuAction.add('Impersonate', function (menuItemName, menuItemJson, menuItem) {
                if (Csw.clientChanges.manuallyCheckChanges()) {
                    $.CswDialog('ImpersonateDialog', { onImpersonate: cswPrivate.onImpersonate });
                }
            });
            menuAction.add('Submit_Request', function (menuItemName, menuItemJson, menuItem) {
                if (Csw.clientChanges.manuallyCheckChanges()) {
                    Csw.tryExec(cswPrivate.onSubmitRequest);
                    return true;  //isWholePageNavigation
                }
            });
            menuAction.add('Login Data', function (menuItemName, menuItemJson, menuItem) {
                if (Csw.clientChanges.manuallyCheckChanges()) {
                    Csw.tryExec(cswPrivate.onLoginData);
                    return true;  //isWholePageNavigation
                }
            });
            menuAction.add('NbtManager', function (menuItemName, menuItemJson, menuItem) {
                if (Csw.clientChanges.manuallyCheckChanges()) {
                    Csw.tryExec(cswPrivate.onReturnToNbtManager);
                }
            });
            menuAction.add('default', function (menuItemName, menuItemJson, menuItem) {
                Csw.main.handleAction({ actionname: menuItemJson.action });
            });

            cswPrivate.handleMenuItemClick = function (menuItemName, menuItemJson, menuItem) {
                if (false === Csw.isNullOrEmpty(menuItemJson)) {

                    if (false === Csw.isNullOrEmpty(menuItemJson.href)) {
                        Csw.window.location(menuItemJson.href);

                    } else if (false === Csw.isNullOrEmpty(menuItemJson.popup)) {
                        window.open(menuItemJson.popup);

                    } else if (false === Csw.isNullOrEmpty(menuItemJson.action)) {

                        menuItemJson.action = menuItemJson.action || 'default';
                        var isWholePageNavigation = menuAction[menuItemJson.action](menuItemName, menuItemJson, menuItem);
                        
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

                var makeMenu = function (menuItems) {
                    cswParent.empty();
                    if (cswPublic.menu) {
                        cswPublic.menu.destroy();
                    }
                    var items = [];
                    Csw.iterate(menuItems, function (menuItem, menuItemName) {
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
                        cswPublic.menu = window.Ext.create('Ext.toolbar.Toolbar', {
                            id: cswPrivate.ID + 'toolbar',
                            renderTo: cswParent.getId(),
                            width: cswPrivate.width,
                            items: items,
                            cls: 'menutoolbar'
                        }); // toolbar
                    }

                    Csw.tryExec(cswPrivate.onSuccess);
                };

                if (cswPublic.abort && cswPublic.ajax) {
                    cswPublic.abort();
                }
                cswPublic.ajax = Csw.ajax.deprecatedWsNbt({
                    urlMethod: cswPrivate.ajax.urlMethod,
                    data: cswPrivate.ajax.data,
                    useCache: cswPrivate.useCache,
                    success: makeMenu
                }); // ajax

                cswPublic.abort = cswPublic.ajax.abort;

            }()); // constructor

            return cswPublic;
        });


    Csw.goHome = Csw.goHome ||
        Csw.register('goHome', function () {
            'use strict';
            var toDo = [];
            toDo.push(Csw.clientState.clearCurrent());
            toDo.push(Csw.main.refreshWelcomeLandingPage());
            return Q.all(toDo);
        });

}());
