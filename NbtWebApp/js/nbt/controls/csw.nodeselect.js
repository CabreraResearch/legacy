/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    Csw.controls.nodeSelect = Csw.controls.nodeSelect ||
        Csw.controls.register('nodeSelect', function (cswParent, options) {
            'use strict';
            var internal = {
                $parent: '',
                ID: '',
                nodesUrlMethod: 'getNodes',
                nodeTypeId: '',
                objectClassId: '',
                objectClassName: '',
                onSelect: null,
                onSuccess: null,
                width: '200px',
                addNewOption: false,
                labelText: null,
                excludeNodeTypeIds: ''
            };
            var external = {};

            (function () {

                if (options) {
                    $.extend(internal, options);
                }
                internal.ID += '_nodesel';

                internal.select = cswParent.select(internal);

                external = Csw.dom({}, internal.select);

                external.bind('change', function () {
                    Csw.tryExec(internal.onChange, external);
                    Csw.tryExec(internal.onSelect, external.val());
                });
                
                Csw.ajax.post({
                    urlMethod: internal.nodesUrlMethod,
                    data: {
                        NodeTypeId: Csw.string(internal.nodeTypeId),
                        ObjectClassId: Csw.string(internal.objectClassId),
                        ObjectClass: Csw.string(internal.objectClassName)
                    },
                    success: function (data) {
                        var ret = data;
                        var nodecount = 0;
                        //Case 24155
                        Csw.each(ret, function (nodeName, nodeId) {
                            nodecount += 1;
                            external.option({ value: nodeId, display: nodeName });
                        });

                        Csw.tryExec(internal.onSuccess, ret);
                        external.css('width', Csw.string(internal.width));
                    }
                });
            } ());

            return external;
        });
} ());

