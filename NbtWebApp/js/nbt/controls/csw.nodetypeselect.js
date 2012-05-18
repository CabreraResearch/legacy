/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    Csw.controls.nodeTypeSelect = Csw.controls.nodeTypeSelect ||
        Csw.controls.register('nodeTypeSelect', function (cswParent, options) {
            'use strict';
            var cswPrivate = {
                $parent: '',
                ID: '',
                nodeTypesUrlMethod: 'getNodeTypes',
                nodetypeid: '',
                objectClassName: '',
                objectClassId: '',
                onSelect: null,
                onSuccess: null,
                width: '200px',
                blankOptionText: '',
                filterToPermission: '',
                labelText: null,
                excludeNodeTypeIds: '',
                relatedToNodeTypeId: '',
                relatedObjectClassPropName: ''
            };
            var cswPublic = {};

            (function () {

                if (options) {
                    $.extend(cswPrivate, options);
                }
                cswPrivate.ID += '_sel';

                cswPrivate.select = cswParent.select(cswPrivate);

                cswPublic = Csw.dom({}, cswPrivate.select);

                //$.extend(cswPublic, Csw.literals.select(cswPrivate));

                cswPublic.bind('change', function () {
                    Csw.tryExec(cswPrivate.onChange, cswPublic, cswPrivate.nodetypecount);
                    Csw.tryExec(cswPrivate.onSelect, cswPublic.val(), cswPrivate.nodetypecount);
                });

                if (false === Csw.isNullOrEmpty(cswPrivate.blankOptionText)) {
                    cswPublic.option({
                        value: cswPrivate.blankOptionText,
                        isSelected: true
                    });
                }

                Csw.ajax.post({
                    urlMethod: cswPrivate.nodeTypesUrlMethod,
                    data: {
                        ObjectClassName: Csw.string(cswPrivate.objectClassName),
                        ObjectClassId: Csw.string(cswPrivate.objectClassId),
                        ExcludeNodeTypeIds: cswPrivate.excludeNodeTypeIds,
                        RelatedToNodeTypeId: cswPrivate.relatedToNodeTypeId,
                        RelatedObjectClassPropName: cswPrivate.relatedObjectClassPropName,
                        FilterToPermission: cswPrivate.filterToPermission
                    },
                    success: function (data) {
                        var ret = data;
                        ret.nodetypecount = 0;
                        //Case 24155
                        Csw.each(ret, function (thisNodeType) {
                            if (Csw.contains(thisNodeType, 'id') &&
                            Csw.contains(thisNodeType, 'name')) {
                                var id = thisNodeType.id,
                                    name = thisNodeType.name;
                                delete thisNodeType.id;
                                delete thisNodeType.name;

                                ret.nodetypecount += 1;
                                var option = cswPublic.option({ value: id, display: name });

                                Csw.each(thisNodeType, function (value, key) {
                                    option.propNonDom(key, value);
                                });
                            }
                        });
                        cswPrivate.nodetypecount = ret.nodetypecount;
                        
                        Csw.tryExec(cswPrivate.onSuccess, ret, cswPrivate.nodetypecount);
                        cswPublic.css('width', Csw.string(cswPrivate.width));
                    }
                });
            } ());

            return cswPublic;
        });
} ());

