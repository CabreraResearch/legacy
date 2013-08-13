/// <reference path="CswApp-vsdoc.js" />

(function _mainTearDown() {

    Csw.main.onReady.then(function() {

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

            var designOpt = {};

            Csw.clientState.setCurrentAction(o.actionname, o.actionurl);

            Csw.ajax.post({
                urlMethod: 'SaveActionToQuickLaunch',
                'data': { 'ActionName': o.actionname }
            });

            Csw.main.clear({ 'all': true });
            Csw.main.refreshMainMenu();
            Csw.main.universalsearch.enable();

            var actionName = Csw.string(o.actionname).replace(/_/g, ' ').trim().toLowerCase();
            switch (actionName) {
                case 'create inspection':
                    designOpt = {
                        name: 'cswInspectionDesignWizard',
                        viewid: o.ActionOptions.viewid,
                        viewmode: o.ActionOptions.viewmode,
                        onCancel: function () {
                            Csw.main.clear({ 'all': true });
                            Csw.clientState.setCurrent(Csw.clientState.getLast());
                            Csw.main.refreshSelected();
                        },
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
                    Csw.nbt.createInspectionWizard(Csw.main.centerTopDiv, designOpt);

                    break;
                case 'create material':
                    var createOpt = {
                        state: o.state,
                        request: o.requestitem,
                        onCancel: function () {
                            Csw.main.clear({ 'all': true });
                            Csw.clientState.setCurrent(Csw.clientState.getLast());
                            Csw.main.refreshSelected();
                        },
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
                                            $.CswDialog('AddNodeDialog', {
                                                text: itemData.Text,
                                                nodetypeid: itemData.NodeTypeId,
                                                relatednodeid: actionData.RelatedNodeId,
                                                relatednodename: actionData.RelatedNodeName,
                                                relatednodetypeid: actionData.RelatedNodeTypeId,
                                                relatedobjectclassid: actionData.RelatedObjectClassId,
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
                    Csw.nbt.createMaterialWizard(Csw.main.centerTopDiv, createOpt);
                    break;
                case 'dispensecontainer':
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
                    designOpt = {
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
                        onCancel: function () {
                            Csw.main.clear({ 'all': true });
                            Csw.clientState.setCurrent(Csw.clientState.getLast());
                            Csw.main.refreshSelected();
                        },
                        onFinish: function (viewid) {
                            Csw.main.clear({ 'all': true });
                            Csw.main.handleItemSelect({
                                type: 'view',
                                mode: 'tree',
                                itemid: viewid
                            });
                        }
                    };
                    Csw.nbt.dispenseContainerWizard(Csw.main.centerTopDiv, designOpt);
                    break;
                case 'movecontainer':
                    designOpt = {
                        title: o.title,
                        requestitemid: (o.requestitem) ? o.requestitem.requestitemid : '',
                        location: o.location,
                        onCancel: function () {
                            Csw.main.clear({ all: true });
                            Csw.clientState.setCurrent(Csw.clientState.getLast());
                            Csw.main.refreshSelected();
                        },
                        onSubmit: function (viewid) {
                            Csw.main.clear({ all: true });
                            Csw.clientState.setCurrent(Csw.clientState.getLast());
                            Csw.main.refreshSelected();
                        }
                    };
                    Csw.actions.containerMove(Csw.main.centerTopDiv, designOpt);
                    break;
                case 'edit view':

                    Csw.nbt.vieweditor(Csw.main.centerTopDiv, {
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
                            Csw.main.clear({ 'all': true });
                            Csw.clientState.setCurrent(Csw.clientState.getLast());
                            Csw.main.refreshSelected();
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
                        startingStep: o.ActionOptions.startingStep
                    });
                    break;
                case 'future scheduling':
                    Csw.nbt.futureSchedulingWizard(Csw.main.centerTopDiv, {
                        onCancel: Csw.main.refreshSelected,
                        onFinish: function (viewid, viewmode) {
                            Csw.main.handleItemSelect({ itemid: viewid, mode: viewmode });
                        }
                    });
                    break;
                case 'hmis reporting':
                    Csw.actions.hmisReporting(Csw.main.centerTopDiv, {
                        onSubmit: function () {
                            Csw.main.refreshWelcomeLandingPage();
                        },
                        onCancel: function () {
                            Csw.main.clear({ 'all': true });
                            Csw.clientState.setCurrent(Csw.clientState.getLast());
                            Csw.main.refreshSelected();
                        }
                    });
                    break;
                case 'import data from excel':
                    Csw.nbt.importExcel(Csw.main.centerTopDiv, {
                        onClose: Csw.main.refreshSelected
                    });
                    break;
                case 'login data':
                    Csw.actions.logindata(Csw.main.centerTopDiv, {
                        onCancel: function () {
                            Csw.main.clear({ 'all': true });
                            Csw.clientState.setCurrent(Csw.clientState.getLast());
                            Csw.main.refreshSelected();
                        }
                    });
                    break;
                case 'quotas':
                    Csw.actions.quotas(Csw.main.centerTopDiv, {
                        onQuotaChange: function () {
                            Csw.actions.quotaImage(Csw.main.headerQuota);
                        }
                    });

                    break;
                case 'manage locations':
                    Csw.actions.managelocations(Csw.main.centerTopDiv, {
                        onCancel: function () {
                            Csw.main.clear({ 'all': true });
                            Csw.clientState.setCurrent(Csw.clientState.getLast());
                            Csw.main.refreshSelected();
                        },
                        actionjson: o.ActionOptions
                    });
                    break;
                case 'delete demo data':
                    Csw.actions.deletedemodata(Csw.main.centerTopDiv, {
                        onCancel: function () {
                            Csw.main.clear({ 'all': true });
                            Csw.clientState.setCurrent(Csw.clientState.getLast());
                            Csw.main.refreshSelected();
                        },
                        actionjson: o.ActionOptions
                    });
                    break;
                case 'modules':
                    Csw.actions.modules(Csw.main.centerTopDiv, {
                        onModuleChange: function () {
                            Csw.main.initAll();
                        }
                    });
                    break;
                case 'receiving':
                    o.onFinish = function (viewid) {
                        Csw.main.clear({ 'all': true });
                        Csw.main.handleItemSelect({
                            type: 'view',
                            mode: 'tree',
                            itemid: viewid
                        });
                    };
                    o.onCancel = function () {
                        Csw.main.clear({ 'all': true });
                        Csw.clientState.setCurrent(Csw.clientState.getLast());
                        Csw.main.refreshSelected();
                    };
                    Csw.nbt.receiveMaterialWizard(Csw.main.centerTopDiv, o);
                    break;
                case 'reconciliation':
                    var reconciliationOptions = {
                        onCancel: function () {
                            Csw.main.clear({ 'all': true });
                            Csw.clientState.setCurrent(Csw.clientState.getLast());
                            Csw.main.refreshSelected();
                        },
                        onFinish: function () {
                            Csw.main.refreshWelcomeLandingPage();
                        },
                        startingStep: o.ActionOptions.startingStep
                    };
                    Csw.nbt.ReconciliationWizard(Csw.main.centerTopDiv, reconciliationOptions);
                    break;
                case 'sessions':
                    Csw.actions.sessions(Csw.main.centerTopDiv);
                    break;
                case 'submit request':
                    Csw.actions.requestCarts(Csw.main.centerTopDiv, {
                        onSubmit: function () {
                            //Nada
                        },
                        onCancel: function () {
                            Csw.main.clear({ 'all': true });
                            Csw.clientState.setCurrent(Csw.clientState.getLast());
                            Csw.main.refreshSelected();
                        }
                    });
                    break;
                case 'subscriptions':
                    Csw.actions.subscriptions(Csw.main.centerTopDiv);
                    break;
                case 'tier ii reporting':
                    Csw.actions.tierIIReporting(Csw.main.centerTopDiv, {
                        onSubmit: function () {
                            Csw.main.refreshWelcomeLandingPage();
                        },
                        onCancel: function () {
                            Csw.main.clear({ 'all': true });
                            Csw.clientState.setCurrent(Csw.clientState.getLast());
                            Csw.main.refreshSelected();
                        }
                    });
                    break;
                case 'view scheduled rules':
                    var rulesOpt = {
                        onCancel: function () {
                            Csw.main.clear({ 'all': true });
                            Csw.clientState.setCurrent(Csw.clientState.getLast());
                            Csw.main.refreshSelected();
                        },
                        menuRefresh: Csw.main.refreshSelected
                    };

                    Csw.actions.scheduledRules(Csw.main.centerTopDiv, rulesOpt);
                    break;
                case 'upload legacy mobile data':
                    Csw.nbt.legacyMobileWizard(Csw.main.centerTopDiv, {
                        onCancel: Csw.main.refreshSelected,
                        onFinish: function (viewid, viewmode) {
                            Csw.main.handleItemSelect({ itemid: viewid, mode: viewmode });
                        }
                    });
                    break;
                case 'kiosk mode':
                    Csw.actions.kioskmode(Csw.main.centerTopDiv, {
                        onInit: function () {
                            Csw.main.universalsearch.disable();
                        },
                        onCancel: function () {
                            Csw.main.clear({ 'all': true });
                            Csw.clientState.setCurrent(Csw.clientState.getLast());
                            Csw.main.refreshSelected();
                        }
                    });
                    break;
                default:
                    if (false == Csw.isNullOrEmpty(o.actionurl)) {
                        Csw.window.location(o.actionurl);
                    } else {
                        Csw.main.refreshWelcomeLandingPage();
                    }
                    break;
            }
        });

    });
}());