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
                applyMethod: 'updateRuntimeViewFilters',
                viewid: '',
                onEditFilters: null
            };
            var external = {};

            // constructor
            (function () {

                if (options) $.extend(internal, options);

                var filterbtn;
                var div = internal.parent.div({ ID: internal.ID })
                            .addClass('viewfilters')
                            .hide();
                var outertbl = div.table();
                var outercell11 = outertbl.cell(1,1);
                var outercell12 = outertbl.cell(1,2);
                var outercell11div = outercell11.div()
                   .addClass('viewfiltersdiv');
                var tbl = outercell11div.table();

                Csw.ajax.post({
                    urlMethod: internal.filtersMethod,
                    data: { ViewId: internal.viewid },
                    success: function (data) {
                        
                        outercell12.css({ width: '100%', textAlign: 'right' });

                        var filterbtn = outercell12.imageButton({
                            ButtonType: Csw.enums.imageButton_ButtonType.Refresh,
                            AlternateText: 'Apply Filters',
                            ID: 'filterbtn',
                            onClick: function() {
                                var filtersJson = {};

                                Csw.each(data, function (propJson) {
                                    Csw.each(propJson.filters, function (filtJson) {
                                        filtersJson[filtJson.arbitraryid] = viewPropFilters[filtJson.arbitraryid].getFilterJson();
                                    });
                                });
                                    
                                Csw.ajax.post({
                                    urlMethod: internal.applyMethod,
                                    data: { 
                                        ViewId: internal.viewid, 
                                        FiltersJson: JSON.stringify(filtersJson) 
                                    },
                                    success: function(data) {
                                        Csw.tryExec(internal.onEditFilters, data.newviewid);
                                    }
                                });
                                    
                            } // onClick
                        }); // imageButton

                        var row = 1;
                        var viewPropFilters = {};
                        Csw.each(data, function (propJson) {
                            //propJson.propname = propJson.name;
                            Csw.each(propJson.filters, function (filtJson) {

                                viewPropFilters[filtJson.arbitraryid] = Csw.nbt.viewPropFilter({
                                        parent: tbl,
                                        viewid: internal.viewid,
                                        viewJson: '',
                                        proparbitraryid: propJson.arbitraryid,
                                        filtarbitraryid: filtJson.arbitraryid,
                                        viewbuilderpropid: '',
                                        propRow: row,
                                        firstColumn: 1,
                                        showPropertyName: true,
                                        showSubfield: false,
                                        selectedSubFieldName: filtJson.subfieldname,
                                        selectedFilterMode: filtJson.filtermode,
                                        selectedValue: filtJson.value,
                                        autoFocusInput: false,
                                        $clickOnEnter: filterbtn.$
                                });
                                row++;
                            }); //each()
                        }); //each()

                        if(row > 1) {   // at least one filter

                            div.show();
                        }
                    } // success
                }); // ajax

            })(); // constructor
        }); // register
})();
