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
            searchurl: '/NbtWebApp/wsNBT.asmx/doUniversalSearch'
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

            internal.searchinput.clickOnEnter(internal.searchbutton);
        })();

        // Handle search submission
        internal.handleSearch = function() {
            var searchterm = internal.searchinput.val();
            
            Csw.tryExec(internal.onBeforeSearch);
            
            internal.$searchresults_parent.CswNodeTable({
                searchterm: searchterm,
                ID: Csw.controls.dom.makeId({ ID: internal.ID, suffix: '_srchresults' }),
                onEditNode: null,
                onDeleteNode: null,
                onSuccess: internal.onAfterSearch,
                onNoResults: function() {
                    internal.$searchresults_parent.append('No Results Found'); 
                }
            });

        } // handleSearch()

        return external;
    };

    Csw.controls.register('universalSearch', universalSearch);
    Csw.controls.universalSearch = Csw.controls.universalSearch || universalSearch;
})();
