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
            width: '100px',
            align: 'right',
            onBeforeSearch: null,
            onAfterSearch: null,
            maxheight: '600',
            searchurl: '/NbtWebApp/wsNBT.asmx/doUniversalSearch',
            searchterm: '',
            filters: {}
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
                width: internal.width
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
                    var fdiv, filtersdivid;

                    // Search results
                    internal.$searchresults_parent.CswNodeTable({
                        ID: Csw.controls.dom.makeId(internal.ID, '', 'srchresults'),
                        onEditNode: null,
                        onDeleteNode: null,
                        onSuccess: internal.onAfterSearch,
                        onNoResults: function () {
                            internal.$searchresults_parent.text('No Results Found');
                        },
                        tabledata: data.table,
                        maxheight: internal.maxheight
                    });

                    // Filters
                    filtersdivid = Csw.controls.dom.makeId(internal.ID, '', 'filtersdiv');
                    fdiv = Csw.controls.div({
                        ID: filtersdivid,
                        $parent: internal.$searchfilters_parent
                    }).css({
                        height: internal.maxheight + 'px',
                        overflow: 'auto'
                    });

                    function makeFilterLink(thisFilter) {
                        fdiv.link({
                            ID: Csw.controls.dom.makeId(filtersdivid, '', thisFilter.id),
                            text: thisFilter.name + ' (' + thisFilter.count + ')',
                            onClick: function () {
                                internal.addFilter(thisFilter);
                                return false;
                            }
                        }).br();
                    }
                    function makeFilterSet(thisFilterSet, Name) {
                        fdiv.append('<b>' + Name + ':</b>');
                        fdiv.br();
                        Csw.each(thisFilterSet, makeFilterLink);
                        fdiv.br();
                        fdiv.br();
                    }
                    Csw.each(data.filters, makeFilterSet);

                    Csw.tryExec(internal.onAfterSearch);
                } // success
            }); // ajax
        } // search()


        internal.addFilter = function (thisFilter) {
            internal.filters[thisFilter.id] = thisFilter;
            internal.search()
        } // addFilter()

        return external;
    };

    Csw.controls.register('universalSearch', universalSearch);
    Csw.controls.universalSearch = Csw.controls.universalSearch || universalSearch;
})();
