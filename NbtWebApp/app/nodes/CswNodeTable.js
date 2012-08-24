
/// <reference path="~/app/CswApp-vsdoc.js" />

(function ($) {
    "use strict";
    var pluginName = 'CswNodeTable';

    var methods = {

        'init': function (options) {

            var o = {
                TableSearchUrl: '/NbtWebApp/wsNBT.asmx/getTableSearch',
                TableViewUrl: '/NbtWebApp/wsNBT.asmx/getTableView',
                viewid: '',
                ID: '',
                nodeid: '',
                cswnbtnodekey: '',
                EditMode: Csw.enums.editMode.Edit,
                onEditNode: null,
                onDeleteNode: null,
                onSuccess: null,
                onNoResults: null,  // function({viewid, viewmode})
                columns: 3,      // number of columns to use
                rowpadding: 25,  // padding between table rows, in pixels
                //maxheight: 600,   // maximum display height of table, in pixels
                tabledata: null,
                allowEdit: true,
                allowDelete: true,
                extraAction: null,
                extraActionIcon: Csw.enums.iconType.none,
                onExtraAction: null  // function(nodeObj) {}
            };
            if (options) Csw.extend(o, options);

            var $parent = $(this);
            var parent = Csw.literals.factory($parent);
            var tableDiv, layoutTable;

            var singleColumn = false;
            if (o.columns === 1) {
                singleColumn = true;
            }

            if (false === Csw.isNullOrEmpty(o.tabledata)) {
                _HandleTableData(o.tabledata);
            } else {
                Csw.ajax.post({
                    url: o.TableViewUrl,
                    data: {
                        ViewId: o.viewid,
                        NodeId: o.nodeid,
                        NodeKey: o.cswnbtnodekey
                    },
                    success: _HandleTableData
                });
            }

            function _getThumbnailCell(cellSet) {
                return cellSet[1][1];
            }
            function _getTextCell(cellSet) {
                var ret;
                if (singleColumn) {
                    ret = cellSet[1][2];
                } else {
                    ret = cellSet[2][1];
                }
                return ret;
            }
            function _getButtonCell(cellSet) {
                var ret;
                if (singleColumn) {
                    ret = cellSet[2][2];
                } else {
                    ret = cellSet[3][1];
                }
                return ret;
            }

            function _HandleTableData(data) {
                var results = Csw.number(data.results, -1);
                var r = 1;
                var c = 1;

                var pagenodelimit = Csw.number(20);
                if (pagenodelimit <= 0) pagenodelimit = 20;
                var currentpage = 1;
                var pagenodecount = 0;
                var totalpages = Math.ceil(results / pagenodelimit);

                function _makeNodeCell(nodeObj) {
                    if ((pagenodecount >= pagenodelimit * (currentpage - 1)) &&
                       (pagenodecount < pagenodelimit * currentpage)) {
                        var nodeid = nodeObj.nodeid;

                        if (nodeObj.nodename === "Results Truncated") {
                            c = 1;
                            r += 1;
                        }
                        var cellSet = layoutTable.cellSet(r, c);
                        var thumbwidth = (1 / o.columns * 100) + '%';
                        var textwidth = (1 / o.columns * 100) + '%';
                        var imgwidth = '75%';
                        var verticalAlign = 'top';
                        var bborder = '1px solid #cccccc';
                        var cellpad = o.rowpadding + 'px';
                        if (singleColumn) {
                            thumbwidth = '25%';
                            textwidth = '75%';
                            verticalAlign = 'top';
                            imgwidth = '90%';
                            cellpad = '10px';
                        }
                        var thumbnailCell = _getThumbnailCell(cellSet)
                                                .css({
                                                    width: thumbwidth,
                                                    verticalAlign: verticalAlign,
                                                    paddingTop: cellpad
                                                });
                        var textCell = _getTextCell(cellSet)
                                                .css({
                                                    width: textwidth,
                                                    paddingTop: cellpad
                                                });
                        if (singleColumn) {
                            cellSet[2][1].css({
                                borderBottom: bborder,
                                paddingBottom: cellpad
                            });
                        }
                        var btncell = _getButtonCell(cellSet)
                                                .css({
                                                    borderBottom: bborder,
                                                    paddingBottom: cellpad
                                                });


                        textCell.append('<b>' + nodeObj.nodename + '</b>');
                        if (Csw.bool(nodeObj.locked)) {
                            textCell.img({
                                src: 'Images/quota/lock.gif',
                                title: 'Quota exceeded'
                            });
                        }
                        textCell.br();

                        var thumbtable = thumbnailCell.table({ width: '100%', cellpadding: 0, cellspacing: 0 });
                        var texttable = textCell.table({ width: '100%', cellpadding: 0, cellspacing: 0 });

                        if (false === Csw.isNullOrEmpty(nodeObj.thumbnailurl)) {
                            thumbtable.cell(1, 1).img({
                                src: nodeObj.thumbnailurl
                            }).css({ width: imgwidth });
                        }
                        var moreinfoimg = thumbtable.cell(1, 2).css({ width: '25px' })
                            .img({
                                src: 'Images/info.png',
                                title: 'More Info'
                            });
                        moreinfoimg.propNonDom({ valign: 'top' });
                        moreinfoimg.$.hover(function (event) { Csw.nodeHoverIn(event, nodeid, '', 0); }, Csw.nodeHoverOut);

                        thumbnailCell.br();

                        // Name
                        //var maintextcell = texttable.cell(1, 1);
                        //maintextcell.append('<b>' + nodeObj.nodename + '</b>');

                        //                        if (Csw.bool(nodeObj.locked)) {
                        //                            maintextcell.img({
                        //                                src: 'Images/quota/lock.gif',
                        //                                title: 'Quota exceeded'
                        //                            });
                        //                        }
                        //                        maintextcell.br();

                        var btnTable = btncell.table({
                            ID: Csw.makeId(o.ID, nodeid + '_btntbl'),
                            cellspacing: '5px'
                        });
                        var btncol = 1;
                        var row = 1;
                        // Props
                        Csw.crawlObject(nodeObj.props, function (propObj) {
                            if (propObj.fieldtype === "Button") {

                                // Object Class Buttons
                                var propDiv = btnTable.cell(1, btncol).div();

                                $.CswFieldTypeFactory('make', {
                                    nodeid: nodeid,
                                    fieldtype: propObj.fieldtype,
                                    size: 'small',
                                    propid: propObj.propid,
                                    propDiv: propDiv,
                                    propData: propObj.propData,
                                    ID: Csw.makeId(o.ID, propObj.id, 'tbl'),
                                    EditMode: Csw.enums.editMode.Table,
                                    doSave: function (saveoptions) {
                                        // Nothing to save in this case, so just call onSuccess
                                        var s = { onSuccess: null };
                                        if (saveoptions) {
                                            Csw.extend(s, saveoptions, true);
                                        }
                                        Csw.tryExec(s.onSuccess);
                                    },
                                    onReload: null
                                });
                                btncol += 1;

                            } else {
                                texttable.cell(Csw.number(propObj.row, row), Csw.number(propObj.column, 1)).span({ text: propObj.propname + ': ' + propObj.gestalt });
                                row += 1;
                                //maintextcell.br();
                            }
                        });

                        // System Buttons
                        if (Csw.bool(o.allowEdit) && (Csw.bool(nodeObj.allowview) || Csw.bool(nodeObj.allowedit))) {
                            
                            btnTable.cell(1, btncol).buttonExt({
                                ID: Csw.makeId(o.ID, nodeid, 'editbtn'),
                                enabledText: 'Edit',
                                //tooltip: { title: btntext },
                                icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.pencil),
                                onClick: function () {
                                    $.CswDialog('EditNodeDialog', {
                                        nodeids: [nodeid],
                                        nodekeys: [nodeObj.nodekey],
                                        nodenames: [nodeObj.nodename],
                                        ReadOnly: (false === nodeObj.allowedit),
                                        onEditNode: o.onEditNode
                                    }); // CswDialog
                                } // onClick
                            }); // CswButton
                            btncol += 1;
                        } // if (nodeObj.allowview || nodeObj.allowedit) 

                        if (Csw.bool(o.allowDelete) && Csw.bool(nodeObj.allowdelete)) {
                            btnTable.cell(1, btncol).buttonExt({
                                ID: Csw.makeId(o.ID, nodeid, 'delbtn'),
                                enabledText: 'Delete',
                                disabledOnClick: false,
                                //tooltip: { title: 'Delete' },
                                icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.trash),
                                onClick: Csw.method(function () {
                                    $.CswDialog('DeleteNodeDialog', {
                                        nodenames: [nodeObj.nodename],
                                        nodeids: [nodeid],
                                        cswnbtnodekeys: [nodeObj.nodekey],
                                        onDeleteNode: o.onDeleteNode
                                    }); // CswDialog
                                }) // onClick
                            }); // CswButton
                            btncol += 1;
                        } // if (nodeObj.allowdelete)

                        if (false === Csw.isNullOrEmpty(o.extraAction)) {
                            Csw.debug.assert(Csw.number(o.extraActionIcon) > 0, 'No icon specified for extraAction.');
                            Csw.debug.assert(Csw.isFunction(o.onExtraAction), 'No method specified for extraAction.');

                            btnTable.cell(1, btncol).buttonExt({
                                ID: Csw.makeId(o.ID, nodeid, 'extrabtn'),
                                enabledText: o.extraAction,
                                //tooltip: { title: o.extraAction },
                                icon: o.extraActionIcon,
                                disableOnClick: false,
                                onClick: function () {
                                    Csw.tryExec(o.onExtraAction, nodeObj);
                                } // onClick
                            }); // CswButton
                            btncol += 1;
                        } // if (nodeObj.allowdelete)

                        if (Csw.bool(nodeObj.disabled)) {
                            textCell.addClass('disabled');
                            btnTable.addClass('disabled');
                        }

                        c += 1;
                        if (c > o.columns) { c = 1; r += 1; }
                    } // if((pagenodecount < pagenodelimit * (currentpage - 1))
                    pagenodecount += 1;
                } // _makeNodeCell()

                function _makeTable() {
                    var i;
                    tableDiv.$.empty();
                    r = 1;
                    c = 1;
                    pagenodecount = 0;

                    var cellalign = 'left';
                    var cellset = { rows: 3, columns: 1 };
                    var cellspacing = '5px';
                    if (singleColumn) {
                        cellalign = 'left';
                        cellset = { rows: 2, columns: 2 };
                        cellspacing = '0px';
                    }

                    layoutTable = tableDiv.layoutTable({
                        ID: o.ID + '_tbl',
                        cellSet: cellset,
                        cellalign: cellalign,
                        width: '100%',
                        cellspacing: cellspacing
                    });

                    Csw.crawlObject(data.nodes, _makeNodeCell);

                    if (pagenodelimit < results) {
                        if (currentpage > 1) {
                            tableDiv.a({
                                ID: 'tableprev',
                                text: 'Previous Page',
                                onClick: function () {
                                    currentpage -= 1;
                                    _makeTable();
                                }
                            });
                        }

                        var pageoptions = [];
                        for (i = 0; i < totalpages; i++) {
                            pageoptions[i] = { value: Csw.string(i + 1), display: Csw.string(i + 1) };
                        }
                        var pagesel = tableDiv.select({
                            ID: 'pageselect',
                            values: pageoptions,
                            selected: Csw.string(currentpage),
                            onChange: function () {
                                currentpage = Csw.number(pagesel.val());
                                _makeTable();
                            }
                        });

                        if (currentpage < totalpages) {
                            tableDiv.a({
                                ID: 'tablenext',
                                text: 'Next Page',
                                onClick: function () {
                                    currentpage += 1;
                                    _makeTable();
                                }
                            });
                        }
                    }
                }

                if (results === 0) {
                    Csw.tryExec(o.onNoResults, { viewid: o.viewid, viewmode: Csw.enums.viewMode.table.name });
                } else {
                    tableDiv = parent.div({
                        ID: Csw.makeId({ id: o.ID, suffix: '_scrolldiv' })
                        //height: o.maxheight + 'px',
                        //styles: { overflow: 'auto' }
                    });

                    _makeTable();
                }

                Csw.tryExec(o.onSuccess);

            } // _HandleTableData() 
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

