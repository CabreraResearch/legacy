/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    Csw.actions.modules = Csw.actions.modules ||
        Csw.actions.register('modules', function (cswParent, options) {
            'use strict';
            var internal = {
                urlMethod: 'getModules',
                saveUrlMethod: 'saveModules',
                ID: 'action_modules'
            };
            if (options) $.extend(internal, options);

            // constructor
            (function () {

                internal.table = cswParent.table({
                    ID: internal.ID
                });

                Csw.ajax.post({
                    urlMethod: internal.urlMethod,
                    data: {},
                    success: function (result) {
                        var row = 1;
                        Csw.each(result, function (thisValue, thisModule) {
                            internal.table.cell(row, 1).append(thisModule);
                            internal.table.cell(row, 2).input({
                                ID: thisModule,
                                type: Csw.enums.inputTypes.checkbox,
                                checked: Csw.bool(thisValue)
                            });
                            row++;
                        }); //each()
                    } // success
                }); // ajax()

            }()); // constructor
        }); // register()

} ());
