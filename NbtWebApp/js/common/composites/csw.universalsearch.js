/// <reference path="~/js/CswCommon-vsdoc.js" />
/// <reference path="~/js/CswNbt-vsdoc.js" />

(function () {


    Csw.composites.universalSearch = Csw.composites.universalSearch ||
        Csw.composites.register('universalSearch', function (cswParent, params) {
            'use strict';
            var cswPrivateVar = {
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
                onExtraAction: null,  // function(nodeObj) {}

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
                $.extend(cswPrivateVar, params);
            }
            var cswPublicRet = {};

            // Constructor
            // Adds a searchbox to the form
            (function () {
                
                var cswtable = Csw.literals.table({
                    ID: Csw.makeId(cswPrivateVar.ID, '', '_div'),
                    $parent: cswPrivateVar.$searchbox_parent
                });

                cswPrivateVar.searchinput = cswtable.cell(1, 1).input({
                    ID: Csw.makeId(cswPrivateVar.ID, '', '_input'),
                    type: Csw.enums.inputTypes.text,
                    width: cswPrivateVar.searchbox_width
                });

                cswPrivateVar.searchbutton = cswtable.cell(1, 2).button({
                    ID: Csw.makeId(cswPrivateVar.ID, '', '_srchbtn'),
                    enabledText: 'Search',
                    disabledText: 'Searching...',
                    onClick: function () {
                        cswPrivateVar.searchterm = cswPrivateVar.searchinput.val();
                        //cswPrivateVar.filters = {};
                        cswPrivateVar.newsearch();
                    }
                });

                cswPrivateVar.searchinput.clickOnEnter(cswPrivateVar.searchbutton);
            })();

            // Handle search submission
            cswPrivateVar.newsearch = function () {
                Csw.tryExec(cswPrivateVar.onBeforeSearch);

                Csw.ajax.post({
                    url: cswPrivateVar.newsearchurl,
                    data: { 
                        SearchTerm: cswPrivateVar.searchterm,
                        NodeTypeId: cswPrivateVar.nodetypeid,
                        ObjectClassId: cswPrivateVar.objectclassid
                    },
                    success: function (data) {
                        cswPrivateVar.handleResults(data);
                        Csw.tryExec(cswPrivateVar.onAfterNewSearch, cswPrivateVar.sessiondataid);
                    }
                });
            }; // search()

            cswPrivateVar.handleResults = function (data) {
                var fdiv, ftable, filtersdivid;

                cswPrivateVar.sessiondataid = data.sessiondataid;

                // Search results

                function _renderResultsTable(columns) {
                    
                    cswPrivateVar.$searchresults_parent.contents().remove();
                    cswPrivateVar.$searchresults_parent.css({ paddingTop: '15px' });

                    var resultstable = Csw.literals.table({
                        ID: Csw.makeId(cswPrivateVar.ID, '', 'resultstbl'),
                        $parent: cswPrivateVar.$searchresults_parent,
                        width: '100%'
                    });

                    resultstable.cell(1, 1).append('<b>Search Results: (' + data.table.results + ')</b>');

                    resultstable.cell(1, 2).css({ width: '18px' });
                    cswPrivateVar.buttonSingleColumn = resultstable.cell(1, 2).imageButton({
                        ID: Csw.makeId(cswPrivateVar.ID, '', '_singlecol'),
                        ButtonType: Csw.enums.imageButton_ButtonType.TableSingleColumn,
                        Active: (columns === 1),
                        AlternateText: 'Single Column',
                        onClick: function () {
                            setTimeout(function () { // so we see the clear immediately
                                _renderResultsTable(1);
                            }, 0);
                        }
                    });

                    resultstable.cell(1, 3).css({ width: '18px' });
                    cswPrivateVar.buttonMultiColumn = resultstable.cell(1, 3).imageButton({
                        ID: Csw.makeId(cswPrivateVar.ID, '', '_multicol'),
                        ButtonType: Csw.enums.imageButton_ButtonType.TableMultiColumn,
                        Active: (columns !== 1),
                        AlternateText: 'Multi Column',
                        onClick: function () {
                            setTimeout(function () { // so we see the clear immediately
                                _renderResultsTable(3);
                            }, 0);
                        }
                    });

                    resultstable.cell(2, 1).propDom({ 'colspan': 3 });

                    resultstable.cell(2, 1).$.CswNodeTable({
                        ID: Csw.makeId(cswPrivateVar.ID, '', 'srchresults'),
                        onEditNode: null,
                        onDeleteNode: function () {
                            // case 25380 - refresh on delete
                            cswPublicRet.restoreSearch(cswPrivateVar.sessiondataid);
                        },
                        //onSuccess: cswPrivateVar.onAfterSearch,
                        onNoResults: function () {
                            resultstable.cell(2, 1).text('No Results Found');
                        },
                        tabledata: data.table,
                        //maxheight: cswPrivateVar.searchresults_maxheight
                        columns: columns,
                        allowEdit: cswPrivateVar.allowEdit,
                        allowDelete: cswPrivateVar.allowEdit,
                        extraAction: cswPrivateVar.extraAction,
                        onExtraAction: cswPrivateVar.onExtraAction
                    });
                }

                _renderResultsTable(1);

                // Filter panel
                cswPrivateVar.$searchfilters_parent.contents().remove();

                filtersdivid = Csw.makeId(cswPrivateVar.ID, '', 'filtersdiv');
                fdiv = Csw.literals.div({
                    ID: filtersdivid,
                    $parent: cswPrivateVar.$searchfilters_parent
                }).css({
                    paddingTop: '15px'
                    //height: cswPrivateVar.searchresults_maxheight + 'px',
                    //overflow: 'auto'
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
                        text: thisFilter.filtervalue
                    });
                    if (Csw.bool(thisFilter.removeable)) {
                        ftable.cell(ftable_row, 3).imageButton({
                            ID: Csw.makeId(filtersdivid, '', thisFilter.filterid),
                            ButtonType: Csw.enums.imageButton_ButtonType.Delete,
                            AlternateText: 'Remove Filter',
                            onClick: function () {
                                cswPrivateVar.filter(thisFilter, 'remove');
                            }
                        });
                    }
                    ftable_row++;
                    hasFilters = true;
                }

                Csw.each(data.filtersapplied, showFilter);

                if (hasFilters && cswPrivateVar.showSaveAsView) {
                    fdiv.br();
                    fdiv.button({
                        ID: Csw.makeId(filtersdivid, '', "saveview"),
                        enabledText: 'Save as View',
                        disableOnClick: false,
                        onClick: cswPrivateVar.saveAsView
                    });
                }
                fdiv.br();
                fdiv.br();

                // Filters to add

                function makeFilterLink(thisFilter, div, filterCount) {
                    var flink = div.a({
                        ID: Csw.makeId(filtersdivid, '', thisFilter.filterid),
                        text: thisFilter.filtervalue + ' (' + thisFilter.count + ')',
                        onClick: function () {
                            cswPrivateVar.filter(thisFilter, 'add');
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
                    Csw.each(thisFilterSet, function (thisFilter) {
                        if (filterCount === cswPrivateVar.filterHideThreshold) {
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

                Csw.tryExec(cswPrivateVar.onAfterSearch);
            } // handleResults()


            cswPrivateVar.filter = function (thisFilter, action) {
                //cswPrivateVar.filters[thisFilter.filterid] = thisFilter;

                Csw.tryExec(cswPrivateVar.onBeforeSearch);
                Csw.ajax.post({
                    url: cswPrivateVar.filtersearchurl,
                    data: {
                        SessionDataId: cswPrivateVar.sessiondataid,
                        Filter: JSON.stringify(thisFilter),
                        Action: action
                    },
                    success: cswPrivateVar.handleResults
                });
            }; // filter()

            cswPrivateVar.saveAsView = function () {
                $.CswDialog('AddViewDialog', {
                    ID: Csw.makeId(cswPrivateVar.ID, '', 'addviewdialog'),
                    //viewmode: 'table',
                category: 'Saved Searches',
                    onAddView: function (newviewid, viewmode) {

                        Csw.ajax.post({
                            url: cswPrivateVar.saveurl,
                            data: {
                                SessionDataId: cswPrivateVar.sessiondataid,
                                ViewId: newviewid
                            },
                            success: function (data) {
                            Csw.tryExec(cswPrivateVar.onAddView, newviewid, viewmode);
                                Csw.tryExec(cswPrivateVar.onLoadView, newviewid, viewmode);
                            }
                        }); // ajax  

                    } // onAddView()
                }); // CswDialog
            }; // saveAsView()

            cswPublicRet.restoreSearch = function (searchid) {

                cswPrivateVar.sessiondataid = searchid;

                Csw.tryExec(cswPrivateVar.onBeforeSearch);
                Csw.ajax.post({
                    url: cswPrivateVar.restoresearchurl,
                    data: {
                        SessionDataId: cswPrivateVar.sessiondataid
                    },
                    success: function (data) {
                        cswPrivateVar.handleResults(data);
                        Csw.tryExec(cswPrivateVar.onAfterNewSearch, cswPrivateVar.sessiondataid);
                    }
                });
            }; // restoreSearch()

            return cswPublicRet;
        });

})();
