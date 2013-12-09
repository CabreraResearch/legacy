/// <reference path="app/CswApp-vsdoc.js" />

(function () {

    Csw.nbt.register('nodeTable', function (cswParent, params) {
        'use strict';
        var cswPrivate = {
            viewid: '',
            name: '',
            cssclass: '',
            //                nodeid: '',
            //                nodekey: '',
            EditMode: Csw.enums.editMode.Edit,
            onEditNode: null,
            onDeleteNode: null,
            onSuccess: null,
            onNoResults: null,  // function({viewid, viewmode})
            //columns: 3,      // number of columns to use
            pagenodelimit: 20,
            rowpadding: 10,  // padding between table rows, in pixels
            //maxheight: 600,   // maximum display height of table, in pixels
            tabledata: null,
            allowEdit: true,
            allowDelete: true,
            chemCatConfig: {
                allowImport: true,
                importMenuItems: [],
                importButtons: {}
            },
            searchTarget: null, //c3 addition

            compactResults: false,
            suppressButtons: false,
            extraAction: null,
            extraActionIcon: Csw.enums.iconType.none,
            onExtraAction: null,  // function(nodeObj) {}
            properties: {},
            onMoreClick: function (nodetypeid, nodetypename) {
                // default behavior: rerun with filtered nodetype
                cswPrivate.filterToNodeTypeId = nodetypeid;
                cswPrivate.originaltabledata = cswPrivate.tabledata;
                cswPrivate.tabledata = null;
                cswPrivate.init(function () {
                    cswPrivate.tableDiv.a({
                        text: 'Back to All Results',
                        onClick: function () {
                            cswPrivate.filterToNodeTypeId = '';
                            cswPrivate.tabledata = cswPrivate.originaltabledata;
                            cswPrivate.init();
                        }
                    });
                });
            },
            filterToNodeTypeId: '',
            filterOutNodeId: '',
            forceSingleColumn: false
        };
        if (params) Csw.extend(cswPrivate, params, true);

        var cswPublic = {};


        cswPrivate.getRowHeaderCell = function (cellSet) {
            var ret;
            if (cswPrivate.singleColumn) {
                ret = cellSet[1][1]
                        .propDom({ colspan: 3 })
                        .addClass('SearchSubLabel');
            } else {
                ret = cellSet[1][1]
                        .propDom({ colspan: 4 })
                        .addClass('SearchSubLabel');
            }
            return ret;
        };

        cswPrivate.getThumbnailCell = function (cellSet) {
            var ret;
            if (cswPrivate.singleColumn) {
                ret = cellSet[2][1];
            } else {
                ret = cellSet[2][1];
            }
            return ret;
        };


        cswPrivate.getTextCell = function (cellSet) {
            var ret;
            if (cswPrivate.singleColumn) {
                ret = cellSet[2][2];
            } else {
                ret = cellSet[3][1];
            }
            return ret;
        };


        cswPrivate.getButtonCell = function (cellSet) {
            var ret;
            if (cswPrivate.singleColumn) {
                ret = cellSet[2][3];
            } else {
                ret = cellSet[4][1];
            }
            return ret;
        };


        cswPrivate.makeNodeCell = function(nodeObj) {
            var nodeid = nodeObj.nodeid;
            if (null === cswPrivate.filterOutNodeId || nodeid !== cswPrivate.filterOutNodeId) {
                if (cswPrivate.c <= cswPrivate.columns) {
                    if ((false == cswPrivate.singleColumn || // paging handled in makeTable()
                        cswPrivate.pagenodecount >= cswPrivate.pagenodelimit * (cswPrivate.currentpage - 1)) &&
                        (cswPrivate.pagenodecount < cswPrivate.pagenodelimit * cswPrivate.currentpage)) {


                        var cellSet = cswPrivate.layoutTable.cellSet(cswPrivate.r, cswPrivate.c);
                        var textwidth = (1 / (cswPrivate.columns + 1) * 100) + '%';
                        var imgheight = '';
                        var thumbverticalAlign = 'middle';
                        var thumbhorizontalAlign = 'center';
                        var thumbBackgroundColor = '#ffffff';
                        var cellpad = cswPrivate.rowpadding + 'px';
                        if (cswPrivate.singleColumn) {
                            textwidth = '75%';
                            cellpad = '10px';
                            thumbverticalAlign = 'top';
                            thumbhorizontalAlign = '';
                            thumbBackgroundColor = '';
                        }
                        if (Csw.bool(cswPrivate.compactResults)) {
                            cellpad = '0px';
                        }

                        var thumbnailCell = cswPrivate.getThumbnailCell(cellSet)
                            .css({
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
                                paddingBottom: cellpad
                            });
                            textCell.css({
                                paddingBottom: cellpad
                            });
                        }
                        var btncell = cswPrivate.getButtonCell(cellSet)
                            .css({
                                paddingBottom: cellpad
                            });

                        // Banding
                        if (cswPrivate.singleColumn) {
                            if (cswPrivate.r % 2 === 1) {
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
                        var texttable = textCell.table({ width: '100%', cellpadding: 0, cellspacing: 0 });
                        cswPrivate.texttables.push(texttable);

                        if (Csw.bool(cswPrivate.compactResults)) {
                            texttable.css({ paddingBottom: '10px' });
                            texttable.hide();
                            imgheight = '18px';
                        }

                        if (false === Csw.isNullOrEmpty(nodeObj.thumbnailurl)) {
                            thumbnailCell.img({
                                src: nodeObj.thumbnailurl
                            }).css({
                                height: imgheight,
                                maxWidth: '100px'
                            });
                        }

                        thumbnailCell.$.hover(
                            function(event) {
                                Csw.nodeHoverIn(event, { nodeid: nodeid, nodename: nodeObj.nodename, parentDiv: thumbnailCell });
                            },
                            function(event) {
                                Csw.nodeHoverOut();
                            });
                        textCell.$.hover(
                            function(event) {
                                Csw.nodeHoverIn(event, { nodeid: nodeid, nodename: nodeObj.nodename, parentDiv: thumbnailCell }); // yes, thumbnailCell.
                            },
                            function(event) {
                                Csw.nodeHoverOut();
                            });

                        var btnTable = btncell.table({
                            name: cswPrivate.name + '_' + nodeid + '_btntbl',
                            cellspacing: '5px'
                        });
                        var btncol = 1;
                        var row = 1;
                        var tabid = window.Ext.id();
                        // Props
                        Csw.iterate(nodeObj.props, function(propObj) {
                            if (propObj.fieldtype === "Button") {
                                if (false == Csw.bool(cswPrivate.suppressButtons)) {
                                    // Object Class Buttons
                                    propObj.size = 'small';
                                    propObj.nodeid = nodeid;
                                    propObj.tabState = propObj.tabState || {};
                                    propObj.tabState.nodeid = nodeid;
                                    propObj.tabState.tabid = tabid;
                                    propObj.tabid = tabid;
                                    propObj.name = propObj.propname;
                                    propObj.EditMode = Csw.enums.editMode.Table;
                                    propObj.onRefresh = cswPrivate.onEditNode;

                                    var width = (propObj.propData.values.selectedText.length > propObj.name.length ? propObj.propData.values.selectedText.length * 8 + 5 : propObj.name.length * 6 + 14);
                                    var buttonDiv = btnTable.cell(1, btncol).div().css({ 'width': width });
                                    var fieldOpt = Csw.nbt.propertyOption(propObj, buttonDiv);

                                    Csw.nbt.property(fieldOpt);

                                    btncol += 1;
                                }
                            } else {
                                var propCell = texttable.cell(Csw.number(propObj.row, row), Csw.number(propObj.column, 1));
                                var cssclass = 'searchResult';
                                if (propObj.source === 'Results') {
                                    cssclass = 'searchResultDeemph';
                                }
                                propCell.span({
                                    text: propObj.propname + ': ' + propObj.gestalt,
                                    cssclass: cssclass
                                });
                                row += 1;
                                //maintextcell.br();
                            }
                        });
                        Csw.publish('render_' + nodeid + '_' + tabid);
                        
                        // System Buttons
                        if (Csw.bool(cswPrivate.compactResults)) {
                            btnTable.cell(1, btncol).buttonExt({
                                name: Csw.delimitedString(cswPrivate.name, nodeid, 'morebtn').string('_'),
                                width: ('More Info'.length * 8) + 16,
                                enabledText: 'More Info',
                                icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.info),
                                disableOnClick: false,
                                onClick: function() {
                                    texttable.toggle();
                                } // onClick
                            }); // CswButton
                            btncol += 1;
                        }

                        //Details Button
                        if (Csw.bool(cswPrivate.allowEdit) && (Csw.bool(nodeObj.allowview) || Csw.bool(nodeObj.allowedit))) {
                            btnTable.cell(1, btncol).buttonExt({
                                name: Csw.delimitedString(cswPrivate.name, nodeid, 'editbtn').string('_'),
                                width: ('Details'.length * 7) + 16,
                                enabledText: 'Details',
                                disableOnClick: false,
                                icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.pencil),
                                onClick: function() {
                                    //If C3 search {} else if Universal search {}
                                    if (cswPrivate.searchTarget === "chemcatcentral") {
                                        $.CswDialog('C3DetailsDialog', {
                                            nodeObj: nodeObj,
                                            onEditNode: cswPrivate.onEditNode
                                        });
                                    } else {
                                        $.CswDialog('EditNodeDialog', {
                                            currentNodeId: nodeid,
                                            currentNodeKey: nodeObj.nodekey,
                                            nodenames: [nodeObj.nodename],
                                            ReadOnly: (false === nodeObj.allowedit),
                                            onEditNode: cswPrivate.onEditNode,
                                            onEditView: function(viewid) {
                                                Csw.main.handleAction({
                                                    actionname: 'Edit_View',
                                                    ActionOptions: {
                                                        viewid: viewid,
                                                        viewmode: Csw.enums.viewMode.grid.name,
                                                        startingStep: 2,
                                                        IgnoreReturn: true
                                                    }
                                                });
                                            }
                                        });
                                    } // CswDialog
                                } // onClick
                            }); // CswButton
                            btncol += 1;
                        } // if (nodeObj.allowview || nodeObj.allowedit) 

                        //Delete Button
                        if (false == Csw.bool(cswPrivate.suppressButtons) && Csw.bool(cswPrivate.allowDelete) && Csw.bool(nodeObj.allowdelete)) {
                            var deleteBtn = btnTable.cell(1, btncol).buttonExt({
                                name: Csw.delimitedString(cswPrivate.name, nodeid, 'morebtn').string('_'),
                                width: ('Delete'.length * 8) + 16,
                                enabledText: 'Delete',
                                disabledOnClick: false,
                                //tooltip: { title: 'Delete' },
                                icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.trash),
                                onClick: Csw.method(function() {
                                    $.CswDialog('DeleteNodeDialog', {
                                        nodenames: [nodeObj.nodename],
                                        nodeids: [nodeid],
                                        cswnbtnodekeys: [nodeObj.nodekey],
                                        onDeleteNode: cswPrivate.onDeleteNode
                                    }); // CswDialog
                                    deleteBtn.enable();
                                }) // onClick
                            }); // CswButton
                            btncol += 1;
                        } // if (nodeObj.allowdelete)

                        //Import Button
                        if (false == Csw.bool(cswPrivate.suppressButtons) && Csw.bool(cswPrivate.chemCatConfig.allowImport) && Csw.bool(nodeObj.allowimport)) {

                            var importMenuItems = [];
                            Csw.each(cswPrivate.chemCatConfig.importMenuItems, function(nt) {
                                if (false === Csw.isNullOrEmpty(nt.nodetypename)) {
                                    importMenuItems.push({
                                        text: 'Import ' + nt.nodetypename,
                                        ntname: nt.nodetypename,
                                        ntid: nt.nodetypeid,
                                        icon: nt.iconfilename,
                                        handler: Csw.method(function() {
                                            return importOnClick(nt.nodetypename, nt.nodetypeid);
                                        })
                                    });
                                }
                            }); //Csw.each()

                            var importOnClick = function(nodetypename, nodetypeid) {
                                // Disable all import buttons so Masotti can't create an "import conga line"
                                Csw.iterate(cswPrivate.chemCatConfig.importButtons, function(button, name) {
                                    button.disable();
                                });


                                Csw.ajaxWcf.post({
                                    urlMethod: 'ChemCatCentral/importProduct',
                                    data: {
                                        C3ProductId: nodeObj.c3productid,
                                        NodeTypeName: nodetypename,
                                        NodeTypeId: nodetypeid
                                    },
                                    success: function(data) {
                                        Csw.publish(Csw.enums.events.main.handleAction, data);
                                    },
                                    error: function(data) {
                                        // Re-enable all import buttons
                                        Csw.iterate(cswPrivate.chemCatConfig.importButtons, function(button, name) {
                                            button.enable();
                                        });
                                    },
                                    complete: function(data) {
                                        // Re-enable all import buttons
                                        Csw.iterate(cswPrivate.chemCatConfig.importButtons, function(button, name) {
                                            button.enable();
                                        });
                                    }
                                }); // ajaxWcf
                            };

                            cswPrivate.chemCatConfig.importButtons['button' + cswPrivate.r] = window.Ext.create('Ext.SplitButton', {
                                text: importMenuItems[0].text,
                                icon: importMenuItems[0].icon,
                                width: (importMenuItems[0].text.length * 8) + 16,
                                renderTo: btnTable.cell(1, btncol).getId(),
                                handler: Csw.method(function() {
                                    importOnClick(importMenuItems[0].ntname, importMenuItems[0].ntid);
                                }),
                                menu: {
                                    items: importMenuItems
                                }
                            }); //importButton

                            btncol += 1;

                        } //nodeObj.allowimport

                        if (false === Csw.isNullOrEmpty(cswPrivate.extraAction)) {
                            Csw.debug.assert(Csw.isFunction(cswPrivate.onExtraAction), 'No method specified for extraAction.');

                            btnTable.cell(1, btncol).buttonExt({
                                name: Csw.delimitedString(cswPrivate.name, nodeid, 'extbtn').string('_'),
                                width: (cswPrivate.extraAction.length * 8) + 16,
                                enabledText: cswPrivate.extraAction,
                                //tooltip: { title: cswPrivate.extraAction },
                                icon: cswPrivate.extraActionIcon,
                                disableOnClick: false,
                                onClick: function() {
                                    Csw.tryExec(cswPrivate.onExtraAction, nodeObj);
                                } // onClick
                            }); // CswButton
                            btncol += 1;
                        } // if (nodeObj.allowdelete)

                        //Favorite Button
                        if (cswPrivate.searchTarget != "chemcatcentral") {
                            btnTable.cell(1, btncol).favoriteButton({
                                name: nodeid + '_favBtn',
                                nodeid: nodeid,
                                isFavorite: Csw.bool(nodeObj.isFavorite),
                            });
                            btncol += 1;
                        }

                        if (Csw.bool(nodeObj.disabled)) {
                            textCell.addClass('disabled');
                            btnTable.addClass('disabled');
                        }

                        if (cswPrivate.singleColumn) {
                            cswPrivate.r += 1;
                        } else {
                            cswPrivate.c += 1;
                        }
                    } // if((pagenodecount < pagenodelimit * (currentpage - 1))
                    cswPrivate.pagenodecount += 1;
                } // if (cswPrivate.c <= cswPrivate.columns) {
            } // if (null === cswPrivate.filterOutNodeId || nodeid !== cswPrivate.filterOutNodeId) 
        }; // makeNodeCell()


        cswPrivate.makeTable = function () {
            var i;
            cswPrivate.tableDiv.empty();
            cswPrivate.pagerDiv.empty();
            cswPrivate.r = 1;
            cswPrivate.c = 1;
            cswPrivate.pagenodecount = 0;
            cswPrivate.texttables = [];

            var cellalign = 'left';
            var cellset = { rows: 4, columns: 1 };
            var cellspacing = '5px';
            if (cswPrivate.singleColumn) {
                cellalign = 'left';
                cellset = { rows: 2, columns: 3 };
                cellspacing = '0px';
            }

            cswPrivate.layoutTable = cswPrivate.tableDiv.layoutTable({
                name: cswPrivate.name + '_tbl',
                cellSet: cellset,
                cellalign: cellalign,
                width: '100%',
                cellspacing: cellspacing,
                TableCssClass: cswPrivate.cssclass
            });

            // Iterate Nodes per Nodetype
            Csw.each(cswPrivate.tabledata.nodetypes, function (nodetypeObj) {

                var results = Csw.number(nodetypeObj["results"]);

                var buffer = results;
                if (results > cswPrivate.columns) {
                    buffer = cswPrivate.columns;
                }
                if (cswPrivate.singleColumn || // paging handled in makeNodeCell
                    ((cswPrivate.pagenodecount + buffer) >= cswPrivate.pagenodelimit * (cswPrivate.currentpage - 1)) &&
                    ((cswPrivate.pagenodecount + buffer) < cswPrivate.pagenodelimit * cswPrivate.currentpage)) {

                    Csw.eachRecursive(nodetypeObj.nodes, cswPrivate.makeNodeCell);
                    
                    var nodetypeid = nodetypeObj["nodetypeid"];
                    var nodetypename = nodetypeObj["nodetypename"];
                    var handleClick = function() {
                        Csw.tryExec(cswPrivate.onMoreClick, nodetypeid, nodetypename);
                    };
                    
                    if (false === cswPrivate.singleColumn) {

                        // empty cells if no results, to keep image in place
                        while (cswPrivate.c <= cswPrivate.columns) {
                            var cellSet = cswPrivate.layoutTable.cellSet(cswPrivate.r, cswPrivate.c);
                            var textCell = cswPrivate.getTextCell(cellSet);
                            textCell.append('&nbsp;');
                            cswPrivate.c += 1;
                        }

                        var startCellSet = cswPrivate.layoutTable.cellSet(cswPrivate.r, 1);
                        var startHeaderCell = cswPrivate.getRowHeaderCell(startCellSet);
                        startHeaderCell.a({
                            text: nodetypename + ' Results (' + results + ')',
                            onClick: handleClick
                        });

                        if (results > 3) {
                            cswPrivate.makeEndCell(nodetypename, handleClick);
                        } // if (results > 3) {

                        cswPrivate.c = 1;
                        cswPrivate.r += 1;
                    } // if (false === cswPrivate.singleColumn)
                    else 
                    {
                        if (results > 3 && Csw.number(cswPrivate.tabledata.nodetypecount) > 1 ) {
                            var endTextCell = cswPrivate.makeEndCell(nodetypename, handleClick);
                            endTextCell.br();
                            endTextCell.br();
                            cswPrivate.r += 1;
                        }
                    }
                } else {
                    cswPrivate.pagenodecount += buffer;
                }
            });

            // Pager control
            if (cswPrivate.totalpages > 1) {

                if (cswPrivate.currentpage > 1) {
                    cswPrivate.pagerDiv.a({
                        name: 'tableprev',
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
                    name: 'pageselect',
                    values: pageoptions,
                    selected: Csw.string(cswPrivate.currentpage),
                    onChange: function () {
                        cswPrivate.currentpage = Csw.number(pagesel.val());
                        cswPrivate.makeTable();
                    }
                });

                if (cswPrivate.currentpage < cswPrivate.totalpages) {
                    cswPrivate.pagerDiv.a({
                        name: 'tablenext',
                        text: 'Next Page',
                        onClick: function () {
                            cswPrivate.currentpage += 1;
                            cswPrivate.makeTable();
                        }
                    });
                }
            } // if (pagenodelimit < results)

            // Show truncated
            if (Csw.bool(cswPrivate.tabledata.truncated)) {
                cswPrivate.pagerDiv.br();
                cswPrivate.pagerDiv.span({
                    cssclass: 'truncated',
                    text: 'Results Truncated'
                });
            }
        }; // makeTable()

        cswPrivate.makeEndCell = function(nodetypename, onClick) {
            var endCellSet = cswPrivate.layoutTable.cellSet(cswPrivate.r, cswPrivate.c);
            var endTextCell = cswPrivate.getTextCell(endCellSet);
            endTextCell.a({
                text: 'More ' + nodetypename + ' Results...',
                onClick: onClick
            });
            return endTextCell;
        }; // makeEndCell()



        cswPrivate.HandleTableData = function () {
            cswParent.empty();
            cswPrivate.results = Csw.number(cswPrivate.tabledata.results, -1);
            cswPrivate.pagenodelimit = Csw.number(cswPrivate.tabledata.pagesize, 20);
            cswPrivate.searchTarget = cswPrivate.tabledata.searchtarget; //c3 addition

            // multi-nodetype
            cswPrivate.singleColumn = false;
            cswPrivate.columns = 3;
            if (Csw.number(cswPrivate.tabledata.nodetypecount) <= 1 ) {
                // single nodetype
                cswPrivate.singleColumn = true;
                cswPrivate.columns = 1;
            }

            // calculate totalpages
            if (cswPrivate.singleColumn) {
                cswPrivate.totalpages = Math.ceil(cswPrivate.results / cswPrivate.pagenodelimit);
            } else {
                var constrainedResults = 0;
                Csw.each(cswPrivate.tabledata.nodetypes, function (nodetypeObj) {
                    var add = Csw.number(nodetypeObj.results);
                    if (add > cswPrivate.columns) {
                        add = cswPrivate.columns;
                    }
                    constrainedResults += add;
                });
                cswPrivate.totalpages = Math.ceil(constrainedResults / cswPrivate.pagenodelimit);
            }

            // this needs to be after calculating totalpages
            if (Csw.bool(cswPrivate.forceSingleColumn)) {
                cswPrivate.singleColumn = true;
                cswPrivate.columns = 1;
            }
            
            if (cswPrivate.results === 0) {
                Csw.tryExec(cswPrivate.onNoResults, {
                    viewid: cswPrivate.viewid,
                    viewmode: Csw.enums.viewMode.table.name
                });
            } else {
                cswPrivate.tableDiv = cswParent.div({
                    name: 'scrolldiv'
                }).css({ width: '100%' });
                cswPrivate.pagerDiv = cswParent.div({
                    name: 'pagerdiv'
                });
                cswPrivate.makeTable();
            }
            Csw.tryExec(cswPrivate.onSuccess);
        }; // HandleTableData()

        cswPrivate.init = function (onAfterInit) {
            cswPrivate.r = 1;
            cswPrivate.c = 1;
            cswPrivate.currentpage = 1;
            cswPrivate.pagenodecount = 0;

            if (false === Csw.isNullOrEmpty(cswPrivate.tabledata)) {
                cswPrivate.HandleTableData();
                Csw.tryExec(onAfterInit);
            } else {
                Csw.ajax.deprecatedWsNbt({
                    urlMethod: 'getTableView',
                    data: {
                        ViewId: cswPrivate.viewid,
                        // NodeId: cswPrivate.nodeid,
                        // NodeKey: cswPrivate.nodekey,
                        NodeTypeId: cswPrivate.filterToNodeTypeId
                    },
                    success: function (result) {
                        cswPrivate.tabledata = result;
                        cswPrivate.HandleTableData(onAfterInit);
                        Csw.tryExec(onAfterInit);
                    }
                }); // ajax
            }
        }; // init()

        // constructor
        (function () {
            cswPrivate.init();
        })(); // constructor

        cswPublic.expandAll = function () {
            Csw.each(cswPrivate.texttables, function (texttable) {
                texttable.toggle();
            });
        }; // expandAll()

        return cswPublic;
    }); // register
})();                  // (function
