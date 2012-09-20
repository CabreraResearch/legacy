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
                                    text: menuItemName,
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
                                    nodenames: [nodename],
                                    nodeids: [nodeid],
                                    onDeleteNode: cswPrivate.onAlterNode,
                                    nodeTreeCheck: cswPrivate.nodeTreeCheck,
                                    Multi: cswPrivate.Multi
                                });
                                break;
                            case 'DeleteDemoNodes':
                                $.CswDialog('ConfirmDialog', 'You are about to delete all demo data nodes from the database. Are you sure?', 'Delete All Demo Data', function () {
                                    Csw.ajax.post({
                                        url: Csw.enums.ajaxUrlPrefix + 'DeleteDemoDataNodes',
                                        success: Csw.goHome
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
                                    nodeid: nodeid,
                                    propids: [ Csw.string(menuItemJson.propid) ]
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
                                    nodeids: [menuItemJson.userid],
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
                if (options) Csw.extend(cswPrivate, options);

                Csw.ajax.post({
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

                        if (false === Csw.isNullOrEmpty($('#' + cswParent.getId()), true)) {
                            window.Ext.create('Ext.toolbar.Toolbar', {
                                renderTo: cswParent.getId(),
                                width: cswPrivate.width,
                                items: items,
                                cls: 'menutoolbar'
                            }); // toolbar
                        } // success
                    }
                }); // ajax
            } ()); // constructor

            return cswPublic;
        });


    Csw.goHome = Csw.goHome ||
        Csw.register('goHome', function () {
            'use strict';
            Csw.clientState.clearCurrent();
            Csw.window.location(Csw.getGlobalProp('homeUrl'));
        });

    //    Csw.handleMenuItem = Csw.handleMenuItem ||
    //        Csw.register('handleMenuItem', function (options) {
    //            'use strict';
    //            var o = {
    //                $ul: '',
    //                itemKey: '',
    //                itemJson: '',
    //                onLogout: null, // function () { },
    //                onAlterNode: null, // function (nodeid, nodekey) { },
    //                //                onSearch: {
    //                //                    onViewSearch: null, // function () { },
    //                //                    onGenericSearch: null // function () { }
    //                //                },
    //                onMultiEdit: null, //function () { },
    //                onEditView: null, //function (viewid) { },
    //                onSaveView: null, //function (newviewid) { },
    //                onPrintView: null,  // function () { },
    //                onQuotas: null, // function () { },
    //                onModules: null, // function () { },
    //                onSessions: null, // function () { },
    //                onImpersonate: null,
    //                Multi: false,
    //                nodeTreeCheck: null
    //            };
    //            if (options) {
    //                Csw.extend(o, options);
    //            }
    //            var $li;
    //            var json = o.itemJson;
    //            var href = Csw.string(json.href);
    //            var text = Csw.string(o.itemKey);
    //            var popup = Csw.string(json.popup);
    //            var action = Csw.string(json.action);

    //            if (false === Csw.isNullOrEmpty(href)) {
    //                $li = $('<li><a href="' + href + '">' + text + '</a></li>')
    //                    .appendTo(o.$ul);
    //            } else if (false === Csw.isNullOrEmpty(popup)) {
    //                $li = $('<li class="headermenu_dialog"><a href="' + popup + '" target="_blank">' + text + '</a></li>')
    //                    .appendTo(o.$ul);
    //            } else if (false === Csw.isNullOrEmpty(action)) {
    //                $li = $('<li><a href="#">' + text + '</a></li>')
    //                    .appendTo(o.$ul);
    //                var $a = $li.children('a');
    //                var nodeid = Csw.string(json.nodeid);
    //                var nodename = Csw.string(json.nodename);
    //                var nodetypeid = Csw.string(json.nodetypeid);
    //                var viewid = Csw.string(json.viewid);

    //                switch (action) {
    //                    case 'About':
    //                        $a.click(function () {
    //                            $.CswDialog('AboutDialog');
    //                            return false;
    //                        });
    //                        break;
    //                    case 'AddNode':
    //                        $a.click(function () {
    //                            $.CswDialog('AddNodeDialog', {
    //                                text: text,
    //                                nodetypeid: Csw.string(json.nodetypeid),
    //                                relatednodeid: Csw.string(json.relatednodeid), //for Grid Props
    //                                relatednodename: Csw.string(json.relatednodename), //for Grid Props
    //                                relatednodetypeid: Csw.string(json.relatednodetypeid), //for NodeTypeSelect
    //                                relatedobjectclassid: Csw.string(json.relatedobjectclassid),
    //                                onAddNode: o.onAlterNode
    //                            });
    //                            return false;
    //                        });
    //                        break;
    //                    case 'AddFeedback':
    //                        $a.click(function () {
    //                            $.CswDialog('AddFeedbackDialog', {
    //                                text: text,
    //                                nodetypeid: Csw.string(json.nodetypeid),
    //                                onAddNode: o.onAlterNode
    //                            });
    //                            return false;
    //                        });
    //                        break;
    //                    case 'Clear Cache':
    //                        $a.click(function () {
    //                            return window.location.reload(true);
    //                        });
    //                        break;
    //                    case 'DeleteNode':
    //                        $a.click(function () {
    //                            $.CswDialog('DeleteNodeDialog', {
    //                                nodenames: [nodename],
    //                                nodeids: [nodeid],
    //                                onDeleteNode: o.onAlterNode,
    //                                nodeTreeCheck: o.nodeTreeCheck,
    //                                Multi: o.Multi
    //                            });
    //                            return false;
    //                        });
    //                        break;
    //                    case 'DeleteDemoNodes':
    //                        $a.click(function () {
    //                            $.CswDialog('ConfirmDialog', 'You are about to delete all demo data nodes from the database. Are you sure?', 'Delete All Demo Data', function () {
    //                                Csw.ajax.post({
    //                                    url: Csw.enums.ajaxUrlPrefix + 'DeleteDemoDataNodes',
    //                                    success: Csw.goHome
    //                                });
    //                            }, 'Cancel');
    //                        });
    //                        break;
    //                    case 'editview':
    //                        $a.click(function () {
    //                            o.onEditView(viewid);
    //                            return false;
    //                        });
    //                        break;
    //                    case 'CopyNode':
    //                        $a.click(function () {
    //                            $.CswDialog('CopyNodeDialog', {
    //                                nodename: nodename,
    //                                nodeid: nodeid,
    //                                nodetypeid: nodetypeid,
    //                                onCopyNode: o.onAlterNode
    //                            });
    //                            return false;
    //                        });
    //                        break;
    //                    case 'PrintView':
    //                        $a.click(o.onPrintView);
    //                        break;
    //                    case 'PrintLabel':
    //                        $a.click(function () {
    //                            $.CswDialog('PrintLabelDialog', {
    //                                'nodeid': nodeid,
    //                                'propid': Csw.string(json.propid)
    //                            });
    //                            return false;
    //                        });
    //                        break;
    //                    case 'Logout':
    //                        $a.click(function () {
    //                            o.onLogout();
    //                            return false;
    //                        });
    //                        break;
    //                    case 'Home':
    //                        $a.click(function () {
    //                            Csw.goHome();
    //                            return false;
    //                        });
    //                        break;
    //                    case 'Profile':
    //                        $a.click(function () {
    //                            $.CswDialog('EditNodeDialog', {
    //                                nodeids: [json.userid],
    //                                filterToPropId: '',
    //                                title: 'User Profile',
    //                                onEditNode: null // function (nodeid, nodekey) { }
    //                            });
    //                            return false;
    //                        });
    //                        break;
    //                    case 'ViewSearch':
    //                        $a.click(function () {
    //                            Csw.tryExec(o.onSearch.onViewSearch);
    //                        });
    //                        break;
    //                    case 'GenericSearch':
    //                        $a.click(function () {
    //                            Csw.tryExec(o.onSearch.onGenericSearch);
    //                        });
    //                        break;
    //                    case 'multiedit':
    //                        $a.click(o.onMultiEdit);
    //                        break;
    //                    case 'SaveViewAs':
    //                        $a.click(function () {
    //                            $.CswDialog('AddViewDialog', {
    //                                viewid: viewid,
    //                                viewmode: Csw.string(json.viewmode),
    //                                onAddView: o.onSaveView
    //                            });
    //                            return false;
    //                        });
    //                        break;
    //                    case 'Quotas':
    //                        $a.click(o.onQuotas);
    //                        break;
    //                    case 'Modules':
    //                        $a.click(o.onModules);
    //                        break;
    //                    case 'Sessions':
    //                        $a.click(o.onSessions);
    //                        break;
    //                    case 'Impersonate':
    //                        $a.click(function () {
    //                            $.CswDialog('ImpersonateDialog', { onImpersonate: o.onImpersonate });
    //                            return false;
    //                        });
    //                        break;
    //                    case 'EndImpersonation':
    //                        $a.click(function () {
    //                            Csw.tryExec(o.onEndImpersonation);
    //                            return false;
    //                        });
    //                        break;
    //                    case 'Submit_Request':
    //                        $a.click(function () {
    //                            Csw.tryExec(o.onSubmitRequest);
    //                            return false;
    //                        });
    //                        break;
    //                }
    //            } else {
    //                $li = $('<li>' + text + '</li>')
    //                    .appendTo(o.$ul);
    //            }
    //            return $li;
    //        });

} ());
