/// <reference path="~/app/CswApp-vsdoc.js" />

/*global Csw:true*/
(function csw_nbt_nodeTree() {
    "use strict";

    Csw.nbt.nodeTreeExt = Csw.nbt.nodeTreeExt ||
        Csw.nbt.register('nodeTreeExt', function (cswParent, opts) {

            var cswPrivate = {
                forSearch: false, // if true, used to override default behavior of list views
                onSelectNode: null, // function (optSelect) { var o =  { nodeid: '',  nodename: '', iconurl: '', nodekey: '', viewid: '' }; return o; },
                onBeforeSelectNode: function () { return true; }, //false prevents selection
                isMulti: false,
                validateCheckboxes: true,
                showToggleLink: true,
                useScrollbars: true,
                useHover: true,
                height: '',
                width: '',

                //State
                state: {
                    viewId: '',
                    viewMode: '',
                    nodeId: '',
                    nodeKey: '',
                    includeNodeRequired: false,
                    includeInQuickLaunch: true,
                    onViewChange: function () { },
                    treeData: {

                    },
                    defaultSelect: Csw.enums.nodeTree_DefaultSelect.firstchild.name
                }
            };

            var cswPublic = {};

            (function _preCtor() {
                Csw.extend(cswPrivate, opts);
                cswPublic.div = cswParent.div();
            } ());

            cswPrivate.allowMultiSelection = function (currentNode, checkedNode) {
                var ret = false;
                if (currentNode && currentNode.raw && currentNode.raw.nodetypeid &&
                    checkedNode && checkedNode.raw && checkedNode.raw.nodetypeid &&
                    currentNode.raw.nodetypeid === checkedNode.raw.nodetypeid) {
                    ret = true;
                }
                return ret;
            };

            cswPrivate.make = function (data) {

                var treeOpts = {
                    name: data.Name,
                    height: cswPrivate.height,
                    width: cswPrivate.width,

                    root: data.Tree,
                    columns: data.Columns,
                    fields: data.Fields,
                    selectedId: data.SelectedId,

                    onSelect: cswPrivate.handleSelectNode,
                    beforeSelect: cswPrivate.onBeforeSelectNode,
                    allowMultiSelection: cswPrivate.allowMultiSelection,

                    useArrows: cswPrivate.state.viewMode !== Csw.enums.viewMode.list.name,
                    useToggles: cswPrivate.showToggleLink,
                    useCheckboxes: cswPrivate.isMulti,
                    useScrollbars: cswPrivate.useScrollbars
                };
                if (cswPrivate.useHover) {
                    treeOpts.onMouseEnter = hoverNode;
                    treeOpts.onMouseExit = deHoverNode;
                }
                cswPublic.nodeTree = cswPublic.div.tree(treeOpts);

                function hoverNode(event, treeNode, htmlElement, index, eventObj, eOpts) {
                    cswPrivate.hoverNodeId = treeNode.raw.nodeid;
                    var $div = $(htmlElement).children().first().children();
                    var div = Csw.literals.factory($div);

                    Csw.nodeHoverIn(event, {
                        nodeid: cswPrivate.hoverNodeId,
                        nodekey: treeNode.raw.id,
                        nodename: treeNode.raw.text,
                        parentDiv: div,
                        buttonHoverIn: function () {
                            cswPublic.nodeTree.preventSelect();
                        },
                        buttonHoverOut: function () {
                            cswPublic.nodeTree.allowSelect();
                        }
                    });
                } // hoverNode()

                function deHoverNode() {
                    Csw.nodeHoverOut();
                }

            }; // cswPrivate.make()

            cswPrivate.selectedNode = null;

            cswPrivate.handleSelectNode = function (myoptions) {
                var m = {
                    e: '',
                    data: '',
                    url: '',
                    onSelectNode: cswPrivate.onSelectNode,
                    viewid: '',
                    forsearch: cswPrivate.forsearch
                };
                Csw.extend(m, myoptions);

                var optSelect = {
                    nodeid: m.nodeid,
                    nodename: m.text,
                    iconurl: m.icon,
                    nodekey: m.id,
                    nodespecies: m.species,
                    viewid: cswPrivate.state.viewId
                };

                Csw.tryExec(m.onSelectNode, optSelect);
            }; // cswPrivate.handleSelectNode()


            // For making a tree without using the regular mechanism for fetching tree data
            cswPublic.makeTree = function (treeData) {
                cswPrivate.make(treeData);
            };

            cswPublic.getChecked = function () {
                var checked = cswPublic.nodeTree.getChecked();
                var ret = [];
                if (checked && checked.length > 0) {
                    checked.forEach(function (treeNode) {
                        ret.push({ nodeid: treeNode.raw.nodeid, nodekey: treeNode.raw.id });
                    });
                }
                return ret;
            };

            cswPrivate.runTree = function () {
                Csw.ajaxWcf.post({
                    urlMethod: 'Trees/run',
                    data: {
                        AccessedByObjClassId: '',
                        DefaultSelect: cswPrivate.state.defaultSelect,
                        IncludeInQuickLaunch: cswPrivate.state.includeInQuickLaunch,
                        IncludeNodeRequired: cswPrivate.state.includeNodeRequired,
                        NbtViewId: cswPrivate.state.viewId,
                        NodeId: cswPrivate.state.nodeId,
                        NodeKey: cswPrivate.state.nodeKey,
                        UseCheckboxes: cswPrivate.isMulti
                    },
                    success: function (data) {

                        if (Csw.isNullOrEmpty(data.Tree)) {
                            Csw.error.showError(Csw.enums.errorType.error.name,
                                    'The requested view cannot be rendered as a Tree.',
                                    'View with ViewId: ' + cswPrivate.state.viewId + ' does not exist or is not a Tree view.');
                        } else {
                            if (Csw.clientSession.isDebug()) {
                                data.Columns.forEach(function (column) {
                                    if (column.dataIndex === 'text') {
                                        column.menuDisabled = false;
                                    }
                                });
                            }
                            if (false === Csw.isNullOrEmpty(data.NewViewId) && cswPrivate.state.viewId !== data.NewViewId) {
                                cswPrivate.state.viewId = data.NewViewId;
                                cswPrivate.state.viewMode = data.NewViewMode;
                                Csw.tryExec(cswPrivate.state.onViewChange, data.NewViewId, data.NewViewMode);
                            }

                            cswPrivate.make(data);
                        }

                    } // success
                }); // ajax
            }; // runTree

            (function _postCtor() {

                cswPrivate.runTree();

                Csw.subscribe('CswMultiEdit', (function _onMultiInvoc() {
                    return function _onMulti(eventObj, multiOpts) {
                        if (multiOpts && multiOpts.viewid === cswPrivate.state.viewId) {
                            //cswPublic.nodeTree.is.multi = (multiOpts.multi || Csw.bool(cswPrivate.ShowCheckboxes));
                            //cswPublic.nodeTree.toggleUseCheckboxes();
                            cswPrivate.isMulti = multiOpts.multi;
                            cswPrivate.runTree();
                        } else {
                            Csw.unsubscribe('CswMultiEdit', null, _onMulti);
                            Csw.unsubscribe('CswMultiEdit', null, _onMultiInvoc);
                        }
                    };
                } ()));

            })(); // constructor

            return cswPublic;
        });


})();