/// <reference path="~/js/ChemSW-vsdoc.js" />
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
                maxheight: 600,   // maximum display height of table, in pixels
                tabledata: null
            };
            if (options) $.extend(o, options);

            var $parent = $(this);
            var parent = Csw.controls.factory($parent);
            var scrollingDiv, layoutTable;

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
                    var width = (1 / o.columns * 100) + '%';
                    var thumbnailCell = cellSet[1][1]
                                            .css({
                                                paddingTop: o.rowpadding + 'px',
                                                width: width,
                                                verticalAlign: 'bottom'
                                            });
                    var textCell = cellSet[2][1]
                                            .css({
                                                width: width
                                            });

                    thumbnailCell.$.hover(function (event) { Csw.nodeHoverIn(event, nodeid); }, Csw.nodeHoverOut);
                    textCell.$.hover(function (event) { Csw.nodeHoverIn(event, nodeid); }, Csw.nodeHoverOut);

                    // Name
                    textCell.append('<b>' + nodeObj.nodename + '</b>');

                    if (false === Csw.isNullOrEmpty(nodeObj.thumbnailurl)) {
                        thumbnailCell.img({
                            src: nodeObj.thumbnailurl
                        }).css({ width: '40%' });
                    }
                    thumbnailCell.br();

                    if (Csw.bool(nodeObj.locked)) {
                        textCell.img({
                            src: 'Images/quota/lock.gif',
                            title: 'Quota exceeded'
                        });
                    }
                    textCell.br();

                    // Props
                    Csw.crawlObject(nodeObj.props, function (propObj) {
                        if (propObj.fieldtype == "Button") {

                            var propDiv = textCell.div();
                            $.CswFieldTypeFactory('make', {
                                nodeid: nodeid,
                                fieldtype: propObj.fieldtype,
                                propDiv: propDiv,
                                propData: propObj.propData,
                                ID: Csw.controls.dom.makeId({ ID: o.ID, suffix: propObj.id }),
                                EditMode: Csw.enums.EditMode.Table
                            });

                        } else {
                            textCell.span({text: propObj.propname + ': ' + propObj.gestalt});
                        }
                        textCell.br();
                    });

                    // Buttons
                    var btnTable = textCell.table({
                        ID: Csw.controls.dom.makeId(o.ID, nodeid + '_btntbl')
                    });

                    if (nodeObj.allowview || nodeObj.allowedit) {
                        var btntext = "View";
                        if (nodeObj.allowedit) {
                            btntext = "Edit";
                        }
                        btnTable.cell(1, 1).button({
                            ID: Csw.controls.dom.makeId( o.ID, nodeid, 'editbtn' ),
                            enabledText: btntext,
                            disableOnClick: false,
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
                    } // if (nodeObj.allowview || nodeObj.allowedit) 

                    if (nodeObj.allowdelete) {
                        btnTable.cell(1, 2).button({
                            ID: Csw.controls.dom.makeId(o.ID, nodeid, 'btn' ),
                            enabledText: 'Delete',
                            disableOnClick: false,
                            onClick: function () {
                                $.CswDialog('DeleteNodeDialog', {
                                    nodenames: [nodeObj.nodename],
                                    nodeids: [nodeid],
                                    cswnbtnodekeys: [nodeObj.nodekey],
                                    onDeleteNode: o.onDeleteNode
                                }); // CswDialog
                            } // onClick
                        }); // CswButton
                    } // if (nodeObj.allowdelete)

                    c += 1;
                    if (c > o.columns) { c = 1; r += 1; }
                }

                if (results === 0) {
                    Csw.tryExec(o.onNoResults, { viewid: o.viewid, viewmode: Csw.enums.viewMode.table.name });
                } else {

                    scrollingDiv = parent.div({
                        ID: Csw.controls.dom.makeId({ id: o.ID, suffix: '_scrolldiv' }),
                        height: o.maxheight + 'px',
                        styles: { overflow: 'auto' }
                    });

                    layoutTable = scrollingDiv.layoutTable({
                        ID: o.ID + '_tbl',
                        cellSet: { rows: 2, columns: 1 },
                        cellalign: 'center',
                        width: '100%',
                        cellspacing: '5px'
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

