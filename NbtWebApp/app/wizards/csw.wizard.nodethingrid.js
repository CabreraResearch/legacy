
/// <reference path="~app/CswApp-vsdoc.js" />

(function () {

    Csw.wizard.nodeThinGrid = Csw.wizard.nodeThinGrid ||
        Csw.wizard.register('nodeThinGrid', function (cswParent, options) {
            'use strict';
            ///<summary>Creates a thin grid with an Add form.</summary>
            Csw.error.throwException(Csw.error.exception('Csw.wizard.nodeThinGrid probably (possibly [maybe {dubiously} ] ) works, but it hasn\'t been tested. At all. Not even a little. You could start by uncommenting this line.', '', 'csw.wizard.nodethingrid.js', 22));
            var cswPrivate = {
                ID: 'wizardNodeThinGrid',
                viewid: '',
                nodetypeid: '',
                excludeOcProps: []
            };
            if (options) Csw.extend(cswPrivate, options);

            var cswPublic = {
                nodes: []
            };

            (function _pre() {
                if (Csw.isNullOrEmpty(cswParent)) {
                    Csw.error.throwException(Csw.error.exception('Cannot create a Wizard thin grid without a parent.', '', 'csw.wizard.nodethingrid.js', 22));
                }
                if (Csw.isNullOrEmpty(cswPrivate.viewid)) {
                    Csw.error.throwException(Csw.error.exception('Cannot create a Wizard thin grid without a View ID.', '', 'csw.wizard.nodethingrid.js', 25));
                }
            } ());

            cswPrivate.makeGrid = function (viewid) {
                'use strict';
                cswPublic.rootDiv = cswPublic.rootDiv || cswParent.div();
                cswPublic.rootDiv.empty();
                
                function isNodeNew(node) {
                    var ret = true;
                    Csw.each(nodes, function (nodeVal) {
                        for (var n = 0; n < nodeVal.length; n += 1) {
                            Csw.debug.assert(Csw.string(nodeVal[n]).toLowerCase() === Csw.string(node[n]).toLowerCase(), 'Prop ' + nodeVal[n] + ' was not equal to Prop ' + node[n] + ' at index ' + n + '.');
                            ret = ret && Csw.string(nodeVal[n]).toLowerCase() === Csw.string(node[n]).toLowerCase();
                        }
                    });
                    return ret;
                }

                function addNodeLayout() {
                    cswPrivate.addDiv = addDiv || cswPublic.rootDiv.div();
                    cswPrivate.addDiv.empty();
                    cswPrivate.tabsAndProps = Csw.wizard.addLayout(cswPrivate.addDiv, {
                        nodetypeid: cswPrivate.nodetypeid,
                        excludeOcProps: cswPrivate.excludeOcProps
                    });

                    cswPrivate.addNodeBtn = cswPrivate.addDiv.button({
                        icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.add),
                        enabledText: 'Add',
                        onClick: function () {
                            var nodeData = {};
                            Csw.extend(nodeData, cswPrivate.tabsAndProps.getPropJson(), true);
                            
                            Csw.ajax.post({
                                urlMethod: 'nodePropsToArray',
                                data: {
                                    NodeDefinition: JSON.stringify(nodeData),
                                    NodeTypeId: cswPrivate.nodetypeid
                                },
                                success: function (data) {
                                    var node = data.row;
                                    if (isNodeNew(node)) {
                                        cswPublic.thinGrid.addRows(node);
                                        cswPublic.nodes.push(nodeData);
                                    } else {
                                        $.CswDialog('AlertDialog', 'This node is already defined. Please define a new, unique node.');
                                    }
                                }
                            });
                        }
                    });
                }

                addNodeLayout();
                cswPublic.thinGrid = cswPublic.rootDiv.thinGrid({ linkText: '', hasHeader: true });

                Csw.ajax.post({
                    urlMethod: 'getThinGrid',
                    data: {
                        ViewId: viewid,
                        IncludeNodeKey: '',
                        MaxRows: 1000
                    },
                    success: function (data) {
                        cswPublic.nodes = data.rows || [];

                        cswPublic.thinGrid.addRows(cswPublic.nodes);
                    }
                });

                cswPublic.rootDiv.br();
            };

            (function _post() {
                cswPrivate.makeGrid(cswPrivate.viewid);
            } ());

            return cswPublic;

        });
} ());

