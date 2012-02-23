/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function (options) {
    "use strict";

    function universalSearch(params) {
        var internal = {
            ID: 'newsearch',
            $searchbox_parent: null,
            $searchresults_parent: null,
            $searchfilters_parent: null,
            onBeforeSearch: null,
            onAfterSearch: null,
            onLoadView: null,
            searchresults_maxheight: '600',
            searchbox_width: '200px',

            searchurl: '/NbtWebApp/wsNBT.asmx/doUniversalSearch',
            saveurl: '/NbtWebApp/wsNBT.asmx/saveSearchAsView',
            filters: {},
            searchterm: '',
            filterHideThreshold: 5
        };
        if (params) $.extend(internal, params);

        var external = {};

        // Constructor
        // Adds a searchbox to the form
        (function () {
            var cswtable = Csw.controls.table({
                ID: Csw.controls.dom.makeId(internal.ID, '', '_div'),
                $parent: internal.$searchbox_parent
            });

            internal.searchinput = cswtable.cell(1, 1).input({
                ID: Csw.controls.dom.makeId(internal.ID, '', '_input'),
                type: Csw.enums.inputTypes.text,
                width: internal.searchbox_width
            });

            internal.searchbutton = cswtable.cell(1, 2).button({
                ID: Csw.controls.dom.makeId(internal.ID, '', '_srchbtn'),
                enabledText: 'Search',
                disabledText: 'Searching...',
                onClick: function () {
                    internal.searchterm = internal.searchinput.val();
                    internal.filters = {};
                    internal.search();
                }
            });

            internal.searchinput.clickOnEnter(internal.searchbutton);
        })();

        // Handle search submission
        internal.search = function () {
            Csw.tryExec(internal.onBeforeSearch);

            Csw.ajax.post({
                url: internal.searchurl,
                data: {
                    SearchTerm: internal.searchterm,
                    Filters: JSON.stringify(internal.filters)
                },
                success: function (data) {
                    var fdiv, ftable, filtersdivid;

                    // Search results
                    internal.$searchresults_parent
                        .css({ paddingTop: '15px' })
                        .append('<b>Search Results: (' + data.table.results + ')</b>');

                    internal.$searchresults_parent.CswNodeTable({
                        ID: Csw.controls.dom.makeId(internal.ID, '', 'srchresults'),
                        onEditNode: null,
                        onDeleteNode: null,
                        onSuccess: internal.onAfterSearch,
                        onNoResults: function () {
                            internal.$searchresults_parent.text('No Results Found');
                        },
                        tabledata: data.table,
                        maxheight: internal.searchresults_maxheight
                    });

                    // Filter panel
                    filtersdivid = Csw.controls.dom.makeId(internal.ID, '', 'filtersdiv');
                    fdiv = Csw.controls.div({
                        ID: filtersdivid,
                        $parent: internal.$searchfilters_parent
                    }).css({
                        paddingTop: '15px',
                        height: internal.searchresults_maxheight + 'px',
                        overflow: 'auto'
                    });

                    fdiv.span({ text: 'Searched For: ' + internal.searchterm }).br();
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
                        ftable.cell(ftable_row, 3).imageButton({
                            ID: Csw.controls.dom.makeId(filtersdivid, '', thisFilter.filterid),
                            ButtonType: Csw.enums.imageButton_ButtonType.Delete,
                            AlternateText: 'Remove Filter',
                            onClick: function () {
                                internal.removeFilter(thisFilter);
                                return Csw.enums.imageButton_ButtonType.None;
                            }
                        });
                        ftable_row++;
                        hasFilters = true;
                    }
                    Csw.each(internal.filters, showFilter);

                    if (hasFilters) {
                        fdiv.br();
                        fdiv.button({
                            ID: Csw.controls.dom.makeId(filtersdivid, '', "saveview"),
                            enabledText: 'Save as View',
                            disableOnClick: false,
                            onClick: internal.saveAsView
                        });
                    }
                    fdiv.br();
                    fdiv.br();

                    // Filters to add
                    function makeFilterLink(thisFilter, div, filterCount) {
                        var flink = div.link({
                            ID: Csw.controls.dom.makeId(filtersdivid, '', thisFilter.filterid),
                            text: thisFilter.filtervalue + ' (' + thisFilter.count + ')',
                            onClick: function () {
                                internal.addFilter(thisFilter);
                                return false;
                            }
                        });
                        div.br();
                    }
                    function makeFilterSet(thisFilterSet, Name) {

                        var thisfilterdivid = Csw.controls.dom.makeId(filtersdivid, '', Name);
                        var thisfilterdiv = fdiv.div({ ID: thisfilterdivid });
                        var filterCount = 0;
                        var hiddenfiltersdiv;

                        thisfilterdiv.append('<b>' + Name + ':</b>');
                        thisfilterdiv.br();
                        Csw.each(thisFilterSet, function (thisFilter) {

                            if (filterCount === internal.filterHideThreshold) {

                                hiddenfiltersdiv = thisfilterdiv.div({ ID: Csw.controls.dom.makeId(thisfilterdivid, '', 'hidediv') });
                                hiddenfiltersdiv.hide();

                                var morelink = thisfilterdiv.link({
                                    ID: Csw.controls.dom.makeId(thisfilterdivid, '', 'more'),
                                    text: 'more...',
                                    cssclass: 'filtermorelink',
                                    onClick: function () {
                                        if (morelink.toggleState === Csw.enums.toggleState.on) {
                                            morelink.text('less...');
                                            hiddenfiltersdiv.show();
                                        } else {
                                            morelink.text('more...');
                                            hiddenfiltersdiv.hide();
                                        }
                                        return false;
                                    } // onClick()
                                }); // link()

                            } //  if (filterCount === internal.filterHideThreshold) {

                            if (filterCount >= internal.filterHideThreshold) {
                                makeFilterLink(thisFilter, hiddenfiltersdiv, filterCount);
                            } else {
                                makeFilterLink(thisFilter, thisfilterdiv, filterCount);
                            }
                            filterCount++;
                        });
                        thisfilterdiv.br();
                        thisfilterdiv.br();
                    }

                    Csw.each(data.filters, makeFilterSet);

                    Csw.tryExec(internal.onAfterSearch);
                } // success
            }); // ajax
        } // search()


        internal.addFilter = function (thisFilter) {
            internal.filters[thisFilter.filterid] = thisFilter;
            internal.search();
        } // addFilter()

        internal.removeFilter = function (thisFilter) {
            if (thisFilter.filtertype === "nodetype") {
                //remove all filters
                internal.filters = {};
            } else {
                delete internal.filters[thisFilter.filterid];
            }
            internal.search();
        } // removeFilter()

        internal.saveAsView = function () {
            $.CswDialog('AddViewDialog', {
                ID: Csw.controls.dom.makeId(internal.ID, '', 'addviewdialog'),
                viewmode: 'table',
                onAddView: function (newviewid, viewmode) {

                    Csw.ajax.post({
                        url: internal.saveurl,
                        data: {
                            ViewId: newviewid,
                            SearchTerm: internal.searchterm,
                            Filters: JSON.stringify(internal.filters)
                        },
                        success: function (data) {
                            Csw.tryExec(internal.onLoadView, newviewid, viewmode);
                        }
                    }); // ajax  

                } // onAddView()
            }); // CswDialog
        } // saveAsView()

        return external;
    };

    Csw.controls.register('universalSearch', universalSearch);
    Csw.controls.universalSearch = Csw.controls.universalSearch || universalSearch;
})();
