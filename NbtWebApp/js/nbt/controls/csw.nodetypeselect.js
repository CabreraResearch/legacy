/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    Csw.controls.nodeTypeSelect = Csw.controls.nodeTypeSelect ||
        Csw.controls.register('nodeTypeSelect', function (cswParent, options) {
            'use strict';
            var internal = {
                $parent: '',
                ID: '',
                nodeTypesUrlMethod: 'getNodeTypes',
                nodetypeid: '',
                objectClassName: '',
                onSelect: null,
                onSuccess: null,
                width: '200px',
                addNewOption: false,
                labelText: null,
                excludeNodeTypeIds: '',
                relatedToNodeTypeId: '',
                relatedObjectClassPropName: ''
            };
            var external = {};

            (function () {

                if (options) {
                    $.extend(internal, options);
                }
                internal.ID += '_sel';

                internal.select = cswParent.select(internal);

                external = Csw.dom({}, internal.select);

                //$.extend(external, Csw.literals.select(internal));

                external.bind('change', function () {
                    Csw.tryExec(internal.onChange, external, internal.nodetypecount);
                    Csw.tryExec(internal.onSelect, external.val(), internal.nodetypecount);
                });

                if (Csw.bool(internal.addNewOption)) {
                    external.option({ value: '[Create New]' });
                }

                Csw.ajax.post({
                    urlMethod: internal.nodeTypesUrlMethod,
                    data: {
                        ObjectClassName: Csw.string(internal.objectClassName),
                        ExcludeNodeTypeIds: internal.excludeNodeTypeIds, 
                        RelatedToNodeTypeId: internal.relatedToNodeTypeId,
                        RelatedObjectClassPropName: internal.relatedObjectClassPropName
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
                                var option = external.option({ value: id, display: name });

                                Csw.each(thisNodeType, function (value, key) {
                                    option.propNonDom(key, value);
                                });
                            }
                        });
                        internal.nodetypecount = ret.nodetypecount;
                        Csw.tryExec(internal.onSuccess, ret, internal.nodetypecount);
                        external.css('width', Csw.string(internal.width));
                    }
                });
            } ());

            return external;
        });
} ());

