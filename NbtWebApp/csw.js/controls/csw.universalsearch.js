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
            onSearch: null,
            searchurl: '/NbtWebApp/wsNBT.asmx/doUniversalSearch',
        };
        if (params) $.extend(internal, params);

        var external = {};

        // Constructor
        // Adds a searchbox to the form
        (function () {
            var cswtable = Csw.controls.table({
                ID: Csw.controls.dom.makeId({ ID: internal.ID, suffix: '_div' }),
                $parent: internal.$searchbox_parent
            });

            internal.searchinput = cswtable.cell(1, 1).input({
                ID: Csw.controls.dom.makeId({ ID: internal.ID, suffix: '_input' }),
                type: Csw.enums.inputTypes.text,
                width: internal.width
            });

            internal.searchbutton = cswtable.cell(1, 2).button({
                ID: Csw.controls.dom.makeId({ ID: internal.ID, suffix: '_srchbtn' }),
                enabledText: 'Search',
                disabledText: 'Searching...',
                onClick: function () {
                    Csw.tryExec(internal.onSearch);
                    internal.handleSearch();
                }
            });
        })();

        // Handle search submission
        internal.handleSearch = function() {
            var searchterm = internal.searchinput.getValue();
            Csw.ajax.post({
                url: internal.searchurl,
                data: { SearchTerm: searchterm },
                success: function(data) {
Csw.log(data);

                } // success
            }); // ajax
        } // handleSearch()

        return external;
    };

    Csw.controls.register('newSearch', universalSearch);
    Csw.controls.newSearch = Csw.controls.universalSearch || universalSearch;
})();
