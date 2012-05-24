/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {
    'use strict';

    Csw.goHome = Csw.goHome ||
        Csw.register('goHome', function () {
            'use strict';
            Csw.clientState.clearCurrent();
            Csw.window.location(Csw.getGlobalProp('homeUrl'));
        });

    Csw.handleMenuItem = Csw.handleMenuItem ||
        Csw.register('handleMenuItem', function (options) {
            'use strict';
            var o = {
                $ul: '',
                itemKey: '',
                itemJson: '',
                onLogout: null, // function () { },
                onAlterNode: null, // function (nodeid, nodekey) { },
//                onSearch: {
//                    onViewSearch: null, // function () { },
//                    onGenericSearch: null // function () { }
//                },
                onMultiEdit: null, //function () { },
                onEditView: null, //function (viewid) { },
                onSaveView: null, //function (newviewid) { },
                onPrintView: null,  // function () { },
                onQuotas: null, // function () { },
                onModules: null, // function () { },
                onSessions: null, // function () { },
                onImpersonate: null,
                Multi: false,
                nodeTreeCheck: null
            };
            if (options) {
                $.extend(o, options);
            }
            var $li;
            var json = o.itemJson;
            var href = Csw.string(json.href);
            var text = Csw.string(o.itemKey);
            var popup = Csw.string(json.popup);
            var action = Csw.string(json.action);

            if (false === Csw.isNullOrEmpty(href)) {
                $li = $('<li><a href="' + href + '">' + text + '</a></li>')
                    .appendTo(o.$ul);
            } else if (false === Csw.isNullOrEmpty(popup)) {
                $li = $('<li class="headermenu_dialog"><a href="' + popup + '" target="_blank">' + text + '</a></li>')
                    .appendTo(o.$ul);
            } else if (false === Csw.isNullOrEmpty(action)) {
                $li = $('<li><a href="#">' + text + '</a></li>')
                    .appendTo(o.$ul);
                var $a = $li.children('a');
                var nodeid = Csw.string(json.nodeid);
                var nodename = Csw.string(json.nodename);
                var nodetypeid = Csw.string(json.nodetypeid);
                var viewid = Csw.string(json.viewid);

                switch (action) {
                    case 'About':
                    $a.click(function() {
                            $.CswDialog('AboutDialog');
                            return false;
                        });
                        break;
                    case 'AddNode':
                    $a.click(function() {
                            $.CswDialog('AddNodeDialog', {
                                text: text,
                                nodetypeid: Csw.string(json.nodetypeid),
                                relatednodeid: Csw.string(json.relatednodeid), //for Grid Props
                            relatednodename: Csw.string(json.relatednodename), //for Grid Props
                                relatednodetypeid: Csw.string(json.relatednodetypeid), //for NodeTypeSelect
                            relatedobjectclassid: Csw.string(json.relatedobjectclassid),
                                onAddNode: o.onAlterNode
                            });
                            return false;
                        });
                        break;
                    case 'Clear Cache':
                        $a.click(function () {
                            return window.location.reload(true);
                        });
                        break;
                    case 'DeleteNode':
                    $a.click(function() {
                            $.CswDialog('DeleteNodeDialog', {
                                nodenames: [nodename],
                                nodeids: [nodeid],
                                onDeleteNode: o.onAlterNode,
                                nodeTreeCheck: o.nodeTreeCheck,
                                Multi: o.Multi
                            });
                            return false;
                        });
                        break;
                case 'DeleteDemoNodes':
                    $a.click(function() {
                        $.CswDialog('ConfirmDialog', 'You are about to delete all demo data nodes from the database. Are you sure?', 'Delete All Demo Data', function() {
                            Csw.ajax.post({
                                url: Csw.enums.ajaxUrlPrefix + 'DeleteDemoDataNodes',
                                success: Csw.goHome
                            });
                        }, 'Cancel');
                    });
                    break;
                    case 'editview':
                    $a.click(function() {
                            o.onEditView(viewid);
                            return false;
                        });
                        break;
                    case 'CopyNode':
                    $a.click(function() {
                            $.CswDialog('CopyNodeDialog', {
                                nodename: nodename,
                                nodeid: nodeid,
                                nodetypeid: nodetypeid,
                                onCopyNode: o.onAlterNode
                            });
                            return false;
                        });
                        break;
                    case 'PrintView':
                        $a.click(o.onPrintView);
                        break;
                    case 'PrintLabel':
                    $a.click(function() {
                            $.CswDialog('PrintLabelDialog', {
                                'nodeid': nodeid,
                                'propid': Csw.string(json.propid)
                            });
                            return false;
                        });
                        break;
                    case 'Logout':
                    $a.click(function() {
                            o.onLogout();
                            return false;
                        });
                        break;
                    case 'Home':
                    $a.click(function() {
                            Csw.goHome();
                            return false;
                        });
                        break;
                    case 'Profile':
                    $a.click(function() {
                            $.CswDialog('EditNodeDialog', {
                                nodeids: [json.userid],
                                filterToPropId: '',
                                title: 'User Profile',
                                onEditNode: null // function (nodeid, nodekey) { }
                            });
                            return false;
                        });
                        break;
                    case 'ViewSearch':
                    $a.click(function() {
                            Csw.tryExec(o.onSearch.onViewSearch);
                        });
                        break;
                    case 'GenericSearch':
                    $a.click(function() {
                            Csw.tryExec(o.onSearch.onGenericSearch);
                        });
                        break;
                    case 'multiedit':
                        $a.click(o.onMultiEdit);
                        break;
                    case 'SaveViewAs':
                    $a.click(function() {
                            $.CswDialog('AddViewDialog', {
                                viewid: viewid,
                                viewmode: Csw.string(json.viewmode),
                                onAddView: o.onSaveView
                            });
                            return false;
                        });
                        break;
                    case 'Quotas':
                        $a.click(o.onQuotas);
                        break;
                    case 'Modules':
                        $a.click(o.onModules);
                        break;
                    case 'Sessions':
                        $a.click(o.onSessions);
                    break;
                case 'Impersonate':
                    $a.click(function () {
                        $.CswDialog('ImpersonateDialog', { onImpersonate: o.onImpersonate });
                        return false;
                    });
                    break;
                case 'EndImpersonation':
                    $a.click(function () {
                        Csw.tryExec(o.onEndImpersonation);
                        return false;
                    });
                        break;
                }
            } else {
                $li = $('<li>' + text + '</li>')
                    .appendTo(o.$ul);
            }
            return $li;
        });

} ());
