/// <reference path="~/app/CswApp-vsdoc.js" />
(function () {

    Csw.composites.register('universalSearch', function (cswParent, params) {
        'use strict';
        var cswPrivate = {
            name: 'newsearch',
            searchBoxParent: {},
            searchResultsParent: {},
            searchFiltersParent: {},
            nodetypeid: '',       // automatically filter results to this nodetype
            objectclassid: '',    // automatically filter results to this objectclass
            allowNodeTypeChange: true,  // allowed to change which nodetype is filtered
            onBeforeSearch: null,
            onAfterSearch: null,
            onAfterNewSearch: null,
            onLoadView: null,
            onAddView: null,
            onSuccess: null,
            searchbox_width: '200px',
            showSave: true,
            allowEdit: true,
            allowDelete: true,
            allowImport: false, //c3 addition
            extraAction: null,
            extraActionIcon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.none),
            onExtraAction: null,  // function(nodeObj) {}
            compactResults: false,   // Don't show full table layout unless expanded
            suppressButtons: false,  // Don't include buttons in results
            newsearchurl: 'doUniversalSearch',
            restoresearchurl: 'restoreUniversalSearch',
            sessiondataid: '',
            searchterm: '',
            searchtype: 'Begins',
            filterHideThreshold: 5,
            universalSearchOnly: false,
            filterOutNodeId: null,
            forceSingleColumn: false
            //buttonSingleColumn: '',
            //buttonMultiColumn: ''
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

            cswPrivate.onPreFilterClick = function (nodetypeObj) {
                if (false === Csw.isNullOrEmpty(nodetypeObj)) {
                    cswPrivate.preFilterSelect.setText('');
                    cswPrivate.preFilterSelect.setIcon(nodetypeObj.iconfilename);
                    cswPrivate.nodetypeid = nodetypeObj.id;
                } else {
                    cswPrivate.preFilterSelect.setText('All');
                    cswPrivate.preFilterSelect.setIcon('');
                    cswPrivate.nodetypeid = '';
                }
            }; // onFilterClick()

            // Nodetype Filter Menu
            cswPublic.ready = Csw.ajax.deprecatedWsNbt({
                urlMethod: 'getNodeTypes',
                data: {
                    PropertySetName: '',
                    ObjectClassName: '',
                    ObjectClassId: '',
                    ExcludeNodeTypeIds: '',
                    RelatedToNodeTypeId: '',
                    RelatedObjectClassPropName: '',
                    FilterToPermission: '',
                    RelationshipNodeTypePropId: '',
                    Searchable: true
                },
                success: function (data) {
                    var items = [];

                    items.push({
                        text: 'All',
                        icon: '',
                        handler: Csw.method(function () { cswPrivate.onPreFilterClick(null); })
                    });

                    var selectedText = 'All';
                    var selectedIcon = '';
                    Csw.each(data, function (nt) {
                        if (false === Csw.isNullOrEmpty(nt.name)) {
                            items.push({
                                text: nt.name,
                                icon: nt.iconfilename,
                                handler: Csw.method(function () { cswPrivate.onPreFilterClick(nt); })
                            });
                            if (cswPrivate.nodetypeid === nt.id) {
                                selectedText = '';
                                selectedIcon = nt.iconfilename;
                            }
                        }
                    });

                    cswPrivate.preFilterSelect = window.Ext.create('Ext.SplitButton', {
                        text: selectedText,
                        icon: selectedIcon,
                        width: 40,
                        renderTo: cswtable.cell(1, 1).getId(),
                        menu: {
                            items: items
                        }
                    }); // toolbar

                    if (false === cswPrivate.allowNodeTypeChange) {
                        cswPrivate.preFilterSelect.disable();
                    }

                    Csw.tryExec(cswPrivate.onSuccess);
                } // success
            }); // ajax

            // Search Menu
            Csw.ajaxWcf.post({
                urlMethod: 'Menus/getSearchMenuItems',
                data: cswPrivate.universalSearchOnly,
                success: function (data) {
                    var srchMenuItems = [];

                    srchMenuItems.push({
                        text: 'Search',
                        icon: Csw.getIconUrlString(16, Csw.enums.iconType.magglass),
                        handler: srchOnClick
                    });

                    var selectedText = 'Search';
                    var selectedIcon = '';
                    Csw.each(data.searchTargets, function (st) {
                        if (false === Csw.isNullOrEmpty(st.name)) {
                            srchMenuItems.push({
                                text: st.name,
                                icon: st.iconfilename,
                                handler: Csw.method(function () { return srchOnClick(st.name); })
                            });
                        }
                    });

                    cswPrivate.searchButton = window.Ext.create('Ext.SplitButton', {
                        text: selectedText,
                        icon: selectedIcon,
                        width: (selectedText.length * 8) + 16,
                        renderTo: cswtable.cell(1, 4).getId(),
                        handler: Csw.method(srchOnClick),
                        menu: {
                            items: srchMenuItems
                        }
                    }); // searchButton

                } // success
            }); // ajaxWcf

            var srchOnClick = function (selectedOption) {
                var ret = Csw.clientChanges.manuallyCheckChanges();
                if (ret) {
                    switch (selectedOption) {
                        case 'Structure Search':
                            $.CswDialog('StructureSearchDialog', { loadView: cswPrivate.onLoadView });
                            break;
                        case 'ChemCatCentral Search':
                            $.CswDialog('C3SearchDialog', {
                                loadView: cswPrivate.onLoadView,
                                c3searchterm: cswPrivate.searchinput.val(),
                                c3handleresults: cswPublic.handleResults,
                                clearview: cswPrivate.onBeforeSearch
                            });
                            break;
                        default:
                            Csw.publish('initPropertyTearDown');
                            cswPrivate.searchterm = cswPrivate.searchinput.val();
                            cswPrivate.newsearch();
                    }
                }
                return ret;
            };

            // Make search type selector
            cswPrivate.onSearchTypeSelect = function (val) {
                cswPrivate.searchtype = val;
                cswPrivate.searchTypeSelect.setText(val);
            };
            cswPrivate.searchTypeSelect = window.Ext.create('Ext.SplitButton', {
                text: cswPrivate.searchtype,
                renderTo: cswtable.cell(1, 2).getId(),
                width: 70,
                menu: {
                    items: [
                        { text: 'Begins', handler: Csw.method(function () { return cswPrivate.onSearchTypeSelect('Begins'); }) },
                        { text: 'Contains', handler: Csw.method(function () { return cswPrivate.onSearchTypeSelect('Contains'); }) },
                        { text: 'Ends', handler: Csw.method(function () { return cswPrivate.onSearchTypeSelect('Ends'); }) },
                        { text: 'Exact', handler: Csw.method(function () { return cswPrivate.onSearchTypeSelect('Exact'); }) }
                    ],
                }
            }); // toolbar

            cswPrivate.searchinput = cswtable.cell(1, 3).input({
                type: Csw.enums.inputTypes.search,
                value: cswPrivate.searchterm,
                width: cswPrivate.searchbox_width,
                cssclass: 'mousetrap',
                onKeyEnter: function () {
                    Csw.tryExec(srchOnClick, cswPrivate.searchButton.selectedOption);
                }
            });
        })();

        // Handle search submission
        cswPrivate.newsearch = function () {
            Csw.tryExec(cswPrivate.onBeforeSearch);

            Csw.ajax.deprecatedWsNbt({
                urlMethod: cswPrivate.newsearchurl,
                data: {
                    SearchTerm: cswPrivate.searchterm,
                    SearchType: cswPrivate.searchtype,
                    NodeTypeId: cswPrivate.nodetypeid,
                    ObjectClassId: cswPrivate.objectclassid,
                    Page: '0',
                    Limit: '0',
                },
                success: function (data) {
                    cswPublic.handleResults(data);
                    Csw.tryExec(cswPrivate.onAfterNewSearch, cswPrivate.sessiondataid);
                }
            });
        }; // search()

        cswPublic.handleResults = function (data) {
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

                var table2 = resultstable.cell(1, 1).table({
                    cellvalign: 'bottom'
                }).css({ 'padding-bottom': '5px' });

                //If user is performing a universal search, direct them to C3 search
                if (data.searchtarget == 'universal') {

                    table2.cell(1, 1).div({
                        cssclass: 'SearchLabel',
                        text: 'Search Results: (' + data.table.results + ')'
                    });

                    //If the C3 module is enabled
                    if (data.alternateoption) {

                        table2.cell(1, 2).div({
                            text: '&nbsp; &nbsp;'
                        });

                        table2.cell(1, 3).div({
                            cssclass: 'SearchC3Label',
                            text: 'Not the results you wanted? Try searching'
                        });

                        table2.cell(1, 4).div({
                            text: '&nbsp;'
                        });

                        //C3 icon
                        table2.cell(1, 5).img({
                            src: Csw.getIconUrlString(18, Csw.enums.iconType.cat)
                        });

                        table2.cell(1, 6).div({
                            text: '&nbsp;'
                        });

                        table2.cell(1, 7).a({
                            cssclass: 'SearchC3Label',
                            text: 'ChemCatCentral.',
                            onClick: function () {
                                $.CswDialog('C3SearchDialog', {
                                    loadView: cswPrivate.onLoadView,
                                    c3searchterm: cswPrivate.searchinput.val(),
                                    c3handleresults: cswPublic.handleResults,
                                    clearview: cswPrivate.onBeforeSearch
                                });
                            }
                        });

                    } // if (data.alternateoption != null)

                } //if (data.searchtarget == 'universal')
                else {
                    var table3 = table2.cell(1, 1).table({
                        cellvalign: 'bottom'
                    }).css({ 'padding-bottom': '5px' });

                    //C3 icon
                    table3.cell(1, 1).img({
                        src: Csw.getIconUrlString(18, Csw.enums.iconType.cat)
                    });

                    table3.cell(1, 2).div({
                        text: '&nbsp;'
                    });

                    table3.cell(1, 3).div({
                        cssclass: 'SearchLabel',
                        text: 'ChemCatCentral Search Results: (' + data.table.results + ')'
                    });
                }

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
                    cssclass: 'SearchTable',
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
                    chemCatConfig: {
                        importMenuItems: data.table.importmenu || []
                    },
                    //columns: columns,
                    allowEdit: cswPrivate.allowEdit,
                    allowDelete: cswPrivate.allowEdit,
                    extraAction: cswPrivate.extraAction,
                    extraActionIcon: cswPrivate.extraActionIcon,
                    onExtraAction: cswPrivate.onExtraAction,
                    compactResults: cswPrivate.compactResults,
                    forceSingleColumn: cswPrivate.forceSingleColumn,
                    suppressButtons: cswPrivate.suppressButtons,
                    filterOutNodeId: cswPrivate.filterOutNodeId,
                    onMoreClick: function (nodetypeid, nodetypename) {
                        // a little bit of a kludge
                        cswPrivate.filterNodeType(nodetypeid);
                    }
                });
            }

            _renderResultsTable(); //1);

            // Filter panel
            cswPrivate.searchFiltersParent.empty();

            fdiv = cswPrivate.searchFiltersParent.div();
            fdiv.addClass('SearchFilterDiv');

            fdiv.span({ text: data.name }).br();
            //fdiv.span({ text: 'Searched For: ' + data.searchterm }).br();
            ftable = fdiv.table({});

            // Filters in use
            var hasFilters = false;
            var atLeastOneShown = false;
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

            if (hasFilters && cswPrivate.showSave) {
                fdiv.br();
                var btntbl = fdiv.table();
                btntbl.cell(1, 1).buttonExt({
                    enabledText: 'Save',
                    disableOnClick: false,
                    icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.save),
                    onClick: cswPrivate.save
                });
                if (false === Csw.isNullOrEmpty(data.searchid)) {
                    btntbl.cell(1, 2).buttonExt({
                        enabledText: 'Delete',
                        disableOnClick: false,
                        icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.trash),
                        onClick: function () {
                            cswPrivate.deleteSave(data.searchid);
                        } // onClick
                    });
                }
            }
            fdiv.br();
            fdiv.br();

            // Filters to add
            var filtersDiv = fdiv.moreDiv({
                moretext: 'More Filters...',
                lesstext: ''
            });
            filtersDiv.moreLink.hide();

            function makeFilterSet(thisFilterSet) {

                var filterCount = 0;
                var destDiv, moreDiv, thisdiv, filterName = '', filterSource, nameSpan;

                Csw.each(thisFilterSet, function (thisFilter) {
                    if (filterName === '') {
                        filterName = thisFilter.filtername;
                        filterSource = thisFilter.source;
                        if (filterSource === 'Results') {
                            destDiv = filtersDiv.hiddenDiv;
                            filtersDiv.moreLink.show();
                        } else {
                            destDiv = filtersDiv.shownDiv;
                            atLeastOneShown = true;
                        }
                        moreDiv = destDiv.moreDiv();
                        moreDiv.moreLink.hide();

                        nameSpan = moreDiv.shownDiv.span({}).css({ fontWeight: 'bold' });
                        moreDiv.shownDiv.br();
                        thisdiv = moreDiv.shownDiv;
                    }
                    if (Csw.bool(thisFilter.usemorelink) && filterCount === cswPrivate.filterHideThreshold) {
                        moreDiv.moreLink.show();
                        thisdiv = moreDiv.hiddenDiv;
                    }

                    thisdiv.a({
                        text: thisFilter.filtervalue + ' (' + thisFilter.count + ')',
                        onClick: function () {
                            cswPrivate.filter(thisFilter, 'add');
                            return false;
                        }
                    });
                    thisdiv.br();

                    filterCount++;
                });
                if (false === Csw.isNullOrEmpty(filterName)) {
                    nameSpan.text(filterName);
                    destDiv.br();
                    destDiv.br();
                }
            } // makeFilterSet()

            Csw.each(data.filters, makeFilterSet);
            if (false === atLeastOneShown) {
                filtersDiv.showHidden();
            }

            cswPrivate.data = data;

            Csw.tryExec(cswPrivate.onAfterSearch, cswPublic);
        }; // handleResults()


        cswPrivate.filter = function (thisFilter, action) {
            Csw.tryExec(cswPrivate.onBeforeSearch);
            Csw.ajax.deprecatedWsNbt({
                urlMethod: 'filterUniversalSearch',
                data: {
                    SessionDataId: cswPrivate.sessiondataid,
                    Filter: JSON.stringify(thisFilter),
                    Action: action
                },
                success: cswPublic.handleResults
            });
        }; // filter()

        cswPrivate.filterNodeType = function (nodetypeid) {
            Csw.tryExec(cswPrivate.onBeforeSearch);
            Csw.ajax.deprecatedWsNbt({
                urlMethod: 'filterUniversalSearchByNodeType',
                data: {
                    SessionDataId: cswPrivate.sessiondataid,
                    NodeTypeId: nodetypeid
                },
                success: cswPublic.handleResults
            });
        }; // filter()

        cswPrivate.save = function () {
            $.CswDialog('SaveSearchDialog', {
                name: cswPrivate.data.name,
                category: cswPrivate.data.category || 'Saved Searches',
                onOk: function (name, category) {

                    Csw.ajax.deprecatedWsNbt({
                        urlMethod: 'saveSearch',
                        data: {
                            SessionDataId: cswPrivate.sessiondataid,
                            Name: name,
                            Category: category
                        },
                        success: function (data) {
                            cswPublic.handleResults(data);
                        } // success
                    }); // ajax  

                } // onAddView()
            }); // CswDialog
        }; // save()

        cswPrivate.deleteSave = function (searchid) {
            Csw.ajax.deprecatedWsNbt({
                urlMethod: 'deleteSearch',
                data: {
                    SearchId: searchid
                },
                success: function (data) {
                    cswPublic.handleResults(data);
                } // success
            }); // ajax  
        }; // save()

        cswPublic.restoreSearch = function (searchid) {

            cswPrivate.sessiondataid = searchid;

            Csw.tryExec(cswPrivate.onBeforeSearch);
            Csw.ajax.deprecatedWsNbt({
                urlMethod: cswPrivate.restoresearchurl,
                data: {
                    SessionDataId: cswPrivate.sessiondataid
                },
                success: function (data) {
                    cswPublic.handleResults(data);
                    Csw.tryExec(cswPrivate.onAfterNewSearch, cswPrivate.sessiondataid);

                    cswPrivate.searchinput.val(data.searchterm);
                    cswPrivate.onSearchTypeSelect(data.searchtype);
                    Csw.each(data.filtersapplied, function(filter) {
                        if (filter.filtertype == "nodetype") {
                            cswPrivate.onPreFilterClick({ id: filter.firstversionid, iconfilename: "Images/newicons/16/" + filter.icon });
                        }
                    });
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

        cswPublic.disable = function () {
            if (cswPrivate.preFilterSelect) {
                cswPrivate.preFilterSelect.disable();
            }
            if (cswPrivate.searchButton) {
                cswPrivate.searchButton.disable();
            }
            cswPrivate.searchinput.disable();
            cswPrivate.searchTypeSelect.disable();
        };

        cswPublic.enable = function () {
            if (cswPrivate.preFilterSelect) {
                cswPrivate.preFilterSelect.enable();
            }
            if (cswPrivate.searchButton) {
                cswPrivate.searchButton.enable();
            }
            cswPrivate.searchinput.enable();
            cswPrivate.searchTypeSelect.enable();
        };

        cswPublic.getSearchTerm = function() {
            return cswPrivate.searchterm;
        };
        
        return cswPublic;
    });
})();
