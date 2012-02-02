/// <reference path="/js/../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../controls/CswGrid.js" />
/// <reference path="../pagecmp/CswDialog.js" />

(function ($) { 
    "use strict";
    var pluginName = 'CswNodeTable';

    var methods = {

        'init': function (options) {

            var o = {
                TableUrl: '/NbtWebApp/wsNBT.asmx/getTable',
                viewid: '',
                ID: '',
                nodeid: '',
                cswnbtnodekey: '',
                EditMode: Csw.enums.editMode.Edit,
                //onAddNode: function (nodeid,cswnbtnodekey){},
                onEditNode: null, //function (nodeid,cswnbtnodekey){},
                onDeleteNode: null, //function (nodeid,cswnbtnodekey){}
                onSuccess: null, // function () {}
                columns: 3,      // number of columns to use
                maxlength: 35,   // max length of node names and property values
                rowpadding: 25,  // padding between table rows, in pixels
                maxheight: 600   // maximum display height of table, in pixels
            };
            if (options) $.extend(o, options);

            var $parent = $(this);

            var $scrollingdiv = $parent.CswDiv({ ID: makeId({ id: o.ID, suffix: '_scrolldiv' }) })
                                    .css({
                                        height: o.height + 'px',
                                        overflow: 'auto'
                                    });

            var $table = $scrollingdiv.CswLayoutTable('init', {
                ID: o.ID + '_tbl',
                cellset: { rows: 2, columns: 1 },
                cellalign: 'center',
                width: '100%',
                cellspacing: '5px'
            });

            Csw.ajax.post({
                url: o.TableUrl,
                data: {
                    ViewId: o.viewid,
                    NodeId: o.nodeid,
                    NodeKey: o.cswnbtnodekey
                },
                success: function (data) {
                    var r = 1;
                    var c = 1;

                    Csw.crawlObject(data, function (nodeObj) {
                        var nodeid = nodeObj.nodeid;

                        if (nodeObj.nodename == "Results Truncated") {
                            c = 1;
                            r += 1;
                        }
                        var cellset = $table.CswLayoutTable('cellset', r, c);
                        var width = (1 / o.columns * 100) + '%';
                        var $thumbnailcell = cellset[1][1]
                                                .css({
                                                    paddingTop: o.rowpadding + 'px',
                                                    width: width,
                                                    verticalAlign: 'bottom'
                                                });
                        var $textcell = cellset[2][1]
                                                .css({
                                                    width: width
                                                });
                        // Name
                        var name;
                        if (nodeObj.nodename.length > o.maxlength) {
                            name = '<b>' + nodeObj.nodename.substr(0, o.maxlength) + '...</b>';
                        } else {
                            name = '<b>' + nodeObj.nodename + '</b>';
                        }

                        if (false === isNullOrEmpty(nodeObj.thumbnailurl)) {
                            $thumbnailcell.append('<img src="' + nodeObj.thumbnailurl + '" style="max-width: 90%;">');
                        }
                        $thumbnailcell.append('<br/>');

                        if (locked) {
                            name += '<img src="Images/quota/lock.gif" title="Quota exceeded" />';
                        }
                        $textcell.append(name + '<br/>');

                        // Props
                        Csw.crawlObject(nodeObj.props, function (propObj) {
                            $textcell.append('' + propObj.propname + ': ');
                            if (propObj.gestalt.length > o.maxlength) {
                                $textcell.append(propObj.gestalt.substr(0, o.maxlength) + '...');
                            } else {
                                $textcell.append(propObj.gestalt);
                            }
                            $textcell.append('<br/>');
                        });

                        // Buttons
                        var $btntable = $textcell.CswTable({ ID: makeId({ id: o.ID, suffix: nodeid + '_btntbl' }) });
                        if (nodeObj.allowview || nodeObj.allowedit) {
                            var btntext = "View";
                            if (nodeObj.allowedit) {
                                btntext = "Edit";
                            }
                            $btntable.CswTable('cell', 1, 1).CswButton({
                                ID: makeId({ id: o.ID, suffix: nodeid + '_editbtn' }),
                                enabledText: btntext,
                                disableOnClick: false,
                                onclick: function () {
                                    $.CswDialog('EditNodeDialog', {
                                        nodeids: [nodeid],
                                        nodekeys: [nodeObj.nodekey],
                                        nodenames: [nodeObj.nodename],
                                        ReadOnly: (false === nodeObj.allowedit),
                                        onEditNode: o.onEditNode
                                    }); // CswDialog
                                } // onclick
                            }); // CswButton
                        } // if (nodeObj.allowview || nodeObj.allowedit) 

                        if (nodeObj.allowdelete) {
                            $btntable.CswTable('cell', 1, 2).CswButton({
                                ID: makeId({ id: o.ID, suffix: nodeid + '_btn' }),
                                enabledText: 'Delete',
                                disableOnClick: false,
                                onclick: function () {
                                    $.CswDialog('DeleteNodeDialog', {
                                        nodenames: [nodeObj.nodename],
                                        nodeids: [nodeid],
                                        cswnbtnodekeys: [nodeObj.nodekey],
                                        onDeleteNode: o.onDeleteNode
                                    }); // CswDialog
                                } // onclick
                            }); // CswButton
                        } // if (nodeObj.allowdelete)

                        c += 1;
                        if (c > o.columns) { c = 1; r += 1; }
                    });


                    if (Csw.isFunction(o.onSuccess)) {
                        o.onSuccess();
                    }
                } // success{} 
            }); // ajax
        } // 'init'
    }; // methods

    $.fn.CswNodeTable = function (method) {
        // Method calling logic
        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName); return false;
        }
    };

})(jQuery);

