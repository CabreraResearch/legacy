/// <reference path="CswApp-vsdoc.js" />

(function _refreshLandingPage() {

    Csw.main.onReady.then(function() {

        Csw.main.refreshWelcomeLandingPage = function () {
            Csw.main.universalsearch.enable();
            var doLoadLandingPage = function() {
                return Csw.layouts.landingpage(Csw.main.centerBottomDiv, {
                    name: 'welcomeLandingPage',
                    Title: '',

                    onLinkClick: Csw.main.handleItemSelect,
                    onAddClick: function (itemData) {
                        Csw.ajax.abortAll();
                        Csw.dialogs.addnode({
                            action: itemData.ActionName,
                            title: itemData.Text,
                            nodetypeid: itemData.NodeTypeId,
                            onAddNode: function (nodeid, nodekey) {
                                Csw.main.clear({ all: true });
                                Csw.main.refreshNodesTree({ 'nodeid': nodeid, 'nodekey': nodekey, 'IncludeNodeRequired': true });
                            }
                        });
                    },
                    onTabClick: function(itemData) {
                        Csw.cookie.set(Csw.cookie.cookieNames.CurrentTabId, itemData.TabId);
                        Csw.main.handleItemSelect(itemData);
                    },
                    onAddComponent: Csw.main.refreshWelcomeLandingPage,

                    useCache: true,

                    landingPageRequestData: {
                        RoleId: ''
                    }
                });
            };
            return Csw.main.setLandingPage(doLoadLandingPage);
        };

        Csw.main.register('setLandingPage', function (loadLandingPage) {
            var toDo = [];
            toDo.push(Csw.main.clear({ all: true }));
            toDo.push(loadLandingPage());
            toDo.push(Csw.main.refreshMainMenu());
            toDo.push(Csw.main.refreshViewSelect());
            return Q.all(toDo);
        });

        Csw.main.register('refreshLandingPage', function (eventObj, opts) {
            var toDo = [];
            toDo.push(Csw.main.clear({ all: true }));
            var layData = {
                ActionId: '',
                RelatedObjectClassId: '',
                RelatedNodeName: '',
                isConfigurable: false,
                Title: '',
                name: 'CswLandingPage'
            };
            Csw.extend(layData, opts);

            toDo.push(Csw.main.refreshMainMenu());
            toDo.push(Csw.main.refreshViewSelect());

            var lp = Csw.layouts.landingpage(Csw.main.centerBottomDiv, {
                name: layData.name,
                Title: layData.Title,
                ActionId: layData.ActionId,
                ObjectClassId: layData.RelatedObjectClassId,
                onLinkClick: Csw.main.handleItemSelect,
                onAddClick: function (itemData) {
                    Csw.dialogs.addnode({
                        action: itemData.ActionName,
                        title: itemData.Text,
                        nodetypeid: itemData.NodeTypeId,
                        relatednodeid: layData.RelatedNodeId,
                        onAddNode: function (nodeid, nodekey) {
                            Csw.main.clear({ all: true });
                            Csw.main.refreshNodesTree({ nodeid: nodeid, nodekey: nodekey, IncludeNodeRequired: true });
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
                onAddComponent: function () { Csw.publish('refreshLandingPage'); },
                landingPageRequestData: layData,
                onActionLinkClick: function (viewId) {
                    Csw.main.handleItemSelect({
                        type: 'view',
                        mode: 'tree',
                        itemid: viewId
                    });
                },
                isConfigurable: layData.isConfigurable
            });
            toDo.push(lp.promise);
            
            return Q.all(toDo);
        });
        Csw.subscribe('refreshLandingPage', Csw.main.refreshLandingPage);

    });
}());