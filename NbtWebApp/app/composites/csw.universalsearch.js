/// <reference path="~/app/CswApp-vsdoc.js" />
(function () {
    Csw.composites.universalSearch = Csw.composites.universalSearch ||
        Csw.composites.register('universalSearch', function (cswParent, params) {
            'use strict';
            var cswPrivate = {
                name: 'newsearch',
                searchBoxParent: {},
                searchResultsParent: {},
                searchFiltersParent: {},
                nodetypeid: '',       // automatically filter results to this nodetype
                objectclassid: '',    // automatically filter results to this objectclass
                onBeforeSearch: null,
                onAfterSearch: null,
                onAfterNewSearch: null,
                onLoadView: null,
                onAddView: null,
                searchbox_width: '200px',
                showSaveAsView: true,
                allowEdit: true,
                allowDelete: true,
                extraAction: null,
                extraActionIcon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.none),
                onExtraAction: null,  // function(nodeObj) {}
                compactResults: false,
                newsearchurl: 'doUniversalSearch',
                restoresearchurl: 'restoreUniversalSearch',
                saveurl: 'saveSearchAsView',
                sessiondataid: '',
                searchterm: '',
                filterHideThreshold: 5,
                buttonSingleColumn: '',
                buttonMultiColumn: ''
            };
            Csw.extend(cswPrivate, params);

            if (false === Csw.isNullOrEmpty(cswParent)) {
                cswPrivate.table = cswParent.table({
                    cellpadding: '2px'
                });

                var cell11 = cswPrivate.table.cell(1, 1);

                cell11.propDom('colspan', 2);

                cswPrivate.searchBoxParent = cell11.div({ name: 'searchdialog_searchdiv' });
                cswPrivate.searchResultsParent = cswPrivate.table.cell(2, 2).div({ name: 'searchdialog_resultsdiv' });
                cswPrivate.searchFiltersParent = cswPrivate.table.cell(2, 1).div({ name: 'searchdialog_filtersdiv' });
            }
            var cswPublic = {};

            // Constructor
            // Adds a searchbox to the form
            (function () {
                cswPrivate.searchBoxParent.empty();
                var cswtable = cswPrivate.searchBoxParent.table();

                var onPreFilterClick = function (objectclass) {
                    if (false === Csw.isNullOrEmpty(objectclass)) {
                        cswPrivate.preFilterSelect.setText('');
                        cswPrivate.preFilterSelect.setIcon(objectclass.iconfilename);
                    } else {
                        cswPrivate.preFilterSelect.setText('All');
                        cswPrivate.preFilterSelect.setIcon('');
                    }
                }; // onFilterClick()

                Csw.ajax.post({
                    urlMethod: 'getObjectClasses',
                    success: function (data) {
                        var objclassItems = [];

                        objclassItems.push({
                            text: 'All',
                            icon: '',
                            handler: function () { onPreFilterClick(null); }
                        });

                        Csw.each(data, function (oc) {
                            objclassItems.push({
                                text: oc.objectclass,
                                icon: oc.iconfilename,
                                handler: function () { onPreFilterClick(oc); }
                            });
                        });

                        cswPrivate.preFilterSelect = window.Ext.create('Ext.SplitButton', {
                            text: 'All',
                            renderTo: cswtable.cell(1, 1).getId(),
                            menu: {
                                items: objclassItems
                            }
                        }); // toolbar

                    } // success
                }); // ajax

                cswPrivate.searchinput = cswtable.cell(1, 2).input({
                    type: Csw.enums.inputTypes.search,
                    width: cswPrivate.searchbox_width,
                    cssclass: 'mousetrap'
                });

                cswPrivate.searchbutton = cswtable.cell(1, 3)
                    .div()
                    .buttonExt({
                        icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.search),
                        width: ('Search'.length * 11) + 16,
                        enabledText: 'Search',
                        disabledText: 'Searching...',
                        bindOnEnter: true,
                        onClick: function () {
                            Csw.publish('initPropertyTearDown');
                            cswPrivate.searchterm = cswPrivate.searchinput.val();
                            cswPrivate.newsearch();
                        }
                    });
            })();

            // Handle search submission
            cswPrivate.newsearch = function () {
                Csw.tryExec(cswPrivate.onBeforeSearch);

                Csw.ajax.post({
                    urlMethod: cswPrivate.newsearchurl,
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

            cswPrivate.handleResults = function (data) {
                var fdiv, ftable;

                cswPrivate.sessiondataid = data.sessiondataid;

                // Search results

                function _renderResultsTable() { //columns) {
                    var nodeTable;

                    cswPrivate.searchResultsParent.empty();
                    cswPrivate.searchResultsParent.css({ paddingTop: '15px' });

                    var resultstable = cswPrivate.searchResultsParent.table({
                        width: '100%'
                    });

                    resultstable.cell(1, 1).div({
                        cssclass: 'SearchLabel',
                        text: 'Search Results: (' + data.table.results + ')'
                    });

                    if (Csw.bool(cswPrivate.compactResults)) {
                        resultstable.cell(1, 2).css({ width: '100px' });
                        cswPrivate.linkExpandAll = resultstable.cell(1, 2).a({
                            text: 'Expand All',
                            onClick: function () {
                                if (cswPrivate.linkExpandAll.text() === 'Expand All') {
                                    cswPrivate.linkExpandAll.text('Collapse All');
                                }
                                else if (cswPrivate.linkExpandAll.text() === 'Collapse All') {
                                    cswPrivate.linkExpandAll.text('Expand All');
                                }
                                nodeTable.expandAll();
                            }
                        });
                    }

                    resultstable.cell(2, 1).propDom({ 'colspan': 3 });

                    nodeTable = Csw.nbt.nodeTable(resultstable.cell(2, 1), {
                        onEditNode: function () {
                            // case 27245 - refresh on edit
                            cswPublic.restoreSearch(cswPrivate.sessiondataid);
                        },
                        onDeleteNode: function () {
                            // case 25380 - refresh on delete
                            cswPublic.restoreSearch(cswPrivate.sessiondataid);
                        },
                        onNoResults: function () {
                            resultstable.cell(2, 1).text('No Results Found');
                        },
                        tabledata: data.table,
                        //columns: columns,
                        allowEdit: cswPrivate.allowEdit,
                        allowDelete: cswPrivate.allowEdit,
                        extraAction: cswPrivate.extraAction,
                        extraActionIcon: cswPrivate.extraActionIcon,
                        onExtraAction: cswPrivate.onExtraAction,
                        compactResults: cswPrivate.compactResults,
                        onMoreClick: function (nodetypeid, nodetypename) {
                            // a little bit of a kludge
                            cswPrivate.filterNodeType(nodetypeid);
                        }
                    });
                }

                _renderResultsTable(); //1);

                // Filter panel
                cswPrivate.searchFiltersParent.empty();

                fdiv = cswPrivate.searchFiltersParent.div().css({
                    paddingTop: '15px'
                });

                fdiv.span({ text: 'Searched For: ' + data.searchterm }).br();
                ftable = fdiv.table({});

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
                        text: thisFilter.filtervalue + '&nbsp;&nbsp;'
                    });
                    if (Csw.bool(thisFilter.removeable)) {
                        ftable.cell(ftable_row, 3).icon({
                            iconType: Csw.enums.iconType.x,
                            hovertext: 'Remove Filter',
                            size: 16,
                            isButton: true,
                            onClick: function () {
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
                        enabledText: 'Save as View',
                        disableOnClick: false,
                        icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.save),
                        onClick: cswPrivate.saveAsView
                    });
                }
                fdiv.br();
                fdiv.br();

                // Filters to add

                function makeFilterLink(thisFilter, div, filterCount) {
                    var flink = div.a({
                        text: thisFilter.filtervalue + ' (' + thisFilter.count + ')',
                        onClick: function () {
                            cswPrivate.filter(thisFilter, 'add');
                            return false;
                        }
                    });
                    div.br();
                } // makeFilterLink()

                function makeFilterSet(thisFilterSet) {

                    var filterCount = 0;
                    var moreDiv = fdiv.moreDiv();
                    var filterName = '';

                    var nameSpan = moreDiv.shownDiv.span({}).css({ fontWeight: 'bold' });
                    moreDiv.shownDiv.br();
                    var thisdiv = moreDiv.shownDiv;
                    moreDiv.moreLink.hide();
                    Csw.each(thisFilterSet, function (thisFilter) {
                        if (filterName === '') {
                            filterName = thisFilter.filtername;
                        }
                        if (filterCount === cswPrivate.filterHideThreshold) {
                            moreDiv.moreLink.show();
                            thisdiv = moreDiv.hiddenDiv;
                        }
                        makeFilterLink(thisFilter, thisdiv, filterCount);
                        filterCount++;
                    });
                    nameSpan.text(filterName);
                    fdiv.br();
                    fdiv.br();
                } // makeFilterSet()

                Csw.each(data.filters, makeFilterSet);

                cswPrivate.data = data;

                Csw.tryExec(cswPrivate.onAfterSearch, cswPublic);
            }; // handleResults()


            cswPrivate.filter = function (thisFilter, action) {
                Csw.tryExec(cswPrivate.onBeforeSearch);
                Csw.ajax.post({
                    urlMethod: 'filterUniversalSearch',
                    data: {
                        SessionDataId: cswPrivate.sessiondataid,
                        Filter: JSON.stringify(thisFilter),
                        Action: action
                    },
                    success: cswPrivate.handleResults
                });
            }; // filter()

            cswPrivate.filterNodeType = function (nodetypeid) {
                Csw.tryExec(cswPrivate.onBeforeSearch);
                Csw.ajax.post({
                    urlMethod: 'filterUniversalSearchByNodeType',
                    data: {
                        SessionDataId: cswPrivate.sessiondataid,
                        NodeTypeId: nodetypeid
                    },
                    success: cswPrivate.handleResults
                });
            }; // filter()

            cswPrivate.saveAsView = function () {
                $.CswDialog('AddViewDialog', {
                    category: 'Saved Searches',
                    onAddView: function (newviewid, viewmode) {

                        Csw.ajax.post({
                            urlMethod: cswPrivate.saveurl,
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
                    urlMethod: cswPrivate.restoresearchurl,
                    data: {
                        SessionDataId: cswPrivate.sessiondataid
                    },
                    success: function (data) {
                        cswPrivate.handleResults(data);
                        Csw.tryExec(cswPrivate.onAfterNewSearch, cswPrivate.sessiondataid);
                    }
                });
            }; // restoreSearch()

            cswPublic.getFilterToNodeTypeId = function () {
                var ret = '';

                function findFilterToNodeTypeId(thisFilter) {
                    if (Csw.isNullOrEmpty(ret) && thisFilter.filtername == 'Filter To') {
                        ret = thisFilter.firstversionid;
                    }
                } // findFilterToNodeTypeId()

                Csw.each(cswPrivate.data.filtersapplied, findFilterToNodeTypeId);
                return ret;
            }; // getFilterToNodeTypeId()

            return cswPublic;
        });
})();
