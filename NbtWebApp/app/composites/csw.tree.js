/*global Csw:true,Ext:true*/
(function () {
    'use strict';

    Csw.composites.tree = Csw.composites.tree ||
        Csw.composites.register('tree', function (cswParent, cswPrivate) {

            //#region Variables

            var cswPublic = {};

            //#endregion Variables

            //#region Pre-ctor
            (function _pre() {
                //set default values on cswPrivate if none are supplied
                cswPrivate.name = cswPrivate.name || 'No name';
                cswPrivate.bodyStyle = cswPrivate.bodyStyle || ''; //'background-img:url(path/to/img)

                //Data for the tree
                cswPrivate.root = cswPrivate.root || {
                    expanded: true,
                    children: [
                        { text: 'No Children', leaf: true }
                    ],
                    text: 'No Tree Provided'
                };

                cswPrivate.columns = cswPrivate.columns || [{
                    xtype: 'treecolumn', //this is so we know which column will show the tree
                    dataIndex: 'text',
                    menuDisabled: true
                }];

                cswPrivate.fields = cswPrivate.fields || [{
                    name: 'text',
                    type: 'string'
                }];

                //State
                cswPrivate.allowMultiSelection = cswPrivate.allowMultiSelection || function () { };

                //Styling
                cswPrivate.height = cswPrivate.height || '100%';
                cswPrivate.width = cswPrivate.width || 270;
                cswPrivate.title = cswPrivate.title || 'No Title';
                cswPrivate.useArrows = cswPrivate.useArrows; //For Lists, useArrows should be false
                cswPrivate.useToggles = cswPrivate.useToggles;
                cswPrivate.useCheckboxes = cswPrivate.useCheckboxes;
                cswPrivate.useScrollbars = cswPrivate.useScrollbars;
                cswPrivate.rootVisible = cswPrivate.rootVisible;
                
                //Events
                cswPrivate.onClick = cswPrivate.onClick || function () { };
                cswPrivate.onMouseEnter = cswPrivate.onMouseEnter || function () { };
                cswPrivate.onMouseExit = cswPrivate.onMouseExit || function () { };
                cswPrivate.beforeSelect = cswPrivate.beforeSelect || function () { return true; };
                cswPrivate.preventSelect = false;

                cswPrivate.lastSelectedPathDbName = 'CswTree_' + cswPrivate.name + '_LastSelectedPath';

                cswParent.empty();
                cswPublic.div = cswParent.div();

                if (cswPrivate.useScrollbars) {
                    cswPublic.div.addClass('treediv');
                } else {
                    cswPublic.div.addClass('treediv_noscroll');
                }
            } ());

            //#endregion Pre-ctor


            //#region Define Class Members

            cswPrivate.getLastSelectedPath = function () {
                var lastSelectedPath;
                if (Csw.isNullOrEmpty(cswPrivate.selectedId)) {
                    lastSelectedPath = Csw.clientDb.getItem(cswPrivate.lastSelectedPathDbName);
                } else {
                    lastSelectedPath = cswPublic.getPathFromId(cswPrivate.selectedId);
                }
                if (!lastSelectedPath) {
                    lastSelectedPath = cswPrivate.rootNode.childNodes[0].raw.path;
                }
                Csw.clientDb.setItem(cswPrivate.lastSelectedPathDbName, lastSelectedPath);
                return lastSelectedPath;
            }; // getLastSelectedPath()

            cswPrivate.makeStore = function () {
                /// <summary>
                /// Makes the data store for the tree. Non-volatile.
                /// </summary>
                /// <returns type="Ext.data.TreeStore">Ext data store</returns>
                window.Ext.define('Tree', {
                    extend: 'Ext.data.Model',
                    fields: cswPrivate.fields
                });
                cswPrivate.store = cswPrivate.store ||
                    window.Ext.create('Ext.data.TreeStore', {
                        model: 'Tree',
                        root: cswPrivate.root[0]
                    });
                return cswPrivate.store;
            };

            cswPrivate.makeListeners = function () {
                /// <summary>
                /// Simple abstraction for event bindings.
                /// </summary>
                /// <returns type="Object">Set of events to bind</returns>
                return {
                    //*click*: function() {
                    // Ext exposes a slew of click handlers. None of them work unless the node is *selected*, so don't bother.
                    //}
                    itemmouseenter: function (thisView, treeNode, htmlElement, index, eventObj, eOpts) {
                        Csw.tryExec(cswPrivate.onMouseEnter, window.event, treeNode, htmlElement, index, eventObj, eOpts);
                    },
                    itemmouseleave: function (thisView, treeNode, htmlElement, index, eventObj, eOpts) {
                        Csw.tryExec(cswPrivate.onMouseExit, window.event, treeNode, htmlElement, index, eventObj, eOpts);
                    },
                    afterlayout: function () {
                        //afterlayout fires anytime you expand/collapse nodes in the tree. It fires once for all new content.
                        //cswPublic.toggleCheckboxes();
                        $('.x-grid-cell-treecolumn').css({
                            background: 'transparent',
                            border: '0px'
                        });
                        $('#' + this.id + '-body').css({
                            background: 'transparent',
                            border: '0px'
                        });
                        Csw.each(this.dockedItems.items, function (item) {
                            $('#' + item.id).css({
                                background: 'transparent',
                                border: '0px'
                            });
                        });
                    },
                    afterrender: function () {
                        //Despite the fact that this is the last "render" event to fire, the tree is still _NOT_ in the DOM. 
                        //It _will_ in the next nano-second, so we have to defer.
                        //cswPublic.toggleCheckboxes();
                    },
                    viewready: function () {
                        //This is the "last" event to fire, but it's _still_ not safe to assume the DOM is ready.
                        Csw.defer(function () {
                            cswPrivate.rootNode = cswPublic.tree.getRootNode();

                            var lastSelectedPath = cswPrivate.getLastSelectedPath();

                            cswPublic.selectNode(null, lastSelectedPath, function _success(succeeded, oLastNode) {
                                if (!succeeded || oLastNode.isRoot()) {
                                    var firstChild = cswPrivate.rootNode.childNodes[0];
                                    Csw.clientDb.setItem('CswTree_LastSelectedPath', firstChild.raw.path);
                                    cswPublic.selectNode(null, firstChild.raw.path);
                                }
                            });
                            //cswPublic.toggleMultiEdit(cswPublic.is.multi);
                        }, 10);

                    },
                    afteritemcollapse: function () {
                        //cswPublic.toggleCheckboxes();
                    },
                    beforedeselect: function (rowModel, record, index, eOpts) {
                        return (cswPrivate.useCheckboxes !== true || cswPrivate.selectedNodeCount <= 1);
                    },
                    beforeselect: function (rowModel, record, index, eOpts) {
                        var ret = (false === cswPrivate.preventSelect && (cswPrivate.useCheckboxes !== true || cswPrivate.selectedNodeCount <= 1));
                        if (false !== ret && cswPrivate.useCheckboxes !== true) {
                            ret = Csw.tryExec(cswPrivate.beforeSelect);
                        }
                        return ret;
                    },
                    select: function (rowModel, record, index, eOpts) {
                        //If you click a node, this event is firing. We must:
                        //1. Keep this generic. Defer to the caller for implementation-specific validation.
                        //2. Properly track the currently and previously selected node (multiple clicks to the same node trigger this event)
                        if (cswPublic.selectedTreeNode !== record) {
                            cswPublic.selectedTreeNode = record;
                            cswPrivate.selectedNodeCount = 1;
                            record.expand();
                            if (cswPublic.selectedTreeNode.raw) {
                                Csw.clientDb.setItem(cswPrivate.lastSelectedPathDbName, cswPublic.selectedTreeNode.raw.path);
                            }
                            Csw.tryExec(cswPrivate.onSelect, record.raw);

                            if (cswPrivate.useCheckboxes) {
                                // also check this node
                                cswPrivate.check(record, true);
                            }
                        }
                    },
                    checkchange: function (record, checked, eOpts) {
                        //Unfortunately, this event doesn't fire regularly if watched. Debug carefully!
                        cswPrivate.check(record, checked);
                    }
                };
            }; // cswPrivate.makeListeners()

            cswPrivate.check = function (record, checked) {
                //Ext doesn't update the raw data, which is the only thing we have to manage state.
                //Manually keep it up to date.
                var inc = (checked) ? 1 : -1;
                if (cswPrivate.useCheckboxes) {
                    if (null !== cswPublic.selectedTreeNode &&
                        false === Csw.tryExec(cswPrivate.allowMultiSelection, cswPublic.selectedTreeNode, record)) {
                        // manually "uncheck" this node and select the unchanged current node
                        checked = false;
                        inc = 0;
                    }
                }
                record.raw.checked = checked;
                record.set('checked', checked);
                cswPrivate.selectedNodeCount += inc;
            }; // cswPrivate.check()

            cswPrivate.selectedNodeCount = 0;
            cswPublic.selectedTreeNode = null;

            cswPrivate.makeTree = function () {
                /// <summary>
                /// Constructs the tree components and attaches it to the DOM
                /// </summary>
                /// <returns type="Ext.tree.Panel">A tree.</returns>
                var allExpanded = false;
                var hideHeaders = (false === Csw.clientSession.isDebug());
                var treeOpts = {
                    store: cswPrivate.makeStore(), //just like grids, we need a data store
                    renderTo: cswPublic.div.getId(),
                    height: cswPrivate.height,
                    width: cswPrivate.width,
                    //maxWidth: cswPrivate.width,
                    title: cswPrivate.root[0].text,  //root should already be validated at this point
                    useArrows: cswPrivate.useArrows, //not for List views
                    rootVisible: cswPrivate.rootVisible,
                    hideHeaders: hideHeaders, //this hides the tree grid column names
                    border: false,
                    bodyStyle: 'background: transparent !important;',
                    listeners: cswPrivate.makeListeners(),
                    columns: cswPrivate.columns, //this is secretly a tree grid
                    dockedItems: {
                        xtype: 'toolbar',
                        items: []
                    },
                    scroll: false,
                    plugins: [new Ext.ux.tree.plugin.NodeDisabled()]
                };

                if (cswPrivate.useArrows) { //then show expand/collapse
                    if (cswPrivate.useToggles !== false) {
                        treeOpts.dockedItems.items.push({
                            text: 'Expand All',
                            handler: function () {
                                var toolbar = this;
                                cswPublic.toggleExpanded(toolbar);
                            }
                        });

                    }
                }

                cswPublic.tree = window.Ext.create('Ext.tree.Panel', treeOpts);
                return cswPublic.tree;
            };

            //#region Tree Mutators

            cswPublic.collapseAll = function (button, toolbar) {
                /// <summary>
                /// Collapses all nodes in the tree.
                /// </summary>
                /// <returns type="Csw.composites.tree">This tree</returns>
                if (cswPrivate.useToggles) {
                    button.setText('Expand All');
                    cswPublic.tree.getEl().mask('Collapsing tree...');
                }
                cswPublic.tree.collapseAll(function () {
                    if (cswPrivate.useToggles) {
                        cswPrivate.rootNode.expand();     // expand the root
                        cswPublic.tree.getEl().unmask();
                        toolbar.enable();
                    }
                });
                return cswPublic;
            };

            cswPublic.expandAll = function (button, toolbar) {
                /// <summary>
                /// Expand all nodes in the tree.
                /// </summary>
                /// <returns type="Csw.composites.tree">This tree</returns>
                if (cswPrivate.useToggles) {
                    button.setText('Collapse All');
                    cswPublic.tree.getEl().mask('Expanding tree...');
                }
                cswPublic.eachNode(function (node) {
                    node.expand();
                });
                if (cswPrivate.useToggles) {
                    cswPublic.tree.getEl().unmask();
                    toolbar.enable();
                }
                return cswPublic;
            };

            cswPrivate.allExpanded = false;

            cswPublic.toggleExpanded = function (button) {
                var toolbar = button.up('toolbar');
                toolbar.disable();
                if (cswPrivate.allExpanded === false) {
                    cswPublic.expandAll(button, toolbar);
                } else {
                    cswPublic.collapseAll(button, toolbar);
                }
                cswPrivate.allExpanded = !cswPrivate.allExpanded;
            };

            cswPublic.selectNode = function (treeNode, path, onSuccess) {
                /// <summary>
                /// Selects a node from the tree and renders it as the currently selected node
                /// </summary>
                /// <param name="treeNode"></param>
                path = path || cswPublic.getPath(treeNode);
                if (path) {
                    cswPublic.tree.selectPath(path, null, '|', function (succeeded, oLastNode) {
                        Csw.tryExec(onSuccess, succeeded, oLastNode);
                    });
                }
                return false;
            };

            cswPublic.preventSelect = function () {
                cswPrivate.preventSelect = true;
            };
            cswPublic.allowSelect = function () {
                cswPrivate.preventSelect = false;
            };

            cswPublic.addToolbarItem = function (itemDef, position) {
                /// <summary>
                /// Add a docked item to the toolbar
                /// </summary>
                /// <returns type="Csw.composites.tree">This tree</returns>
                if (itemDef && itemDef.text && itemDef.handler) {
                    if (!itemDef.dock) {
                        itemDef.dock = 'top';
                    }
                    cswPublic.tree.addDocked(itemDef, position);
                }
                return cswPublic;
            };

            //#endregion Tree Mutators

            //#region Getters

            cswPublic.getPath = function (treeNode) {
                /// <summary>
                /// Get the path of a tree node from root
                /// </summary>
                /// <returns type="String"></returns>
                var ret = '';
                if (treeNode && treeNode.raw && treeNode.raw.path) {
                    ret = treeNode.raw.path;
                }
                return ret;
            };

            cswPublic.getPathFromId = function (id) {
                /// <summary>
                /// Get the path of a tree node from root
                /// </summary>
                /// <returns type="String"></returns>
                var ret = '';
                if (id) {
                    cswPublic.eachNode(function (node) {
                        var keepIterating = true;
                        if (node && node.raw && node.raw.id === id) {
                            ret = cswPublic.getPath(node);
                            keepIterating = false; //stop iterating when we've found a match
                        }
                        return keepIterating;
                    });
                }
                return ret;
            };

            cswPublic.getChecked = function () {
                /// <summary>
                /// For Multi-Edit, get all nodes which are selected (checked) in the tree.
                /// </summary>
                var checked = cswPublic.tree.getChecked() || [];
                if (checked.length === 0) {
                    //We've switched states and the selectionModel change has not taken effect.
                    cswPublic.eachNode(function (node) {
                        //Iterate the tree. If the node is checked and it validates, add it to the return.
                        if (node && node.raw && node.raw.checked) {
                            if (Csw.tryExec(cswPrivate.allowMultiSelection, node, cswPublic.selectedTreeNode)) {
                                checked.push(node);
                            } else {
                                node.raw.checked = false;
                                node.set('checked', false);
                            }
                        }
                    });
                }
                return checked;
            };

            cswPrivate.eachNodeRecursive = function (node, callBack) {
                var ret = true;
                if (node && callBack) {
                    ret = callBack(node);
                    if (false !== ret &&
                        node.childNodes &&
                        node.childNodes.length > 0) {

                        node.childNodes.forEach(function (childNode) {
                            ret = cswPrivate.eachNodeRecursive(childNode, callBack);
                        });
                    }
                }
                return ret;
            };

            cswPublic.eachNode = function (callBack) {
                if (cswPrivate.rootNode && callBack) {
                    cswPrivate.eachNodeRecursive(cswPrivate.rootNode, callBack);
                }
            };

            //#endregion Getters

            //#endregion Define Class Members


            //#region Post-ctor

            (function _post() {
                if (cswPrivate.root && cswPrivate.root.length >= 1) {
                    cswPrivate.makeTree();
                } else {
                    //throw
                }

            } ());

            //#endregion Post-ctor

            return cswPublic;
        });

} ());
