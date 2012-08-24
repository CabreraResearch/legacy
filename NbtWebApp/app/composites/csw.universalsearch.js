
/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {


    Csw.composites.universalSearch = Csw.composites.universalSearch ||
        Csw.composites.register('universalSearch', function (cswParent, params) {
            'use strict';
            var cswPrivate = {
                ID: 'newsearch',
                $searchbox_parent: null,
                $searchresults_parent: null,
                $searchfilters_parent: null,
                nodetypeid: '',       // automatically filter results to this nodetype
                objectclassid: '',    // automatically filter results to this objectclass
                onBeforeSearch: null,
                onAfterSearch: null,
                onAfterNewSearch: null,
                onLoadView: null,
                onAddView: null,
                //searchresults_maxheight: '600',
                searchbox_width: '200px',
                showSaveAsView: true,
                allowEdit: true,
                allowDelete: true,
                extraAction: null,
                extraActionIcon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.none),
                onExtraAction: null,  // function(nodeObj) {}
                compactResults: false,

                newsearchurl: '/NbtWebApp/wsNBT.asmx/doUniversalSearch',
                filtersearchurl: '/NbtWebApp/wsNBT.asmx/filterUniversalSearch',
                restoresearchurl: '/NbtWebApp/wsNBT.asmx/restoreUniversalSearch',
                saveurl: '/NbtWebApp/wsNBT.asmx/saveSearchAsView',
                //filters: {},
                sessiondataid: '',
                searchterm: '',
                filterHideThreshold: 5,
                buttonSingleColumn: '',
                buttonMultiColumn: ''
            };
            if (params) {
                Csw.extend(cswPrivate, params);
            }
            if(false === Csw.isNullOrEmpty(cswParent)) {
                cswPrivate.table = cswParent.table({
                    ID: Csw.makeId(cswPrivate.ID, 'table'),
                    cellpadding: '2px'
                });
                
                var cell11 = cswPrivate.table.cell(1, 1);

                cell11.propDom('colspan', 2);

                cswPrivate.$searchbox_parent = cell11.div({ ID: 'searchdialog_searchdiv' }).$;
                cswPrivate.$searchresults_parent = cswPrivate.table.cell(2, 2).div({ ID: 'searchdialog_resultsdiv' }).$;
                cswPrivate.$searchfilters_parent = cswPrivate.table.cell(2, 1).div({ ID: 'searchdialog_filtersdiv' }).$;
            }
            var cswPublic = {};

            // Constructor
            // Adds a searchbox to the form
            (function () {
                cswPrivate.$searchbox_parent.empty();
                var cswtable = Csw.literals.table({
                    ID: Csw.makeId(cswPrivate.ID, '', '_div'),
                    $parent: cswPrivate.$searchbox_parent
                });

                cswPrivate.searchinput = cswtable.cell(1, 1).input({
                    ID: Csw.makeId(cswPrivate.ID, '', '_input'),
                    type: Csw.enums.inputTypes.search,
                    width: cswPrivate.searchbox_width,
                    cssclass: 'mousetrap'
                });

                cswPrivate.searchbutton = cswtable.cell(1, 2).div().buttonExt({
                    ID: Csw.makeId(cswPrivate.ID, '', '_srchbtn'),
                    icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.search),
                    enabledText: 'Search',
                    disabledText: 'Searching...',
                    bindOnEnter: true,
                    onClick: function () {
                        cswPrivate.searchterm = cswPrivate.searchinput.val();
                        //cswPrivate.filters = {};
                        cswPrivate.newsearch();
                    }
                });
            })();

            // Handle search submission
            cswPrivate.newsearch = function () {
                Csw.tryExec(cswPrivate.onBeforeSearch);

                Csw.ajax.post({
                    url: cswPrivate.newsearchurl,
                    data: {
                        SearchTerm: cswPrivate.searchterm,
                        NodeTypeId: cswPrivate.nodetypeid,
                        ObjectClassId: cswPrivate.objectclassid
                    },
                    success: function (data) {
                        cswPrivate.handleResults(data);
                        Csw.tryExec(cswPrivate.onAfterNewSearch, cswPrivate.sessiondataid);
                    }
                });
            }; // search()

            cswPrivate.handleResults = function(data) {
                var fdiv, ftable, filtersdivid;

                cswPrivate.sessiondataid = data.sessiondataid;

                // Search results

                function _renderResultsTable(columns) {
                    var nodeTable;
                    
                    cswPrivate.$searchresults_parent.contents().remove();
                    cswPrivate.$searchresults_parent.css({ paddingTop: '15px' });

                    var resultstable = Csw.literals.table({
                        ID: Csw.makeId(cswPrivate.ID, '', 'resultstbl'),
                        $parent: cswPrivate.$searchresults_parent,
                        width: '100%'
                    });

                    resultstable.cell(1, 1).append('<b>Search Results: (' + data.table.results + ')</b>');
                    
                    if(Csw.bool(cswPrivate.compactResults))
                    {
                        resultstable.cell(1, 2).css({ width: '100px' });
                        cswPrivate.linkExpandAll = resultstable.cell(1, 2).a({
                            ID: Csw.makeId(cswPrivate.ID, '', '_expandall'),
                            text: 'Expand All',
                            onClick: function() {
                                if(cswPrivate.linkExpandAll.text() === 'Expand All'){
                                    cswPrivate.linkExpandAll.text('Collapse All');
                                }
                                else if(cswPrivate.linkExpandAll.text() === 'Collapse All'){
                                    cswPrivate.linkExpandAll.text('Expand All');
                                }
                                nodeTable.expandAll();
                            }
                        });
                    }
                    resultstable.cell(1, 3).css({ width: '18px' });
                    cswPrivate.buttonSingleColumn = resultstable.cell(1, 3).imageButton({
                        ID: Csw.makeId(cswPrivate.ID, '', '_singlecol'),
                        ButtonType: Csw.enums.imageButton_ButtonType.TableSingleColumn,
                        Active: (columns === 1),
                        AlternateText: 'Single Column',
                        onClick: function() {
                            setTimeout(function() { // so we see the clear immediately
                                _renderResultsTable(1);
                            }, 0);
                        }
                    });

                    resultstable.cell(1, 4).css({ width: '18px' });
                    cswPrivate.buttonMultiColumn = resultstable.cell(1, 4).imageButton({
                        ID: Csw.makeId(cswPrivate.ID, '', '_multicol'),
                        ButtonType: Csw.enums.imageButton_ButtonType.TableMultiColumn,
                        Active: (columns !== 1),
                        AlternateText: 'Multi Column',
                        onClick: function() {
                            setTimeout(function() { // so we see the clear immediately
                                _renderResultsTable(3);
                            }, 0);
                        }
                    });

                    resultstable.cell(2, 1).propDom({ 'colspan': 3 });

                    nodeTable = Csw.nbt.nodeTable(resultstable.cell(2, 1), {
                        ID: Csw.makeId(cswPrivate.ID, '', 'srchresults'),
                        onEditNode: function() {
                            // case 27245 - refresh on edit
                            cswPublic.restoreSearch(cswPrivate.sessiondataid);
                        },
                        onDeleteNode: function() {
                            // case 25380 - refresh on delete
                            cswPublic.restoreSearch(cswPrivate.sessiondataid);
                        },
                        //onSuccess: cswPrivate.onAfterSearch,
                        onNoResults: function() {
                            resultstable.cell(2, 1).text('No Results Found');
                        },
                        tabledata: data.table,
                        //maxheight: cswPrivate.searchresults_maxheight
                        columns: columns,
                        allowEdit: cswPrivate.allowEdit,
                        allowDelete: cswPrivate.allowEdit,
                        extraAction: cswPrivate.extraAction,
                        extraActionIcon: cswPrivate.extraActionIcon,
                        onExtraAction: cswPrivate.onExtraAction,
                        compactResults: cswPrivate.compactResults
                    });
                }

                _renderResultsTable(1);

                // Filter panel
                cswPrivate.$searchfilters_parent.contents().remove();

                filtersdivid = Csw.makeId(cswPrivate.ID, '', 'filtersdiv');
                fdiv = Csw.literals.div({
                    ID: filtersdivid,
                    $parent: cswPrivate.$searchfilters_parent
                }).css({
                    paddingTop: '15px'
                    //height: cswPrivate.searchresults_maxheight + 'px',
                    //overflow: 'auto'
                });

                fdiv.span({ text: 'Searched For: ' + data.searchterm }).br();
                ftable = fdiv.table({ });

                // Filters in use
                var hasFilters = false;
                var ftable_row = 1;

                function showFilter(thisFilter) {
                    var cell1 = ftable.cell(ftable_row, 1);
                    cell1.propDom('align', 'right');
                    cell1.span({
                        text: thisFilter.filtername + ':&nbsp;'
                    });
                    ftable.cell(ftable_row, 2).span({
                        text: thisFilter.filtervalue
                    });
                    if (Csw.bool(thisFilter.removeable)) {
                        ftable.cell(ftable_row, 3).imageButton({
                            ID: Csw.makeId(filtersdivid, '', thisFilter.filterid),
                            ButtonType: Csw.enums.imageButton_ButtonType.Delete,
                            AlternateText: 'Remove Filter',
                            onClick: function() {
                                cswPrivate.filter(thisFilter, 'remove');
                            }
                        });
                    }
                    ftable_row++;
                    hasFilters = true;
                }

                Csw.each(data.filtersapplied, showFilter);

                if (hasFilters && cswPrivate.showSaveAsView) {
                    fdiv.br();
                    fdiv.buttonExt({
                        ID: Csw.makeId(filtersdivid, '', "saveview"),
                        enabledText: 'Save as View',
                        disableOnClick: false,
                        icon: Csw.enums.getName( Csw.enums.iconType, Csw.enums.iconType.save),
                        onClick: cswPrivate.saveAsView
                    });
                }
                fdiv.br();
                fdiv.br();

                // Filters to add

                function makeFilterLink(thisFilter, div, filterCount) {
                    var flink = div.a({
                        ID: Csw.makeId(filtersdivid, '', thisFilter.filterid),
                        text: thisFilter.filtervalue + ' (' + thisFilter.count + ')',
                        onClick: function() {
                            cswPrivate.filter(thisFilter, 'add');
                            return false;
                        }
                    });
                    div.br();
                }

                function makeFilterSet(thisFilterSet, Name) {

                    var thisfilterdivid = Csw.makeId(filtersdivid, '', Name);
                    //var thisfilterdiv = fdiv.div({ ID: thisfilterdivid });
                    var filterCount = 0;
                    var moreDiv = Csw.literals.moreDiv({
                        ID: thisfilterdivid,
                        $parent: fdiv.$
                    });

                    moreDiv.shownDiv.append('<b>' + Name + ':</b>');
                    moreDiv.shownDiv.br();
                    var thisdiv = moreDiv.shownDiv;
                    moreDiv.moreLink.hide();
                    Csw.each(thisFilterSet, function(thisFilter) {
                        if (filterCount === cswPrivate.filterHideThreshold) {
                            moreDiv.moreLink.show();
                            thisdiv = moreDiv.hiddenDiv;
                        }
                        makeFilterLink(thisFilter, thisdiv, filterCount);
                        filterCount++;
                    });
                    fdiv.br();
                    fdiv.br();
                }

                Csw.each(data.filters, makeFilterSet);

                cswPrivate.data = data;

                Csw.tryExec(cswPrivate.onAfterSearch, cswPublic);
            }; // handleResults()


            cswPrivate.filter = function (thisFilter, action) {
                //cswPrivate.filters[thisFilter.filterid] = thisFilter;

                Csw.tryExec(cswPrivate.onBeforeSearch);
                Csw.ajax.post({
                    url: cswPrivate.filtersearchurl,
                    data: {
                        SessionDataId: cswPrivate.sessiondataid,
                        Filter: JSON.stringify(thisFilter),
                        Action: action
                    },
                    success: cswPrivate.handleResults
                });
            }; // filter()

            cswPrivate.saveAsView = function () {
                $.CswDialog('AddViewDialog', {
                    ID: Csw.makeId(cswPrivate.ID, '', 'addviewdialog'),
                    //viewmode: 'table',
                    category: 'Saved Searches',
                    onAddView: function (newviewid, viewmode) {

                        Csw.ajax.post({
                            url: cswPrivate.saveurl,
                            data: {
                                SessionDataId: cswPrivate.sessiondataid,
                                ViewId: newviewid
                            },
                            success: function (data) {
                                Csw.tryExec(cswPrivate.onAddView, newviewid, viewmode);
                                Csw.tryExec(cswPrivate.onLoadView, newviewid, viewmode);
                            }
                        }); // ajax  

                    } // onAddView()
                }); // CswDialog
            }; // saveAsView()

            cswPublic.restoreSearch = function (searchid) {

                cswPrivate.sessiondataid = searchid;

                Csw.tryExec(cswPrivate.onBeforeSearch);
                Csw.ajax.post({
                    url: cswPrivate.restoresearchurl,
                    data: {
                        SessionDataId: cswPrivate.sessiondataid
                    },
                    success: function (data) {
                        cswPrivate.handleResults(data);
                        Csw.tryExec(cswPrivate.onAfterNewSearch, cswPrivate.sessiondataid);
                    }
                });
            }; // restoreSearch()

            cswPublic.getFilterToNodeTypeId = function() {
                var ret = '';
                function findFilterToNodeTypeId(thisFilter) {
                    if(Csw.isNullOrEmpty(ret) && thisFilter.filtername == 'Filter To') {
                        ret = thisFilter.firstversionid;
                    }
                } // findFilterToNodeTypeId()
                Csw.each(cswPrivate.data.filtersapplied, findFilterToNodeTypeId);
                return ret;
            } // getFilterToNodeTypeId()

            return cswPublic;
        });

})();
