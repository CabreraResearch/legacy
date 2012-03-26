/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

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
                tabledata: null
            };
            if (options) $.extend(o, options);

            var $parent = $(this);
            var parent = Csw.controls.factory($parent);
            var tableDiv, layoutTable;

            var singleColumn = false;
            if(o.columns === 1) {
                singleColumn = true;
            }

            if (false == Csw.isNullOrEmpty(o.tabledata)) {
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

            function _getThumbnailCell(cellSet)
            {
                return cellSet[1][1];
            }
            function _getTextCell(cellSet)
            {
                var ret;
                if(singleColumn) {
                    ret = cellSet[1][2];
                } else {
                    ret = cellSet[2][1];
                }
                return ret;
            }
            function _getButtonCell(cellSet)
            {
                var ret;
                if(singleColumn) {
                    ret = cellSet[2][2];
                } else {
                    ret = cellSet[3][1];
                }
                return ret;
            }

            function _HandleTableData(data) {
                var r = 1;
                var c = 1;
                var results = Csw.number(data.results, -1);

                function _makeNodeCell(nodeObj) {
                    var nodeid = nodeObj.nodeid;

                    if (nodeObj.nodename == "Results Truncated") {
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
                    if(singleColumn) {
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
                    if(singleColumn)
                    {
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


                    var thumbtable = thumbnailCell.table({ width: '100%', cellpadding: 0, cellspacing: 0 });
                    var texttable = textCell.table({ width: '100%', cellpadding: 0, cellspacing: 0 });

                    if (false === Csw.isNullOrEmpty(nodeObj.thumbnailurl)) {
                        thumbtable.cell(1,1).img({
                            src: nodeObj.thumbnailurl
                        }).css({ width: imgwidth });
                    }
                    var moreinfoimg = thumbtable.cell(1,2).css({ width: '25px' })
                        .img({
                           src: 'Images/info.png',
                           title: 'More Info'
                        });
                    moreinfoimg.propNonDom({ valign: 'top' });
                    moreinfoimg.$.hover(function (event) { Csw.nodeHoverIn(event, nodeid, '', 0); }, Csw.nodeHoverOut);

                    thumbnailCell.br();

                    // Name
                    var maintextcell = texttable.cell(1,1);
                    maintextcell.append('<b>' + nodeObj.nodename + '</b>');

                    if (Csw.bool(nodeObj.locked)) {
                        maintextcell.img({
                            src: 'Images/quota/lock.gif',
                            title: 'Quota exceeded'
                        });
                    }
                    maintextcell.br();

                    var btnTable = btncell.table({
                        ID: Csw.controls.dom.makeId(o.ID, nodeid + '_btntbl'),
                        cellspacing: '5px'
                    });
                    var btncol = 1;

                    // Props
                    Csw.crawlObject(nodeObj.props, function (propObj) {
                        if (propObj.fieldtype == "Button") {

                            // Object Class Buttons
                            var propDiv = btnTable.cell(1,btncol).div();
                            propObj.propData.values.mode = 'link';      // force link
                            $.CswFieldTypeFactory('make', {
                                nodeid: nodeid,
                                fieldtype: propObj.fieldtype,
                                propid: propObj.propid,
                                propDiv: propDiv,
                                propData: propObj.propData,
                                ID: Csw.controls.dom.makeId({ ID: o.ID, suffix: propObj.id }),
                                EditMode: Csw.enums.editMode.Table,
                                doSave: function(saveoptions) { 
                                    // Nothing to save in this case, so just call onSuccess
                                    var s = { onSuccess: null };
                                    if(saveoptions) $.extend(s, saveoptions);
                                    Csw.tryExec(s.onSuccess);
                                },
                                onReload: null
                            });
                            btncol += 1;

                        } else {
                            maintextcell.span({text: propObj.propname + ': ' + propObj.gestalt});
                            maintextcell.br();
                        }
                    });

                    // System Buttons
                    if (nodeObj.allowview || nodeObj.allowedit) {
                        var btntext = "View";
                        if (nodeObj.allowedit) {
                            btntext = "Edit";
                        }
                        btnTable.cell(1, btncol).link({
                            ID: Csw.controls.dom.makeId( o.ID, nodeid, 'editbtn' ),
                            text: btntext,
                            //disableOnClick: false,
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

                    if (nodeObj.allowdelete) {
                        btnTable.cell(1, btncol).link({
                            ID: Csw.controls.dom.makeId(o.ID, nodeid, 'btn' ),
                            text: 'Delete',
                            //disableOnClick: false,
                            onClick: function () {
                                $.CswDialog('DeleteNodeDialog', {
                                    nodenames: [nodeObj.nodename],
                                    nodeids: [nodeid],
                                    cswnbtnodekeys: [nodeObj.nodekey],
                                    onDeleteNode: o.onDeleteNode
                                }); // CswDialog
                            } // onClick
                        }); // CswButton
                        btncol += 1;
                    } // if (nodeObj.allowdelete)

                    c += 1;
                    if (c > o.columns) { c = 1; r += 1; }
                }

                if (results === 0) {
                    Csw.tryExec(o.onNoResults, { viewid: o.viewid, viewmode: Csw.enums.viewMode.table.name });
                } else {

                    tableDiv = parent.div({
                        ID: Csw.controls.dom.makeId({ id: o.ID, suffix: '_scrolldiv' })
                        //height: o.maxheight + 'px',
                        //styles: { overflow: 'auto' }
                    });

                    var cellalign = 'left';
                    var cellset = { rows: 3, columns: 1 };
                    var cellspacing = '5px';
                    if(singleColumn) {
                        cellalign = 'left';
                        cellset = { rows: 2, columns: 2 }
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

