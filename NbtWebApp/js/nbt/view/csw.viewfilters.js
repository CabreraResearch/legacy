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
                viewid: ''
            };
            var external = {};

            // constructor
            (function () {

                if (options) $.extend(internal, options);

                var tbl = internal.parent.table({ ID: internal.ID });
                tbl.addClass('viewfilterstbl');

                Csw.ajax.post({
                    urlMethod: internal.filtersMethod,
                    data: { ViewId: internal.viewid },
                    success: function (data) {

                        //Property_root_NT_8_OCP_1228: Object
                            //arbitraryid: "root_NT_8_OCP_1228"
                            //fieldtype: "List"
                            //filters: Object
                                //Filter_root_NT_8_OCP_1228_Value_NotEquals_Retired: Object
                                    //arbitraryid: "root_NT_8_OCP_1228_Value_NotEquals_Retired"
                                    //casesensitive: "True"
                                    //filtermode: "NotEquals"
                                    //nodename: "filter"
                                    //showatruntime: "True"
                                    //subfieldname: "Value"
                                    //value: "Retired"
                            //name: "Status"
                            //nodename: "property"
                            //nodetypepropid: "-2147483648"
                            //objectclasspropid: "1228"
                            //order: ""
                            //sortby: "False"
                            //sortmethod: "Ascending"
                            //type: "ObjectClassPropId"
                            //width: ""

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
                                    advancedIsHidden: false,
                                    selectedSubfieldVal: filtJson.subfieldname,
                                    selectedFilterMode: filtJson.filtermode,
                                    selectedFilterVal: filtJson.value,
                                    autoFocusInput: false
                                });
                                row++;
                            }); //each()

                        }); //each()
                    } // success
                }); // ajax

            })(); // constructor
        }); // register
})();
