/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    Csw.controls.nodeTypeSelect = Csw.controls.nodeTypeSelect ||
        Csw.controls.register('nodeTypeSelect', function (cswParent, options) {
            'use strict';
            var cswPrivateVar = {
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
            var cswPublicRet = {};

            (function () {

                if (options) {
                    $.extend(cswPrivateVar, options);
                }
                cswPrivateVar.ID += '_sel';

                cswPrivateVar.select = cswParent.select(cswPrivateVar);

                cswPublicRet = Csw.dom({}, cswPrivateVar.select);

                //$.extend(cswPublicRet, Csw.literals.select(cswPrivateVar));

                cswPublicRet.bind('change', function () {
                    Csw.tryExec(cswPrivateVar.onChange, cswPublicRet, cswPrivateVar.nodetypecount);
                    Csw.tryExec(cswPrivateVar.onSelect, cswPublicRet.val(), cswPrivateVar.nodetypecount);
                });

                if (false === Csw.isNullOrEmpty(cswPrivateVar.blankOptionText)) {
                    cswPublicRet.option({
                        value: cswPrivateVar.blankOptionText,
                        isSelected: true
                    });
                }

                Csw.ajax.post({
                    urlMethod: cswPrivateVar.nodeTypesUrlMethod,
                    data: {
                        ObjectClassName: Csw.string(cswPrivateVar.objectClassName),
                        ObjectClassId: Csw.string(cswPrivateVar.objectClassId),
                        ExcludeNodeTypeIds: cswPrivateVar.excludeNodeTypeIds,
                        RelatedToNodeTypeId: cswPrivateVar.relatedToNodeTypeId,
                        RelatedObjectClassPropName: cswPrivateVar.relatedObjectClassPropName,
                        FilterToPermission: cswPrivateVar.filterToPermission
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
                                var option = cswPublicRet.option({ value: id, display: name });

                                Csw.each(thisNodeType, function (value, key) {
                                    option.propNonDom(key, value);
                                });
                            }
                        });
                        cswPrivateVar.nodetypecount = ret.nodetypecount;
                        
                        Csw.tryExec(cswPrivateVar.onSuccess, ret, cswPrivateVar.nodetypecount);
                        cswPublicRet.css('width', Csw.string(cswPrivateVar.width));
                    }
                });
            } ());

            return cswPublicRet;
        });
} ());

