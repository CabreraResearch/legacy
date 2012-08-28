
/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    Csw.nbt.nodeTable = Csw.nbt.nodeTable ||
        Csw.nbt.register('nodeTable', function (cswParent, params) {
            'use strict';
            var cswPrivate = {
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
                pagenodelimit: 20,
                rowpadding: 25,  // padding between table rows, in pixels
                //maxheight: 600,   // maximum display height of table, in pixels
                tabledata: null,
                allowEdit: true,
                allowDelete: true,
                compactResults: false,
                extraAction: null,
                extraActionIcon: Csw.enums.iconType.none,
                onExtraAction: null  // function(nodeObj) {}
            };
            if (params) Csw.extend(cswPrivate, params);
            
            var cswPublic = {};


            cswPrivate.getThumbnailCell = function (cellSet) {
                return cellSet[1][1];
            };
            
            
            cswPrivate.getTextCell = function (cellSet) {
                var ret;
                if (cswPrivate.singleColumn) {
                    ret = cellSet[1][2];
                } else {
                    ret = cellSet[2][1];
                }
                return ret;
            };


            cswPrivate.getButtonCell = function (cellSet) {
                var ret;
                if (cswPrivate.singleColumn) {
                    ret = cellSet[1][3];
                } else {
                    ret = cellSet[3][1];
                }
                return ret;
            };


            cswPrivate.makeNodeCell = function (nodeObj) {
                if ((cswPrivate.pagenodecount >= cswPrivate.pagenodelimit * (cswPrivate.currentpage - 1)) &&
                       (cswPrivate.pagenodecount < cswPrivate.pagenodelimit * cswPrivate.currentpage)) {
                    var nodeid = nodeObj.nodeid;

                    if (nodeObj.nodename === "Results Truncated") {
                        cswPrivate.c = 1;
                        cswPrivate.r += 1;
                    }
                    var cellSet = cswPrivate.layoutTable.cellSet(cswPrivate.r, cswPrivate.c);
                    var thumbwidth = (1 / cswPrivate.columns * 100) + '%';
                    var textwidth = (1 / cswPrivate.columns * 100) + '%';
                    var imgwidth = '75%';
                    var imgheight = '';
                    var thumbverticalAlign = 'middle';
                    var thumbhorizontalAlign = 'center';
                    var thumbBackgroundColor = '#ffffff';
                    var bborder = '1px solid #cccccc';
                    var cellpad = cswPrivate.rowpadding + 'px';
                    if (cswPrivate.singleColumn) {
                        thumbwidth = '25%';
                        textwidth = '75%';
                        imgwidth = '90%';
                        cellpad = '10px';
                        thumbverticalAlign = 'top';
                        thumbhorizontalAlign = '';
                        thumbBackgroundColor = '';
                    }
                    if (Csw.bool(cswPrivate.compactResults)) {
                        thumbwidth = '10%';
                        cellpad = '0px';
                    }

                    var thumbnailCell = cswPrivate.getThumbnailCell(cellSet)
                                                .css({
                                                    width: thumbwidth,
                                                    verticalAlign: thumbverticalAlign,
                                                    backgroundColor: thumbBackgroundColor,
                                                    textAlign: thumbhorizontalAlign,
                                                    paddingTop: cellpad,
                                                    maxWidth: '100px'
                                                });
                    var textCell = cswPrivate.getTextCell(cellSet)
                                                .css({
                                                    width: textwidth,
                                                    paddingTop: cellpad
                                                });
                    if (cswPrivate.singleColumn) {
                        thumbnailCell.css({
                            borderBottom: bborder,
                            paddingBottom: cellpad
                        });
                        textCell.css({
                            borderBottom: bborder,
                            paddingBottom: cellpad
                        });
                    }
                    var btncell = cswPrivate.getButtonCell(cellSet)
                                                .css({
                                                    borderBottom: bborder,
                                                    paddingBottom: cellpad
                                                });

                    // Banding
                    if (cswPrivate.singleColumn) {
                        if(cswPrivate.r % 2 === 1)
                        {
                            thumbnailCell.addClass('NodeTableOddRow');
                            textCell.addClass('NodeTableOddRow');
                            btncell.addClass('NodeTableOddRow');
                        } else {
                            thumbnailCell.addClass('NodeTableEvenRow');
                            textCell.addClass('NodeTableEvenRow');
                            btncell.addClass('NodeTableEvenRow');
                        }
                    }

                    textCell.append('<b>' + nodeObj.nodename + '</b>');
                    if (Csw.bool(nodeObj.locked)) {
                        textCell.img({
                            src: 'Images/quota/lock.gif',
                            title: 'Quota exceeded'
                        });
                    }
                    textCell.br();

                    //var thumbtable = thumbnailCell.table({ width: '100%', cellpadding: 0, cellspacing: 0 });
                    var texttable = textCell.table({ width: '100%', cellpadding: 0, cellspacing: 0 });
                    cswPrivate.texttables.push(texttable);

                    if (Csw.bool(cswPrivate.compactResults)) {
                        texttable.css({ paddingBottom: '10px' });
                        texttable.hide();
                        imgwidth = '';
                        imgheight = '18px';
                    }

                    if (false === Csw.isNullOrEmpty(nodeObj.thumbnailurl)) {
                        thumbnailCell.img({
                            src: nodeObj.thumbnailurl
                        }).css({ width: imgwidth, height: imgheight });
                    }
//                    var moreinfoimg = thumbtable.cell(1, 2).css({ width: '25px' })
//                            .img({
//                                src: 'Images/newicons/18/info.png',
//                                title: 'More Info'
//                            });
//                    moreinfoimg.propNonDom({ valign: 'top' });
//                    moreinfoimg.$.hover(function (event) { Csw.nodeHoverIn(event, nodeid, '', 0); }, Csw.nodeHoverOut);
                    thumbnailCell.$.hover(function (event) { Csw.nodeHoverIn(event, nodeid, ''); },
                                          function (event) { Csw.nodeHoverOut(event, nodeid, ''); });
                    textCell.$.hover(function (event) { Csw.nodeHoverIn(event, nodeid, ''); },
                                     function (event) { Csw.nodeHoverOut(event, nodeid, ''); });

                    //thumbnailCell.br();

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
                        ID: Csw.makeId(cswPrivate.ID, nodeid + '_btntbl'),
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
                                ID: Csw.makeId(cswPrivate.ID, propObj.id, 'tbl'),
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
                    if (Csw.bool(cswPrivate.compactResults)) {
                        btnTable.cell(1, btncol).buttonExt({
                            ID: Csw.makeId(cswPrivate.ID, nodeid, 'morebtn'),
                            enabledText: 'More Info',
                            icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.info),
                            disableOnClick: false,
                            onClick: function () {
                                texttable.toggle();
                            } // onClick
                        }); // CswButton
                        btncol += 1;
                    }
                    if (Csw.bool(cswPrivate.allowEdit) && (Csw.bool(nodeObj.allowview) || Csw.bool(nodeObj.allowedit))) {

                        btnTable.cell(1, btncol).buttonExt({
                            ID: Csw.makeId(cswPrivate.ID, nodeid, 'editbtn'),
                            enabledText: 'Details',
                            //tooltip: { title: btntext },
                            icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.pencil),
                            onClick: function () {
                                $.CswDialog('EditNodeDialog', {
                                    nodeids: [nodeid],
                                    nodekeys: [nodeObj.nodekey],
                                    nodenames: [nodeObj.nodename],
                                    ReadOnly: (false === nodeObj.allowedit),
                                    onEditNode: cswPrivate.onEditNode
                                }); // CswDialog
                            } // onClick
                        }); // CswButton
                        btncol += 1;
                    } // if (nodeObj.allowview || nodeObj.allowedit) 

                    if (Csw.bool(cswPrivate.allowDelete) && Csw.bool(nodeObj.allowdelete)) {
                        btnTable.cell(1, btncol).buttonExt({
                            ID: Csw.makeId(cswPrivate.ID, nodeid, 'delbtn'),
                            enabledText: 'Delete',
                            disabledOnClick: false,
                            //tooltip: { title: 'Delete' },
                            icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.trash),
                            onClick: Csw.method(function () {
                                $.CswDialog('DeleteNodeDialog', {
                                    nodenames: [nodeObj.nodename],
                                    nodeids: [nodeid],
                                    cswnbtnodekeys: [nodeObj.nodekey],
                                    onDeleteNode: cswPrivate.onDeleteNode
                                }); // CswDialog
                            }) // onClick
                        }); // CswButton
                        btncol += 1;
                    } // if (nodeObj.allowdelete)

                    if (false === Csw.isNullOrEmpty(cswPrivate.extraAction)) {
                        Csw.debug.assert(Csw.number(cswPrivate.extraActionIcon) > 0, 'No icon specified for extraAction.');
                        Csw.debug.assert(Csw.isFunction(cswPrivate.onExtraAction), 'No method specified for extraAction.');

                        btnTable.cell(1, btncol).buttonExt({
                            ID: Csw.makeId(cswPrivate.ID, nodeid, 'extrabtn'),
                            enabledText: cswPrivate.extraAction,
                            //tooltip: { title: cswPrivate.extraAction },
                            icon: cswPrivate.extraActionIcon,
                            disableOnClick: false,
                            onClick: function () {
                                Csw.tryExec(cswPrivate.onExtraAction, nodeObj);
                            } // onClick
                        }); // CswButton
                        btncol += 1;
                    } // if (nodeObj.allowdelete)

                    if (Csw.bool(nodeObj.disabled)) {
                        textCell.addClass('disabled');
                        btnTable.addClass('disabled');
                    }

                    cswPrivate.c += 1;
                    if (cswPrivate.c > cswPrivate.columns) {
                        cswPrivate.c = 1; cswPrivate.r += 1;
                    }
                } // if((pagenodecount < pagenodelimit * (currentpage - 1))
                cswPrivate.pagenodecount += 1;
            }; // makeNodeCell()


            cswPrivate.makeTable = function () {
                var i;
                cswPrivate.tableDiv.$.empty();
                cswPrivate.pagerDiv.$.empty();
                cswPrivate.r = 1;
                cswPrivate.c = 1;
                cswPrivate.pagenodecount = 0;
                cswPrivate.texttables = [];

                var cellalign = 'left';
                var cellset = { rows: 3, columns: 1 };
                var cellspacing = '5px';
                if (cswPrivate.singleColumn) {
                    cellalign = 'left';
                    cellset = { rows: 1, columns: 3 };
                    cellspacing = '0px';
                }

                cswPrivate.layoutTable = cswPrivate.tableDiv.layoutTable({
                    ID: cswPrivate.ID + '_tbl',
                    cellSet: cellset,
                    cellalign: cellalign,
                    width: '',
                    cellspacing: cellspacing
                });

                Csw.crawlObject(cswPrivate.tabledata.nodes, cswPrivate.makeNodeCell);

                // Pager control
                if (cswPrivate.pagenodelimit < cswPrivate.results) {
                    
                    if (cswPrivate.currentpage > 1) {
                        cswPrivate.pagerDiv.a({
                            ID: 'tableprev',
                            text: 'Previous Page',
                            onClick: function () {
                                cswPrivate.currentpage -= 1;
                                cswPrivate.makeTable();
                            }
                        });
                    }

                    var pageoptions = [];
                    for (i = 0; i < cswPrivate.totalpages; i++) {
                        pageoptions[i] = { value: Csw.string(i + 1), display: Csw.string(i + 1) };
                    }
                    var pagesel = cswPrivate.pagerDiv.select({
                        ID: 'pageselect',
                        values: pageoptions,
                        selected: Csw.string(cswPrivate.currentpage),
                        onChange: function () {
                            cswPrivate.currentpage = Csw.number(pagesel.val());
                            cswPrivate.makeTable();
                        }
                    });

                    if (cswPrivate.currentpage < cswPrivate.totalpages) {
                        cswPrivate.pagerDiv.a({
                            ID: 'tablenext',
                            text: 'Next Page',
                            onClick: function () {
                                cswPrivate.currentpage += 1;
                                cswPrivate.makeTable();
                            }
                        });
                    }
                } // if (pagenodelimit < results)
            }; // makeTable()


            cswPrivate.HandleTableData = function () {
                cswPrivate.results = Csw.number(cswPrivate.tabledata.results, -1);
                cswPrivate.totalpages = Math.ceil(cswPrivate.results / cswPrivate.pagenodelimit);

                if (cswPrivate.results === 0) {
                    Csw.tryExec(cswPrivate.onNoResults, {
                        viewid: cswPrivate.viewid,
                        viewmode: Csw.enums.viewMode.table.name
                    });
                } else {
                    cswPrivate.tableDiv = cswParent.div({
                        ID: Csw.makeId({ id: cswPrivate.ID, suffix: '_scrolldiv' })
                    }).css({ width: '100%' });
                    cswPrivate.pagerDiv = cswParent.div({
                        ID: Csw.makeId({ id: cswPrivate.ID, suffix: '_pagerdiv' })
                    });
                    cswPrivate.makeTable();
                }
                Csw.tryExec(cswPrivate.onSuccess);
            }; // HandleTableData()


            cswPublic.expandAll = function() {
                Csw.each(cswPrivate.texttables, function(texttable) {
                    texttable.toggle();
                });
            }; // expandAll()

            // constructor
            (function () {
                cswPrivate.singleColumn = (cswPrivate.columns === 1);
                cswPrivate.r = 1;
                cswPrivate.c = 1;
                cswPrivate.currentpage = 1;
                cswPrivate.pagenodecount = 0;

                if (false === Csw.isNullOrEmpty(cswPrivate.tabledata)) {
                    cswPrivate.HandleTableData();
                } else {
                    Csw.ajax.post({
                        url: cswPrivate.TableViewUrl,
                        data: {
                            ViewId: cswPrivate.viewid,
                            NodeId: cswPrivate.nodeid,
                            NodeKey: cswPrivate.cswnbtnodekey
                        },
                        success: function (result) {
                            cswPrivate.tabledata = result;
                            cswPrivate.HandleTableData();
                        }
                    });
                }
            })(); // constructor

            return cswPublic;
        }); // register
})(); // (function
