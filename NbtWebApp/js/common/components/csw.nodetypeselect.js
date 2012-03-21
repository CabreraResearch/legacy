/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/js/ChemSW-vsdoc.js" />

(function () {

    Csw.controls.nodeTypeSelect = Csw.controls.nodeTypeSelect ||
        Csw.controls.register('nodeTypeSelect', function (options) {

            var internal = {
                $parent: '',
                ID: '',
                NodeTypesUrl: '/NbtWebApp/wsNBT.asmx/getNodeTypes',
                nodetypeid: '',
                objectClassName: '',
                onSelect: null,
                onSuccess: null,
                width: '200px',
                addNewOption: false,
                excludeNodeTypeIds: ''
            };
            if (options) {
                $.extend(internal, options);
            }

            (function () {

                if (options) {
                    $.extend(internal, options);
                }
                internal.ID += '_sel';

                $.extend(external, Csw.controls.select(internal));

                external.bind('change', function () {
                    Csw.tryExec(internal.onChange, external);
                    Csw.tryExec(internal.onSelect, external.val());
                });

                if (Csw.bool(internal.addNewOption)) {
                    external.option({ value: '[Create New]' });
                }

                Csw.ajax.post({
                    url: internal.NodeTypesUrl,
                    data: { ObjectClassName: Csw.string(internal.objectClassName), ExcludeNodeTypeIds: internal.excludeNodeTypeIds },
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

                        Csw.tryExec(internal.onSuccess, ret);
                        external.css('width', Csw.string(internal.width));
                    }
                });
            } ());
            
            return external;
        });
} ());

