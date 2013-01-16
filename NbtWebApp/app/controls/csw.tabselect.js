/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    Csw.controls.tabSelect = Csw.controls.tabSelect ||
        Csw.controls.register('tabSelect', function (cswParent, options) {
            'use strict';
            var cswPrivate = {
                name: '',
                value: '',
                selectedName: '',
                nodeTypeName: '',
                nodeTypeId: '',
                onSelect: null,
                onSuccess: null,
                width: '200px',
                filterToPermission: '',
                includeIdentityTab: false
            };
            var cswPublic = {};

            (function () {
                if (options) {
                    Csw.extend(cswPrivate, options);
                }
                cswPrivate.name += '_tabsel';
                cswPrivate.select = cswParent.select(cswPrivate);
                cswPublic = Csw.dom({}, cswPrivate.select);

                cswPublic.bind('change', function () {
                    Csw.tryExec(cswPrivate.onChange, cswPublic);
                    Csw.tryExec(cswPrivate.onSelect, cswPublic.val());
                });

                Csw.ajax.post({
                    urlMethod: 'getNodeTypeTabs',
                    data: {
                        NodeTypeName: Csw.string(cswPrivate.nodeTypeName),
                        NodeTypeId: Csw.string(cswPrivate.nodeTypeId),
                        FilterToPermission: cswPrivate.filterToPermission
                    },
                    success: function (data) {
                        Csw.each(data, function (thisTab) {
                            if (Csw.contains(thisTab, 'id') && Csw.contains(thisTab, 'name')) {
                                if(cswPrivate.includeIdentityTab || thisTab.name !== 'Identity') {
                                    cswPublic.option({
                                        value: thisTab.id,
                                        display: thisTab.name,
                                        isSelected: cswPrivate.isSelected(thisTab.id, thisTab.name)
                                    });
                                }
                            }
                        });
                        Csw.tryExec(cswPrivate.onSuccess, data);
                        cswPublic.css('width', Csw.string(cswPrivate.width));
                    }
                });

                ///Summary
                //Returns true if option matches the id or name that was originally passed into the control. 
                //If both id and name are present, id takes precedence
                cswPrivate.isSelected = function(id, name) {
                    return (false === Csw.isNullOrEmpty(cswPrivate.value) &&
                        Csw.number(cswPrivate.value) === Csw.number(id)) ||
                        (Csw.isNullOrEmpty(cswPrivate.value) &&
                            false === Csw.isNullOrEmpty(cswPrivate.selectedName) &&
                            cswPrivate.selectedName === name);
                };

            }());

            return cswPublic;
        });
}());

