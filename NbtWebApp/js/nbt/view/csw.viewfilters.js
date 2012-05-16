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


            internal.renderLinks = function(data)
            {
                var editbtn;
                var outertbl = internal.div.table();
                var outercell11 = outertbl.cell(1,1);
                var outercell12 = outertbl.cell(1,2);
                var outercell11div = outercell11.div()
                    .addClass('viewfiltersdiv');
                var tbl = outercell11div.table({ cellpadding: '2px' });

                outercell12.css({ width: '100%', textAlign: 'right' });

                var editbtn = outercell12.imageButton({
                    ButtonType: Csw.enums.imageButton_ButtonType.Edit,
                    AlternateText: 'Edit Filters',
                    ID: 'editfilterbtn',
                    onClick: function() {
                        
                        internal.renderDialog(data);

                    } // onClick
                }); // imageButton

                var row = 1;
                var viewPropFilters = {};
                Csw.each(data, function (propJson) {
                    Csw.each(propJson.filters, function (filtJson) {
                        viewPropFilters[filtJson.arbitraryid] = Csw.nbt.viewPropFilter({
                                parent: tbl,
                                viewid: internal.viewid,
                                viewJson: '',
                                propsData: propJson,
                                propname: propJson.name,
                                proparbitraryid: propJson.arbitraryid,
                                filtarbitraryid: filtJson.arbitraryid,
                                viewbuilderpropid: '',
                                propRow: row,
                                firstColumn: 1,
                                showPropertyName: true,
                                showSubfield: false,
                                readOnly: true,
                                selectedSubFieldName: filtJson.subfieldname,
                                selectedFilterMode: filtJson.filtermode,
                                selectedValue: filtJson.value,
                                autoFocusInput: false//,
                                //$clickOnEnter: filterbtn.$
                        });
                        row++;
                    }); //each()
                }); //each()

                if(row > 1) {   // at least one filter
                    internal.div.show();
                }
            }; // internal.renderLinks()


            internal.renderDialog = function(data)
            {
                var filterbtn;
                var dialogdiv = Csw.literals.div({ ID: internal.ID })
                                .addClass('viewfilters')
                                .hide();

                var tbl = dialogdiv.table({ cellpadding: '2px' });

                var row = 1;
                var viewPropFilters = {};
                Csw.each(data, function (propJson) {
                    
                    Csw.each(propJson.filters, function (filtJson) {

                        viewPropFilters[filtJson.arbitraryid] = Csw.nbt.viewPropFilter({
                                parent: tbl,
                                viewid: internal.viewid,
                                viewJson: '',
                                propsData: null,   // to fully populate the filter options
                                proparbitraryid: propJson.arbitraryid,
                                filtarbitraryid: filtJson.arbitraryid,
                                viewbuilderpropid: '',
                                propRow: row,
                                firstColumn: 1,
                                showPropertyName: true,
                                showSubfield: false,
                                readOnly: internal.readOnly,
                                selectedSubFieldName: filtJson.subfieldname,
                                selectedFilterMode: filtJson.filtermode,
                                selectedValue: filtJson.value,
                                autoFocusInput: false//,
                                //$clickOnEnter: filterbtn.$
                        });
                        row++;
                    }); //each()
                }); //each()

                $.CswDialog('GenericDialog', {
                    div: dialogdiv, 
                    title: 'Edit Filters', 
                    onOk: function() {
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
                    } // onOk
                }); // CswDialog
            }; // internal.renderEditable()
            
            
            // constructor
            (function () {

                if (options) $.extend(internal, options);

                internal.div = internal.parent.div({ ID: internal.ID })
                                .addClass('viewfilters')
                                .hide();

                Csw.ajax.post({
                    urlMethod: internal.filtersMethod,
                    data: { ViewId: internal.viewid },
                    success: function (data) {

                        // case 26331
                        // render as text and links first
                        // edit filters in dialog
                        internal.renderLinks(data);

                    } // success
                }); // ajax


            })(); // constructor
        }); // register
})();
