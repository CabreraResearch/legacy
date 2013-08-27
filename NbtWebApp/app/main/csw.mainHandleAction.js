/// <reference path="CswApp-vsdoc.js" />

(function _mainTearDown() {

    var onCancel = function () {
        Csw.main.clear({ 'all': true });
        Csw.clientState.setCurrent(Csw.clientState.getLast());
        Csw.main.refreshSelected();
    };

    var defaultAct = function (o) {
        if (false == Csw.isNullOrEmpty(o.actionurl)) {
            Csw.window.location(o.actionurl);
        } else {
            return Csw.main.refreshWelcomeLandingPage();
        }
    };

    Csw.main.onReady.then(function () {

        var actionHandler = Csw.object();
        (function buildActHandler() {
            actionHandler.add('create inspection', function (o) {
                var designOpt = {
                    name: 'cswInspectionDesignWizard',
                    viewid: o.ActionOptions.viewid,
                    viewmode: o.ActionOptions.viewmode,
                    onCancel: onCancel,
                    onFinish: function (viewid) {
                        Csw.main.clear({ 'all': true });
                        Csw.main.handleItemSelect({
                            type: 'view',
                            mode: 'tree',
                            itemid: viewid
                        });

                    },
                    startingStep: o.ActionOptions.startingStep,
                    menuRefresh: Csw.main.refreshSelected
                };
                return Csw.nbt.createInspectionWizard(Csw.main.centerTopDiv, designOpt);
            });
            actionHandler.add('create material', function (o) {
                var createOpt = {
                    state: o.state,
                    request: o.requestitem,
                    onCancel: onCancel,
                    onFinish: function (actionData) {
                        var createMaterialLandingPage = function () {
                            Csw.main.setLandingPage(function () {
                                Csw.layouts.landingpage(Csw.main.centerBottomDiv, {
                                    name: 'createMaterialLandingPage',
                                    Title: 'Created:',
                                    ActionId: actionData.ActionId,
                                    ObjectClassId: actionData.RelatedObjectClassId,
                                    onLinkClick: Csw.main.handleItemSelect,
                                    onAddClick: function (itemData) {
                                        Csw.dialogs.addnode({
                                            title: itemData.Text,
                                            nodetypeid: itemData.NodeTypeId,
                                            relatednodeid: actionData.RelatedNodeId,
                                            onAddNode: function (nodeid, nodekey) {
                                                Csw.main.clear({ all: true });
                                                Csw.main.refreshNodesTree({ 'nodeid': nodeid, 'nodekey': nodekey, 'IncludeNodeRequired': true });
                                            }
                                        });
                                    },
                                    onTabClick: function (itemData) {
                                        Csw.cookie.set(Csw.cookie.cookieNames.CurrentTabId, itemData.TabId);
                                        Csw.main.handleItemSelect(itemData);
                                    },
                                    onButtonClick: function (itemData) {
                                        Csw.composites.nodeButton(Csw.main.centerBottomDiv, {
                                            name: itemData.Text,
                                            value: itemData.ActionName,
                                            mode: 'landingpage',
                                            propId: itemData.NodeTypePropId
                                        });
                                    },
                                    onAddComponent: createMaterialLandingPage,
                                    landingPageRequestData: actionData,
                                    onActionLinkClick: function (viewId) {
                                        Csw.main.handleItemSelect({
                                            type: 'view',
                                            mode: 'tree',
                                            itemid: viewId
                                        });
                                    },
                                    isConfigurable: actionData.isConfigurable
                                });
                            });
                        };
                        createMaterialLandingPage();
                    },
                    startingStep: o.ActionOptions.startingStep
                };
                return Csw.nbt.createMaterialWizard(Csw.main.centerTopDiv, createOpt);
            });
            actionHandler.add('dispensecontainer', function (o) {
                var requestItemId = '', requestMode = '';
                if (o.requestitem) {
                    requestItemId = o.requestitem.requestitemid;
                    requestMode = o.requestitem.requestMode;
                }
                var title = o.title;
                if (false === title) {
                    title = 'Dispense from ';
                    if (false === Csw.isNullOrEmpty(o.barcode)) {
                        title += 'Barcode [' + o.barcode + ']';
                    } else {
                        title += 'Selected Container';
                    }
                }
                var designOpt = {
                    title: title,
                    state: {
                        sourceContainerNodeId: o.sourceContainerNodeId,
                        currentQuantity: o.currentQuantity,
                        currentUnitName: o.currentUnitName,
                        precision: o.precision,
                        initialQuantity: o.initialQuantity,
                        requestItemId: requestItemId,
                        requestMode: requestMode,
                        title: title,
                        location: o.location,
                        material: o.material,
                        barcode: o.barcode,
                        containerNodeTypeId: o.containernodetypeid,
                        containerObjectClassId: o.containerobjectclassid,
                        customBarcodes: o.customBarcodes,
                        netQuantityEnforced: o.netQuantityEnforced
                    },
                    onCancel: onCancel,
                    onFinish: function (viewid) {
                        Csw.main.clear({ 'all': true });
                        Csw.main.handleItemSelect({
                            type: 'view',
                            mode: 'tree',
                            itemid: viewid
                        });
                    }
                };
                return Csw.nbt.dispenseContainerWizard(Csw.main.centerTopDiv, designOpt);
            });
            actionHandler.add('movecontainer', function (o) {
                var designOpt = {
                    title: o.title,
                    requestitemid: (o.requestitem) ? o.requestitem.requestitemid : '',
                    location: o.location,
                    onCancel: onCancel,
                    onSubmit: function (viewid) {
                        Csw.main.clear({ all: true });
                        Csw.clientState.setCurrent(Csw.clientState.getLast());
                        Csw.main.refreshSelected();
                    }
                };
                return Csw.actions.containerMove(Csw.main.centerTopDiv, designOpt);
            });
            actionHandler.add('movecontainer', function (o) {
                var designOpt = {
                    title: o.title,
                    requestitemid: (o.requestitem) ? o.requestitem.requestitemid : '',
                    location: o.location,
                    onCancel: onCancel,
                    onSubmit: onCancel
                };
                return Csw.actions.containerMove(Csw.main.centerTopDiv, designOpt);
            });
            actionHandler.add('edit view', function (o) {
                return Csw.nbt.vieweditor(Csw.main.centerTopDiv, {
                    onFinish: function (viewid, viewmode) {
                        Csw.main.clear({ 'all': true });
                        Csw.main.refreshViewSelect();
                        if (Csw.bool(o.ActionOptions.IgnoreReturn)) {
                            Csw.clientState.setCurrent(Csw.clientState.getLast());
                            Csw.main.refreshSelected();
                        } else {
                            Csw.main.handleItemSelect({ itemid: viewid, mode: viewmode });
                        }
                    },
                    onCancel: function () {
                        onCancel();
                        Csw.main.refreshViewSelect();
                    },
                    onDeleteView: function (deletedviewid) {
                        var current = Csw.clientState.getCurrent();
                        if (current.viewid == deletedviewid) {
                            Csw.clientState.clearCurrent();
                        }
                        var last = Csw.clientState.getLast();
                        if (last.viewid == deletedviewid) {
                            Csw.clientState.clearLast();
                        }
                        Csw.main.refreshViewSelect();
                    },
                    selectedViewId: o.ActionOptions.viewid,
                    startingStep: o.ActionOptions.startingStep,
                    viewmode: o.ActionOptions.viewmode
                });
            });
            actionHandler.add('future scheduling', function (o) {
                return Csw.nbt.futureSchedulingWizard(Csw.main.centerTopDiv, {
                    onCancel: onCancel,
                    onFinish: function (viewid, viewmode) {
                        Csw.main.handleItemSelect({ itemid: viewid, mode: viewmode });
                    }
                });
            });
            actionHandler.add('hmis reporting', function (o) {
                return Csw.actions.hmisReporting(Csw.main.centerTopDiv, {
                    onSubmit: function () {
                        Csw.main.refreshWelcomeLandingPage();
                    },
                    onCancel: onCancel
                });
            });
            actionHandler.add('import data from excel', function (o) {
                return Csw.actions.importExcel(Csw.main.centerTopDiv, {
                    onClose: Csw.main.refreshSelected
                });
            });
            actionHandler.add('login data', function (o) {
                return Csw.actions.logindata(Csw.main.centerTopDiv, {
                    onCancel: onCancel
                });
            });
            actionHandler.add('quotas', function (o) {
                return Csw.actions.quotas(Csw.main.centerTopDiv, {
                    onQuotaChange: function () {
                        Csw.actions.quotaImage(Csw.main.headerQuota);
                    }
                });
            });
            actionHandler.add('manage locations', function (o) {
                return Csw.actions.managelocations(Csw.main.centerTopDiv, {
                    onCancel: onCancel,
                    actionjson: o.ActionOptions
                });
            });
            actionHandler.add('delete demo data', function (o) {
                return Csw.actions.deletedemodata(Csw.main.centerTopDiv, {
                    onCancel: onCancel,
                    actionjson: o.ActionOptions
                });
            });
            actionHandler.add('modules', function (o) {
                return Csw.actions.modules(Csw.main.centerTopDiv, {
                    onModuleChange: function () {
                        Csw.main.initAll();
                    }
                });
            });
            actionHandler.add('receiving', function (o) {
                var opts = Csw.extend({}, o);
                opts.onFinish = function (viewid) {
                    Csw.main.clear({ 'all': true });
                    Csw.main.handleItemSelect({
                        type: 'view',
                        mode: 'tree',
                        itemid: viewid
                    });
                };
                opts.onCancel = onCancel;
                return Csw.nbt.receiveMaterialWizard(Csw.main.centerTopDiv, opts);
            });
            actionHandler.add('reconciliation', function (o) {
                var reconciliationOptions = {
                    onCancel: onCancel,
                    onFinish: function () {
                        Csw.main.refreshWelcomeLandingPage();
                    },
                    startingStep: o.ActionOptions.startingStep
                };
                return Csw.nbt.ReconciliationWizard(Csw.main.centerTopDiv, reconciliationOptions);
            });
            actionHandler.add('sessions', function (o) {
                return Csw.actions.sessions(Csw.main.centerTopDiv);
            });
            actionHandler.add('subscriptions', function (o) {
                return Csw.actions.subscriptions(Csw.main.centerTopDiv);
            });
            actionHandler.add('submit request', function (o) {
                return Csw.actions.requestCarts(Csw.main.centerTopDiv, {
                    onSubmit: function () {
                    },
                    onCancel: onCancel
                });
            });
            actionHandler.add('tier ii reporting', function (o) {
                return Csw.actions.tierIIReporting(Csw.main.centerTopDiv, {
                    onSubmit: Csw.main.refreshWelcomeLandingPage,
                    onCancel: onCancel
                });
            });
            actionHandler.add('view scheduled rules', function (o) {
                var rulesOpt = {
                    onCancel: onCancel,
                    menuRefresh: Csw.main.refreshSelected
                };
                return Csw.actions.scheduledRules(Csw.main.centerTopDiv, rulesOpt);
            });
            actionHandler.add('upload legacy mobile data', function (o) {
                return Csw.nbt.legacyMobileWizard(Csw.main.centerTopDiv, {
                    onCancel: onCancel,
                    onFinish: function (viewid, viewmode) {
                        Csw.main.handleItemSelect({ itemid: viewid, mode: viewmode });
                    }
                });
            });
            actionHandler.add('kiosk mode', function (o) {
                return Csw.actions.kioskmode(Csw.main.centerTopDiv, {
                    onInit: function () {
                        Csw.main.universalsearch.disable();
                    },
                    onCancel: onCancel
                });
            });
            actionHandler.add('default', defaultAct);
        }()); //buildActHandler
        
        Csw.subscribe(Csw.enums.events.main.handleAction, function (eventObj, opts) {
            Csw.main.handleAction(opts);
        }); // _handleAction()

        Csw.main.register('handleAction', function (options) {
            var o = {
                actionname: '',
                actionurl: '',
                ActionOptions: {}
            };
            Csw.extend(o, options);

            Csw.main.initGlobalEventTeardown();
            Csw.clientState.setCurrentAction(o.actionname, o.actionurl);
            Csw.ajax.deprecatedWsNbt({
                urlMethod: 'SaveActionToQuickLaunch',
                'data': { 'ActionName': o.actionname }
            });

            Csw.main.clear({ 'all': true });
            Csw.main.refreshMainMenu();
            Csw.main.universalsearch.enable();

            var actionName = Csw.string(o.actionname).replace(/_/g, ' ').trim().toLowerCase();
            actionName = actionName || 'default';
            return actionHandler[actionName](o);
        });

    });
}());