/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.properties.relationship = Csw.properties.relationship ||
        Csw.properties.register('relationship',
            Csw.method(function (propertyOption) {
                'use strict';
                var cswPrivate = {};
                var cswPublic = {
                    data: propertyOption || Csw.nbt.propertyOption(propertyOption)
                };

                //The render function to be executed as a callback
                var render = function () {
                    'use strict';

                    cswPrivate.propVals = cswPublic.data.propData.values;
                    cswPrivate.parent = cswPublic.data.propDiv;

                    var nodeSelect = {};
                    nodeSelect.name = Csw.string(cswPublic.data.propData.name).trim();
                    nodeSelect.selectedNodeId = Csw.string(cswPrivate.propVals.relatednodeid).trim();
                    nodeSelect.selectedNodeLink = Csw.string(cswPrivate.propVals.relatednodelink).trim();
                    nodeSelect.selectedName = Csw.string(cswPrivate.propVals.name).trim();
                    nodeSelect.nodeTypeId = Csw.string(cswPrivate.propVals.nodetypeid).trim();
                    nodeSelect.viewid = Csw.string(cswPrivate.propVals.viewid).trim();
                    nodeSelect.objectClassId = Csw.string(cswPrivate.propVals.objectclassid).trim();
                    nodeSelect.allowAdd = Csw.bool(cswPrivate.propVals.allowadd);
                    nodeSelect.options = cswPrivate.propVals.options;
                    nodeSelect.useSearch = Csw.bool(cswPrivate.propVals.usesearch);
                    nodeSelect.cellCol = 1;
                    nodeSelect.selectedNodeType = null;
                    nodeSelect.addImage = null;
                    nodeSelect.onAddNodeFunc = function () { };
                    nodeSelect.onSelectNode = function (nodeObj) {
                        Csw.tryExec(cswPublic.data.onChange, nodeObj.nodeid);
                        cswPublic.data.onPropChange({ nodeid: nodeObj.nodeid });
                    };

                    nodeSelect.relatedTo = {};
                    nodeSelect.relatedTo.relatednodeid = cswPublic.data.tabState.relatednodeid;
                    nodeSelect.relatedTo.relatednodetypeid = cswPublic.data.tabState.relatednodetypeid;
                    nodeSelect.relatedTo.relatednodename = cswPublic.data.tabState.relatednodename;
                    nodeSelect.relatedTo.relatedobjectclassid = cswPublic.data.tabState.relatedobjectclassid;

                    nodeSelect.isRequired = cswPublic.data.isRequired();
                    nodeSelect.isMulti = cswPublic.data.isMulti();
                    nodeSelect.isReadOnly = cswPublic.data.isReadOnly();

                    nodeSelect.doGetNodes = false;

                    nodeSelect.showSelectOnLoad = (function () {
                        return cswPublic.data.tabState.EditMode === Csw.enums.editMode.Add ||
                               cswPublic.data.isMulti() ||
                            (cswPublic.data.isRequired() && Csw.isNullOrEmpty(nodeSelect.selectedNodeId));
                    } ());

                    cswPublic.control = cswPrivate.parent.nodeSelect(nodeSelect);

                };

                //Bind the callback to the render event
                cswPublic.data.bindRender(render);

                //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
                //cswPublic.data.unBindRender();

                return cswPublic;
            }));

} ());        