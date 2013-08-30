/// <reference path="~/vendor/extjs-4.1.0/ext-all-debug.js" />
/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    'use strict';

    var menuAction = Csw.object();
    menuAction.add('About', function () { $.CswDialog('AboutDialog'); });
    menuAction.add('AddNode', function (privateScope, menuItemName, menuItemJson, menuItem) {
        Csw.dialogs.addnode({
            title: 'Add New ' + menuItemName,
            nodetypeid: Csw.string(menuItemJson.nodetypeid),
            relatednodeid: Csw.string(menuItemJson.relatednodeid), //for Grid Props
            onAddNode: privateScope.onAlterNode
        });
    });
    menuAction.add('AddFeedback', function (privateScope, menuItemName, menuItemJson, menuItem) {
        $.CswDialog('AddFeedbackDialog', {
            text: menuItemName,
            nodetypeid: Csw.string(menuItemJson.nodetypeid),
            onAddNode: privateScope.onAlterNode
        });
    });
    menuAction.add('Clear Cache', function (privateScope, menuItemName, menuItemJson, menuItem) {
        if (Csw.clientChanges.manuallyCheckChanges()) {
            window.location.reload(true);
        }
    });
    menuAction.add('DeleteNode', function (privateScope, menuItemName, menuItemJson, menuItem) {
        Csw.clientChanges.unsetChanged();
        $.CswDialog('DeleteNodeDialog', {
            nodes: privateScope.getSelectedNodes(menuItemJson),
            onDeleteNode: privateScope.onAlterNode,
            nodeTreeCheck: privateScope.nodeTreeCheck,
            Multi: privateScope.Multi
        });
    });
    menuAction.add('editview', function (privateScope, menuItemName, menuItemJson, menuItem) {
        if (Csw.clientChanges.manuallyCheckChanges()) {
            Csw.tryExec(privateScope.onEditView, Csw.string(menuItemJson.viewid));
            return true;  //isWholePageNavigation
        }
    });
    menuAction.add('CopyNode', function (privateScope, menuItemName, menuItemJson, menuItem) {
        if (Csw.clientChanges.manuallyCheckChanges()) {
            Csw.dialogs.copynode({
                copyType: Csw.string(menuItemJson.copytype),
                nodename: Csw.string(menuItemJson.nodename),
                nodeid: Csw.string(menuItemJson.nodeid),
                nodetypeid: Csw.string(menuItemJson.nodetypeid),
                onCopyNode: privateScope.onAlterNode
            });
        }
    });
    menuAction.add('PrintView', function (privateScope, menuItemName, menuItemJson, menuItem) { Csw.tryExec(privateScope.onPrintView); });
    menuAction.add('PrintLabel', function (privateScope, menuItemName, menuItemJson, menuItem) {
        $.CswDialog('PrintLabelDialog', {
            nodes: privateScope.getSelectedNodes(menuItemJson),
            nodetypeid: Csw.string(menuItemJson.nodetypeid)
        });
    });
    menuAction.add('Logout', function (privateScope, menuItemName, menuItemJson, menuItem) {
        if (Csw.clientChanges.manuallyCheckChanges()) {
            Csw.tryExec(privateScope.onLogout);
            return true;  //isWholePageNavigation
        }
    });
    menuAction.add('Home', function (privateScope, menuItemName, menuItemJson, menuItem) {
        var enable = function () {
            menuItem.enable();
        };
        if (Csw.clientChanges.manuallyCheckChanges()) {
              menuItem.disable();
              isWholePageNavigation = true;
              Csw.goHome(enable).then(enable);
        } 
    });
    menuAction.add('Profile', function (privateScope, menuItemName, menuItemJson, menuItem) {
        $.CswDialog('EditNodeDialog', {
            currentNodeId: menuItemJson.userid,
            filterToPropId: '',
            title: 'User Profile',
            onEditNode: null // function (nodeid, nodekey) { }
        });
    });
    menuAction.add('multiedit', function (privateScope, menuItemName, menuItemJson, menuItem) { Csw.tryExec(privateScope.onMultiEdit); });
    menuAction.add('SaveViewAs', function (privateScope, menuItemName, menuItemJson, menuItem) {
        $.CswDialog('AddViewDialog', {
            viewid: Csw.string(menuItemJson.viewid),
            viewmode: Csw.string(menuItemJson.viewmode),
            onAddView: privateScope.onSaveView
        });
    });
    menuAction.add('Quotas', function (privateScope, menuItemName, menuItemJson, menuItem) {
        if (Csw.clientChanges.manuallyCheckChanges()) {
            Csw.tryExec(privateScope.onQuotas);
            return true;  //isWholePageNavigation
        }
    });
    menuAction.add('Manage Locations', function (privateScope, menuItemName, menuItemJson, menuItem) {
        if (Csw.clientChanges.manuallyCheckChanges()) {
            Csw.tryExec(privateScope.onQuotas);
            return true;  //isWholePageNavigation
        }
    });
    menuAction.add('Delete Demo Data', function (privateScope, menuItemName, menuItemJson, menuItem) {
        if (Csw.clientChanges.manuallyCheckChanges()) {
            Csw.tryExec(privateScope.onQuotas);
            return true;  //isWholePageNavigation
        }
    });
    menuAction.add('Modules', function (privateScope, menuItemName, menuItemJson, menuItem) {
        if (Csw.clientChanges.manuallyCheckChanges()) {
            Csw.tryExec(privateScope.onModules);
            return true;  //isWholePageNavigation
        }
    });
    menuAction.add('Sessions', function (privateScope, menuItemName, menuItemJson, menuItem) {
        if (Csw.clientChanges.manuallyCheckChanges()) {
            Csw.tryExec(privateScope.onSessions);
            return true;  //isWholePageNavigation
        }
    });
    menuAction.add('Subscriptions', function (privateScope, menuItemName, menuItemJson, menuItem) {
        if (Csw.clientChanges.manuallyCheckChanges()) {
            Csw.tryExec(privateScope.onSubscriptions);
            return true;  //isWholePageNavigation
        }
    });
    menuAction.add('Impersonate', function (privateScope, menuItemName, menuItemJson, menuItem) {
        if (Csw.clientChanges.manuallyCheckChanges()) {
            $.CswDialog('ImpersonateDialog', { onImpersonate: privateScope.onImpersonate });
        }
    });
    menuAction.add('Submit_Request', function (privateScope, menuItemName, menuItemJson, menuItem) {
        if (Csw.clientChanges.manuallyCheckChanges()) {
            Csw.tryExec(privateScope.onSubmitRequest);
            return true;  //isWholePageNavigation
        }
    });
    menuAction.add('Login Data', function (privateScope, menuItemName, menuItemJson, menuItem) {
        if (Csw.clientChanges.manuallyCheckChanges()) {
            Csw.tryExec(privateScope.onLoginData);
            return true;  //isWholePageNavigation
        }
    });
    menuAction.add('NbtManager', function (privateScope, menuItemName, menuItemJson, menuItem) {
        if (Csw.clientChanges.manuallyCheckChanges()) {
            Csw.tryExec(privateScope.onReturnToNbtManager);
        }
    });
    menuAction.add('default', function (privateScope, menuItemName, menuItemJson, menuItem) {
        Csw.main.handleAction({ actionname: menuItemJson.action });
    });

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
            
            cswPrivate.handleMenuItemClick = function (menuItemName, menuItemJson, menuItem) {
                if (false === Csw.isNullOrEmpty(menuItemJson)) {

                    if (false === Csw.isNullOrEmpty(menuItemJson.href)) {
                        Csw.window.location(menuItemJson.href);

                    } else if (false === Csw.isNullOrEmpty(menuItemJson.popup)) {
                        window.open(menuItemJson.popup);

                    } else if (false === Csw.isNullOrEmpty(menuItemJson.action)) {

                        var action = menuAction[menuItemJson.action] ? menuItemJson.action : 'default';
                        var isWholePageNavigation = menuAction[action](cswPrivate, menuItemName, menuItemJson, menuItem);

                        
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
        Csw.register('goHome', function (onError) {
            'use strict';
            var toDo = [];
            toDo.push(Csw.clientState.clearCurrent());
            toDo.push(Csw.main.refreshWelcomeLandingPage());
            return Q.all(toDo).fail(onError);
        });

}());
