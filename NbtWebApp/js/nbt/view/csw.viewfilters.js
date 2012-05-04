/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    Csw.nbt.viewFilters = Csw.nbt.viewFilters ||
        Csw.nbt.register('viewFilters', function (options) {
            'use strict';

            var internal = {
                ID: '',
                parent: '',
                filtersMethod: 'getRuntimeViewFilters',
                viewid: '',
                onEditFilters: null
            };
            var external = {};

            // constructor
            (function () {

                if (options) $.extend(internal, options);

                var div = internal.parent.div({ ID: internal.ID });
                div.hide()
                   .addClass('viewfiltersdiv');
                var tbl = div.table();

                Csw.ajax.post({
                    urlMethod: internal.filtersMethod,
                    data: { ViewId: internal.viewid },
                    success: function (data) {
                        
                        var row = 1;
                        Csw.each(data, function (propJson) {
                            //propJson.propname = propJson.name;
                            Csw.each(propJson.filters, function (filtJson) {

                                tbl.$.CswViewPropFilter('init', {
                                    //options
                                    viewid: internal.viewid,
                                    viewJson: '',
                                    //propsData: propJson,
                                    proparbitraryid: propJson.arbitraryid,
                                    filtarbitraryid: filtJson.arbitraryid,
                                    viewbuilderpropid: '',
                                    propRow: row,
                                    firstColumn: 1,
                                    includePropertyName: true,
                                    advancedIsHidden: true,
                                    selectedSubfieldVal: filtJson.subfieldname,
                                    selectedFilterMode: filtJson.filtermode,
                                    selectedFilterVal: filtJson.value,
                                    autoFocusInput: false
                                });
                                row++;
                            }); //each()
                        }); //each()

                        if(row > 1) {   // at least one filter
                            var toprightcell = tbl.cell(1,5);
                            toprightcell.css({ width: '100%', textAlign: 'right' });

                            var filterbtn = tbl.cell(1, 5).imageButton({
                                ButtonType: Csw.enums.imageButton_ButtonType.Refresh,
                                AlternateText: 'Apply Filters',
                                ID: 'filterbtn',
                                onClick: function() {
                                    var filtersJson = {};

                                    Csw.each(data, function (propJson) {
                                        Csw.each(propJson.filters, function (filtJson) {
                                            var newFiltJson = tbl.$.CswViewPropFilter('getFilterJson', {
                                                ID: internal.ID,
                                                filtJson: propJson,
                                                proparbitraryid: propJson.arbitraryid,
                                                filtarbitraryid: filtJson.arbitraryid,
                                                allowNullFilterValue: true
                                            });
                                            filtersJson[filtJson.arbitraryid] = newFiltJson;
                                        });
                                    });
                                    Csw.tryExec(internal.onEditFilters, filtersJson);
                                } // onClick
                            });
                            div.show();
                        }
                    } // success
                }); // ajax

            })(); // constructor
        }); // register
})();
